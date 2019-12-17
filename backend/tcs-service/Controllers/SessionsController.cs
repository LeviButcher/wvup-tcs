using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private ISessionRepo _iRepo;
        private IMapper _mapper;

        public SessionsController(ISessionRepo iRepo, IMapper mapper)
        {
            _mapper = mapper;
            _iRepo = iRepo;
        }

        [HttpGet]
        public IActionResult GetSessions([FromQuery] int page = 1)
        {
            var pageResult = new Paging<Session>(page, _iRepo.GetAll());
            return Ok(pageResult);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession([FromRoute] int id)
        {
            var session = await _iRepo.Find(x => x.Id == id);

            if (session == null)
            {
                return NotFound();
            }

            return Ok(session);
        }

        [HttpGet("signedin")]
        public IActionResult GetSignedIn() => Ok(_iRepo.GetAll(x => x.OutTime == null));

        [HttpPost]
        public async Task<IActionResult> PostSession([FromBody] SessionCreateDTO sessionDTO)
        {
            if(sessionDTO.SelectedClasses.Count < 1 )
            {
                return BadRequest(new { message = "Must select at least one class." });
            }

            if(sessionDTO.SelectedReasons.Count < 1 && !sessionDTO.Tutoring )
            {
                return BadRequest(new { message = "Must select at least one reason for visit." });
            }

            var session = _mapper.Map<Session>(sessionDTO);
            sessionDTO.SelectedClasses.ForEach(x => session.SessionClasses.Add(new SessionClass() { ClassId = x }));
            sessionDTO.SelectedReasons.ForEach(x => session.SessionReasons.Add(new SessionReason() { ReasonId = x }));
            await _iRepo.Create(session);

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
                var result = await _iRepo.Update(session);
                return Ok(result);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound(new { message = "Something went wrong" });
            }
        }

        [HttpPut("signout/{studentId}")]
        public async Task<IActionResult> SignOut([FromRoute] int studentId)
        {
            var session = _iRepo.GetAll(x => x.PersonId == studentId).OrderBy(x => x.Id).LastOrDefault();
            
            if(session.OutTime != null)
            {
                return BadRequest(new { message = "You aren't signed in" });
            }

            session.OutTime = DateTime.Now;
            await _iRepo.Update(session);

            return Ok(session);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession([FromRoute] int id)
        {
            var session = await _iRepo.Remove(x => x.Id == id);
            return Ok(session);
        }
    }
}