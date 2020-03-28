using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Models.DTOs;
using tcs_service.Repos.Interfaces;
using tcs_service.UnitOfWorks.Interfaces;

namespace tcs_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SessionsController : ControllerBase
    {
        readonly private ISessionRepo _sessionRepo;
        readonly private ISemesterRepo _semesterRepo;
        readonly private IPersonRepo _personRepo;
        private readonly ISessionReasonRepo _sessionReasonRepo;
        private readonly ISessionClassRepo _sessionClassRepo;
        readonly private IMapper _mapper;
        readonly private ICSVParser<CSVSessionUpload> _csvParser;
        private readonly IUnitOfWorkSession _unitSession;

        public SessionsController(ISessionRepo sessionRepo, ISemesterRepo semesterRepo, IPersonRepo personRepo,
            ISessionReasonRepo sessionReasonRepo, ISessionClassRepo sessionClassRepo, IMapper mapper, ICSVParser<CSVSessionUpload> csvParser, IUnitOfWorkSession unitSession)
        {
            _mapper = mapper;
            _csvParser = csvParser;
            _unitSession = unitSession;
            _sessionRepo = sessionRepo;
            _semesterRepo = semesterRepo;
            _personRepo = personRepo;
            _sessionReasonRepo = sessionReasonRepo;
            _sessionClassRepo = sessionClassRepo;
        }

        [HttpGet]
        public IActionResult GetSessions([FromQuery] DateTime? start, [FromQuery] DateTime? end, [FromQuery] int? crn, [FromQuery] string email, [FromQuery] int page = 1)
        {
            var resultSet = _sessionRepo.GetAll();
            if (start.HasValue)
            {
                resultSet = resultSet.Where(x => x.InTime.Date >= start.Value.Date);
            }
            if (end.HasValue)
            {
                resultSet = resultSet.Where(x => x.OutTime.GetValueOrDefault(end.Value).Date <= end.Value.Date);
            }
            if (crn.HasValue)
            {
                resultSet = resultSet.Where(x => x.SessionClasses.Any(c => c.ClassId == crn.Value));
            }
            if (!string.IsNullOrEmpty(email))
            {
                resultSet = resultSet.Where(x => x.Person.Email == email);
            }
            var sessionsDisplay = resultSet.Select(x => _mapper.Map<SessionDisplayDTO>(x));
            var pageResult = new Paging<SessionDisplayDTO>(page, sessionsDisplay);
            return Ok(pageResult);
        }

        [HttpGet("semester/{semesterCode}")]
        public IActionResult SemesterSession(int semesterCode)
        {
            var result = _sessionRepo.GetAll(x => x.SemesterCode == semesterCode).Select(x => _mapper.Map<SessionDisplayDTO>(x));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession([FromRoute] int id)
        {
            var session = await _sessionRepo.Find(x => x.Id == id);

            if (session == null)
            {
                return NotFound();
            }
            var sessionUpdate = _mapper.Map<SessionInfoDTO>(session);

            return Ok(sessionUpdate);
        }

        [HttpGet("in")]
        public IActionResult GetSignedIn() => Ok(_sessionRepo.GetAll(x => x.OutTime == null));

        [HttpPost]
        public async Task<IActionResult> PostSession([FromBody] SessionPostOrPutDTO sessionDTO)
        {
            var person = await _personRepo.Find(x => x.Id == sessionDTO.PersonId);
            if (person.PersonType == PersonType.Teacher)
            {
                var sessionTeacher = _mapper.Map<Session>(sessionDTO);
                await _sessionRepo.Create(sessionTeacher);
                return Created($"sessions/{sessionTeacher.Id}", sessionTeacher);
            }

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

            return Created($"sessions/{session.Id}", session);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSession([FromRoute] int id, [FromBody] SessionPostOrPutDTO sessionDTO)
        {
            var person = await _personRepo.Find(x => x.Id == sessionDTO.PersonId);
            var isTeacher = person.PersonType == PersonType.Teacher;

            if (id != sessionDTO.Id)
            {
                throw new TCSException("Session does not exist");
            }

            if (!isTeacher)
            {

                if (sessionDTO.SelectedClasses.Count < 1)
                {
                    throw new TCSException("Must select at least one class.");
                }

                if (sessionDTO.SelectedReasons.Count < 1 && !sessionDTO.Tutoring)
                {
                    throw new TCSException("Must select at least one reason for visit.");
                }
            }

            try
            {
                var session = new Session()
                {
                    Id = sessionDTO.Id,
                    InTime = sessionDTO.InTime,
                    OutTime = sessionDTO.OutTime,
                    PersonId = sessionDTO.PersonId,
                    SemesterCode = sessionDTO.SemesterCode,
                    Tutoring = sessionDTO.Tutoring,
                };

                await _sessionRepo.Update(session);
                if (!isTeacher)
                {
                    await _sessionClassRepo.RemoveAll(x => x.SessionId == sessionDTO.Id);
                    foreach (var x in sessionDTO.SelectedClasses)
                    {
                        await _sessionClassRepo.Create(new SessionClass() { SessionId = sessionDTO.Id, ClassId = x });
                    }
                    await _sessionReasonRepo.RemoveAll(x => x.SessionId == sessionDTO.Id);
                    foreach (var x in sessionDTO.SelectedReasons)
                    {
                        await _sessionReasonRepo.Create(new SessionReason() { SessionId = sessionDTO.Id, ReasonId = x });
                    }
                }
                var updatedSession = await _sessionRepo.Find(x => x.Id == sessionDTO.Id);
                return Ok(updatedSession);
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
            var person = await _personRepo.Find(x => x.Id == signIn.PersonId);
            if (person.PersonType == PersonType.Teacher)
            {
                var teacherSession = new Session()
                {
                    InTime = DateTime.Now,
                    PersonId = signIn.PersonId,
                    SemesterCode = _semesterRepo.GetAll().Last().Code
                };
                var teacherResult = await _sessionRepo.Create(teacherSession);
                if (teacherResult is Session)
                {
                    return Created($"sessions/{teacherResult.Id}", teacherResult);
                }
                throw new TCSException("Something went wrong");
            }

            if (!signIn.Tutoring && signIn.SelectedReasons.Count() < 1) throw new TCSException("Must select 1 or more reasons for visiting.");

            var alreadySignedIn = await _sessionRepo.Exist(x => x.PersonId == signIn.PersonId && x.OutTime == null);
            if (alreadySignedIn) throw new TCSException("You are already signed in.");

            var session = new Session()
            {
                InTime = DateTime.Now,
                SessionClasses = signIn.SelectedClasses.Select(x => new SessionClass() { ClassId = x }).ToList(),
                SessionReasons = signIn.SelectedReasons.Select(x => new SessionReason() { ReasonId = x }).ToList(),
                PersonId = signIn.PersonId,
                SemesterCode = _semesterRepo.GetAll().Last().Code,
                Tutoring = signIn.Tutoring
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

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession([FromRoute] int id)
        {
            var session = await _sessionRepo.Remove(x => x.Id == id);
            return Ok(session);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCSV(IFormFile csvFile)
        {
            var csvData = _csvParser.Parse(csvFile);
            return Ok(await _unitSession.UploadSessions(csvData));
        }
    }
}