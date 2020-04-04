using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tcs_service.Models;
using tcs_service.Models.DTO;
using tcs_service.Repos.Interfaces;
using tcs_service.Services;
using tcs_service.Services.Interfaces;

namespace tcs_service.Controllers {

    /// <summary>Reports API</summary>
    [Route ("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase {
        private readonly ISessionRepo _sessionRepo;
        private readonly IClassTourRepo _classTourRepo;
        private readonly IBannerService _bannerService;

        /// <summary>Reports Controller Constructor</summary>
        public ReportsController (ISessionRepo sessionRepo, IClassTourRepo classTourRepo, IBannerService bannerService) {
            _sessionRepo = sessionRepo;
            _classTourRepo = classTourRepo;
            _bannerService = bannerService;
        }

        /// <summary>Retrieve the amount of sessions during each week during a start and end date</summary>
        /// <response code="200">List of Weeks with amount of sessions</response>
        [HttpGet ("weekly-visits")]
        [ProducesResponseType (typeof (IEnumerable<WeeklyVisitsDTO>), 200)]
        public IActionResult Get ([FromQuery] DateTime start, [FromQuery] DateTime end) {
            return Ok (ReportsBusinessLogic.WeeklyVisits (_sessionRepo.GetAll (), start, end));
        }

        /// <summary>
        /// Retrieve the amount of sessions that occurred during
        /// each hour between a start and end date
        /// </summary>
        /// <response code="200">List of Hours with amount of sessions</response>
        [HttpGet ("peakhours")]
        [ProducesResponseType (typeof (IEnumerable<PeakHoursDTO>), 200)]
        public IActionResult PeakHours ([FromQuery] DateTime start, [FromQuery] DateTime end) {
            return Ok (ReportsBusinessLogic.PeakHours (_sessionRepo.GetAll (), start, end));
        }

        /// <summary>
        /// Retrieve a list of the amount of times each classtour has visited the center
        /// between a start and end period
        /// </summary>
        /// <response code="200">List of ClassTourName with amount of times visited</response>
        [HttpGet ("classtours")]
        [ProducesResponseType (typeof (IEnumerable<ClassTourReportDTO>), 200)]
        public IActionResult ClassTours ([FromQuery] DateTime start, [FromQuery] DateTime end) {
            return Ok (ReportsBusinessLogic.ClassTours (_classTourRepo.GetAll (), start, end));
        }

        /// <summary>
        /// Retrieve a list of the amount of times each teacher has
        /// visited the center between a start and end period
        /// </summary>
        /// <response code="200">List of TeacherNames with amount of hours volunteered</response>
        [HttpGet ("volunteers")]
        [ProducesResponseType (typeof (IEnumerable<TeacherSignInTimeDTO>), 200)]
        public IActionResult Volunteers ([FromQuery] DateTime start, [FromQuery] DateTime end) {
            return Ok (ReportsBusinessLogic.Volunteers (_sessionRepo.GetAll (), start, end));
        }

        /// <summary>
        /// Retrive a list of how many visitors came in for a specific reason
        /// and class between a start and end date
        /// </summary>
        /// <response code="200">List of ReasonName with ClassName and how many visitors</response>
        [HttpGet ("reasons")]
        [ProducesResponseType (typeof (IEnumerable<ReasonWithClassVisitsDTO>), 200)]
        public IActionResult Reasons ([FromQuery] DateTime start, [FromQuery] DateTime end) {
            return Ok (ReportsBusinessLogic.Reasons (_sessionRepo.GetAll (), start, end));
        }

        /*
            *Success Report API Rules*
            Success Report must look up all the sessions for the semester Code passed in that were only sessions for students.
            Then it will call banner to get the grade for each class visited for during a session within the semester.
            The ClassGrade returned by banner will be passed off to SuccessReport for it to do the final calculations and that will be returned to the frontend.
        */
        /// <summary>
        /// Retrieve a list of Classes with how many people
        /// passed, failed, completed and unique visitors for that class during a semester
        /// </summary>
        /// <remarks>If you pass in a semester code that doesn't
        /// exist or before final grades have been posted, then this will return a empty list
        ///
        /// How Grades are counted
        /// Passed - A, B, C, OR I
        /// Completed - A, B, C, D, F, I
        /// Dropped, W, WIF
        /// </remarks>
        /// <response code="200">Return List of ClassSuccessCountDTO</response>
        [HttpGet ("success/{semesterCode}")]
        [ProducesResponseType (typeof (IEnumerable<ClassSuccessCountDTO>), 200)]
        public async Task<IActionResult> SuccessReport ([FromRoute] int semesterCode) {
            var tasks = _sessionRepo.GetAll (x => x.SemesterCode == semesterCode &&
                    x.Person.PersonType == PersonType.Student)
                .SelectMany (x => x.SessionClasses.Select (s => new { x.PersonId, s.Class.CRN }))
                .Distinct ()
                .Select (x => _bannerService.GetStudentGrade (x.PersonId, x.CRN, semesterCode));

            var classGrades = await Task.WhenAll (tasks);

            return Ok (ReportsBusinessLogic.SuccessReport (classGrades));
        }
    }
}