﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Models.DTOs;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        readonly private ISessionRepo _sessionRepo;
        readonly private ISemesterRepo _semesterRepo;
        readonly private IMapper _mapper;

        public SessionsController(ISessionRepo sessionRepo, ISemesterRepo semesterRepo, IMapper mapper)
        {
            _mapper = mapper;
            _sessionRepo = sessionRepo;
            _semesterRepo = semesterRepo;
        }

        [HttpGet]
        public IActionResult GetSessions([FromQuery] int page = 1)
        {
            var pageResult = new Paging<Session>(page, _sessionRepo.GetAll());
            return Ok(pageResult);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession([FromRoute] int id)
        {
            var session = await _sessionRepo.Find(x => x.Id == id);

            if (session == null)
            {
                return NotFound();
            }

            return Ok(session);
        }

        [HttpGet("in")]
        public IActionResult GetSignedIn() => Ok(_sessionRepo.GetAll(x => x.OutTime == null));

        [HttpPost]
        public async Task<IActionResult> PostSession([FromBody] SessionCreateDTO sessionDTO)
        {
            if (sessionDTO.SelectedClasses.Count < 1)
            {
                throw new TCSException("Must select at least one class.");
            }

            if (sessionDTO.SelectedReasons.Count < 1 && !sessionDTO.Tutoring)
            {
                throw new TCSException("Must select at least one reason for visit.");
            }

            var session = _mapper.Map<Session>(sessionDTO);
            session.SessionClasses = sessionDTO.SelectedClasses.Select(x => new SessionClass() { ClassId = x }).ToList();
            session.SessionReasons = sessionDTO.SelectedReasons.Select(x => new SessionReason() { ReasonId = x }).ToList();
            await _sessionRepo.Create(session);

            return Ok(session);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSession([FromRoute] int id, [FromBody] SessionCreateDTO sessionDTO)
        {
            if (id != sessionDTO.Id)
            {
                return BadRequest();
            }

            if (sessionDTO.SelectedClasses.Count < 1)
            {
                return BadRequest(new { message = "Must select at least one class." });
            }

            if (sessionDTO.SelectedReasons.Count < 1 && !sessionDTO.Tutoring)
            {
                return BadRequest(new { message = "Must select at least one reason for visit." });
            }

            try
            {
                var session = _mapper.Map<Session>(sessionDTO);
                var result = await _sessionRepo.Update(session);
                return Ok(result);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound(new { message = "Something went wrong" });
            }
        }

        [HttpPost("in")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] KioskSignInDTO signIn)
        {
            var alreadySignedIn = await _sessionRepo.Exist(x => x.PersonId == signIn.PersonId && x.OutTime == null);
            if (alreadySignedIn) throw new TCSException("You are already signed in.");

            var session = new Session()
            {
                InTime = DateTime.Now,
                SessionClasses = signIn.SelectedClasses.Select(x => new SessionClass() { ClassId = x }).ToList(),
                SessionReasons = signIn.SelectedReasons.Select(x => new SessionReason() { ReasonId = x }).ToList(),
                PersonId = signIn.PersonId,
                SemesterCode = _semesterRepo.GetAll().Last().Code
            };

            var result = await _sessionRepo.Create(session);
            if (result is Session)
            {
                return Created($"sessions/{result.Id}", result);
            }
            throw new TCSException("Something went wrong");
        }

        [HttpPut("out")]
        [AllowAnonymous]
        public async Task<IActionResult> SignOut(KioskSignOutDTO signOut)
        {
            var session = await _sessionRepo.Find(x => x.PersonId == signOut.PersonId && x.OutTime == null);

            if (!(session is Session)) throw new TCSException("You aren't signed in.");

            session.OutTime = DateTime.Now;
            var result = await _sessionRepo.Update(session);

            return Ok(session);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession([FromRoute] int id)
        {
            var session = await _sessionRepo.Remove(x => x.Id == id);
            return Ok(session);
        }
    }
}