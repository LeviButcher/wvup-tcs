﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Models.DTOs;
using tcs_service.Repos.Interfaces;
using tcs_service.UnitOfWorks.Interfaces;

namespace tcs_service.Controllers {

    /// <summary>Sessions API</summary>
    [Route ("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SessionsController : ControllerBase {
        readonly private ISessionRepo _sessionRepo;
        readonly private ISemesterRepo _semesterRepo;
        readonly private IPersonRepo _personRepo;
        private readonly ISessionReasonRepo _sessionReasonRepo;
        private readonly ISessionClassRepo _sessionClassRepo;
        readonly private IMapper _mapper;
        readonly private ICSVParser<CSVSessionUpload> _csvParser;
        private readonly IUnitOfWorkSession _unitSession;

        /// <summary>Sessions Controller Constructor</summary>
        public SessionsController (ISessionRepo sessionRepo, ISemesterRepo semesterRepo, IPersonRepo personRepo,
            ISessionReasonRepo sessionReasonRepo, ISessionClassRepo sessionClassRepo, IMapper mapper, ICSVParser<CSVSessionUpload> csvParser, IUnitOfWorkSession unitSession) {
            _mapper = mapper;
            _csvParser = csvParser;
            _unitSession = unitSession;
            _sessionRepo = sessionRepo;
            _semesterRepo = semesterRepo;
            _personRepo = personRepo;
            _sessionReasonRepo = sessionReasonRepo;
            _sessionClassRepo = sessionClassRepo;
        }

        /// <summary>
        ///  Returns a list of Sessions between the start and end date
        /// and matching the passed in crn, email
        /// </summary>
        /// <remarks>
        /// If dataset is large, only ten records will be returned at a time, pass in the next page number to
        /// get the next page
        /// page numbers passed the max amount of pages returns empty lists
        /// </remarks>
        /// <response code="200">Returns List of Sessions</response>
        [HttpGet]
        [ProducesResponseType (typeof (IEnumerable<SessionDisplayDTO>), 200)]
        public IActionResult GetSessions ([FromQuery] DateTime? start, [FromQuery] DateTime? end, [FromQuery] int? crn, [FromQuery] string email, [FromQuery] int page = 1) {
            var resultSet = _sessionRepo.GetAll ();
            if (start.HasValue) {
                resultSet = resultSet.Where (x => x.InTime.Date >= start.Value.Date);
            }
            if (end.HasValue) {
                resultSet = resultSet.Where (x => x.OutTime.GetValueOrDefault (end.Value).Date <= end.Value.Date);
            }
            if (crn.HasValue) {
                resultSet = resultSet.Where (x => x.SessionClasses.Any (c => c.ClassId == crn.Value));
            }
            if (!string.IsNullOrEmpty (email)) {
                resultSet = resultSet.Where (x => x.Person.Email == email);
            }
            var sessionsDisplay = resultSet.Select (x => _mapper.Map<SessionDisplayDTO> (x));
            var pageResult = new Paging<SessionDisplayDTO> (page, sessionsDisplay);
            return Ok (pageResult);
        }

        /// <summary>
        ///  Returns a list of the sessions that happened during the semester that match semesterCode
        /// </summary>
        /// <remarks>
        /// This returns ALL sessions during a semester, dataset will be large
        /// </remarks>
        /// <response code="200">Returns List of Sessions</response>
        [HttpGet ("semester/{semesterCode}")]
        [ProducesResponseType (typeof (IEnumerable<SessionDisplayDTO>), 200)]
        public IActionResult SemesterSession (int semesterCode) {
            var result = _sessionRepo.GetAll (x => x.SemesterCode == semesterCode).Select (x => _mapper.Map<SessionDisplayDTO> (x));
            return Ok (result);
        }

        /// <summary>
        ///  Returns the session that matches the id
        /// </summary>
        /// <response code="200">Returns Session</response>
        /// <response code="404">No session could be found</response>
        [HttpGet ("{id}")]
        [ProducesResponseType (typeof (SessionInfoDTO), 200)]
        public async Task<IActionResult> GetSession ([FromRoute] int id) {
            var session = await _sessionRepo.Find (x => x.Id == id);

            if (session == null) {
                return NotFound ();
            }
            var sessionUpdate = _mapper.Map<SessionInfoDTO> (session);

            return Ok (sessionUpdate);
        }

        /// <summary>
        ///  Returns the Sessions that are currently signed in
        /// </summary>
        /// <response code="200">Returns list of Sessions</response>
        [HttpGet ("in")]
        [ProducesResponseType (typeof (IEnumerable<Session>), 200)]
        public IActionResult GetSignedIn () => Ok (_sessionRepo.GetAll (x => x.OutTime == null));

        /// <summary>
        ///  Create a new Session
        /// </summary>
        /// <response code="201">Session was created</response>
        /// <response code="500">Session did not have 1 or more class selected</response>
        /// <response code="500">Session did not have 1 or more reason selected (tutoring counts as a reason)</response>
        [HttpPost]
        [ProducesResponseType (typeof (Session), 201)]
        [ProducesResponseType (typeof (ErrorMessage), 500)]
        public async Task<IActionResult> PostSession ([FromBody] SessionPostOrPutDTO sessionDTO) {
            var person = await _personRepo.Find (x => x.Id == sessionDTO.PersonId);
            if (person.PersonType == PersonType.Teacher) {
                var sessionTeacher = _mapper.Map<Session> (sessionDTO);
                await _sessionRepo.Create (sessionTeacher);
                return Created ($"sessions/{sessionTeacher.Id}", sessionTeacher);
            }

            if (sessionDTO.SelectedClasses.Count < 1) {
                throw new TCSException ("Must select at least one class.");
            }

            if (sessionDTO.SelectedReasons.Count < 1 && !sessionDTO.Tutoring) {
                throw new TCSException ("Must select at least one reason for visit.");
            }

            var session = _mapper.Map<Session> (sessionDTO);
            session.SessionClasses = sessionDTO.SelectedClasses.Select (x => new SessionClass () { ClassId = x }).ToList ();
            session.SessionReasons = sessionDTO.SelectedReasons.Select (x => new SessionReason () { ReasonId = x }).ToList ();
            await _sessionRepo.Create (session);

            return Created ($"sessions/{session.Id}", session);
        }

        /// <summary>
        ///  Updates a Session with the associated id
        /// </summary>
        /// <response code="200">Session was updated</response>
        /// <response code="500">Session did not have 1 or more class selected</response>
        /// <response code="500">Session did not have 1 or more reason selected (tutoring counts as a reason)</response>
        [HttpPut ("{id}")]
        [ProducesResponseType (typeof (Session), 200)]
        [ProducesResponseType (typeof (ErrorMessage), 500)]
        public async Task<IActionResult> UpdateSession ([FromRoute] int id, [FromBody] SessionPostOrPutDTO sessionDTO) {
            var person = await _personRepo.Find (x => x.Id == sessionDTO.PersonId);
            var isTeacher = person.PersonType == PersonType.Teacher;

            if (id != sessionDTO.Id) {
                throw new TCSException ("Session does not exist");
            }

            if (!isTeacher) {

                if (sessionDTO.SelectedClasses.Count < 1) {
                    throw new TCSException ("Must select at least one class.");
                }

                if (sessionDTO.SelectedReasons.Count < 1 && !sessionDTO.Tutoring) {
                    throw new TCSException ("Must select at least one reason for visit.");
                }
            }

            try {
                var session = new Session () {
                    Id = sessionDTO.Id,
                    InTime = sessionDTO.InTime,
                    OutTime = sessionDTO.OutTime,
                    PersonId = sessionDTO.PersonId,
                    SemesterCode = sessionDTO.SemesterCode,
                    Tutoring = sessionDTO.Tutoring,
                };

                await _sessionRepo.Update (session);
                if (!isTeacher) {
                    await _sessionClassRepo.RemoveAll (x => x.SessionId == sessionDTO.Id);
                    foreach (var x in sessionDTO.SelectedClasses) {
                        await _sessionClassRepo.Create (new SessionClass () { SessionId = sessionDTO.Id, ClassId = x });
                    }
                    await _sessionReasonRepo.RemoveAll (x => x.SessionId == sessionDTO.Id);
                    foreach (var x in sessionDTO.SelectedReasons) {
                        await _sessionReasonRepo.Create (new SessionReason () { SessionId = sessionDTO.Id, ReasonId = x });
                    }
                }
                var updatedSession = await _sessionRepo.Find (x => x.Id == sessionDTO.Id);
                return Ok (updatedSession);
            } catch (DbUpdateConcurrencyException) {
                return NotFound (new { message = "Something went wrong" });
            }
        }

        /// <summary>
        ///  Creates a New Session for someone coming into the center
        /// </summary>
        /// <response code="201">Session was created</response>
        /// <response code="500">Session did not have 1 or more class selected</response>
        /// <response code="500">Session did not have 1 or more reason selected (tutoring counts as a reason)</response>
        /// <response code="500">The Person was already signed in today</response>
        [HttpPost ("in")]
        [AllowAnonymous]
        [ProducesResponseType (typeof (Session), 201)]
        [ProducesResponseType (typeof (ErrorMessage), 500)]
        public async Task<IActionResult> SignIn ([FromBody] KioskSignInDTO signIn) {
            var person = await _personRepo.Find (x => x.Id == signIn.PersonId);
            if (person.PersonType == PersonType.Teacher) {
                var teacherSession = new Session () {
                InTime = DateTime.Now,
                PersonId = signIn.PersonId,
                SemesterCode = _semesterRepo.GetAll ().Last ().Code
                };
                var teacherResult = await _sessionRepo.Create (teacherSession);
                if (teacherResult is Session) {
                    return Created ($"sessions/{teacherResult.Id}", teacherResult);
                }
                throw new TCSException ("Something went wrong");
            }

            if (!signIn.Tutoring && signIn.SelectedReasons.Count () < 1) throw new TCSException ("Must select 1 or more reasons for visiting.");

            var alreadySignedIn = await _sessionRepo.Exist (x => x.PersonId == signIn.PersonId && x.OutTime == null);
            if (alreadySignedIn) throw new TCSException ("You are already signed in.");

            var session = new Session () {
                InTime = DateTime.Now,
                SessionClasses = signIn.SelectedClasses.Select (x => new SessionClass () { ClassId = x }).ToList (),
                SessionReasons = signIn.SelectedReasons.Select (x => new SessionReason () { ReasonId = x }).ToList (),
                PersonId = signIn.PersonId,
                SemesterCode = _semesterRepo.GetAll ().Last ().Code,
                Tutoring = signIn.Tutoring
            };

            var result = await _sessionRepo.Create (session);
            if (result is Session) {
                return Created ($"sessions/{result.Id}", result);
            }
            throw new TCSException ("Something went wrong");
        }

        /// <summary>
        ///  Updates the outtime of the Person's last sessions
        /// </summary>
        /// <response code="201">Session was created</response>
        /// <response code="500">The Person is not signed in</response>
        [HttpPut ("out")]
        [AllowAnonymous]
        [ProducesResponseType (typeof (Session), 201)]
        [ProducesResponseType (typeof (ErrorMessage), 500)]
        public async Task<IActionResult> SignOut (KioskSignOutDTO signOut) {
            var session = await _sessionRepo.Find (x => x.PersonId == signOut.PersonId && x.OutTime == null);

            if (!(session is Session)) throw new TCSException ("You aren't signed in.");

            session.OutTime = DateTime.Now;
            var result = await _sessionRepo.Update (session);

            return Ok (result);
        }

        /// <summary>
        ///  Deletes the session matching the associated id
        /// </summary>
        /// <response code="200">Session was deleted</response>
        [HttpDelete ("{id}")]
        [ProducesResponseType (typeof (Session), 200)]
        public async Task<IActionResult> DeleteSession ([FromRoute] int id) {
            var session = await _sessionRepo.Remove (x => x.Id == id);
            return Ok (session);
        }

        /// <summary>
        ///  Uploads a CSV File of Sessions to save
        /// </summary>
        /// <remarks>
        ///     CSV File must have the following headers:
        ///     Email, InTime, OutTime, CRNs, Reasons, SemesterCode, Tutoring
        ///     Delimit all fields by semicolons (;)
        ///     Lists are plural (s) and are comma seperated
        ///     EX: lbutche3@wvup.edu;2020-03-28 14:00:00;2020-03-28 16:00:00;1,2;Printer Use;201903;TRUE
        /// </remarks>
        /// <response code="200">Sessions was uploads</response>
        /// <response code="500">Sessions failed to upload</response>
        [HttpPost ("upload")]
        [ProducesResponseType (typeof (int), 200)]
        [ProducesResponseType (typeof (ErrorMessage), 500)]
        public async Task<IActionResult> UploadCSV (IFormFile csvFile) {
            var csvData = _csvParser.Parse (csvFile);
            return Ok (await _unitSession.UploadSessions (csvData));
        }
    }
}