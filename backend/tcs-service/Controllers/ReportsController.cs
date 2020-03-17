using System.Collections.Specialized;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tcs_service.Helpers;
using tcs_service.Models.DTO;
using tcs_service.Repos.Interfaces;
using tcs_service.Services;
using tcs_service.Services.Interfaces;
using tcs_service.Models;

namespace tcs_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ISessionRepo _sessionRepo;
        private readonly IClassTourRepo _classTourRepo;
        private readonly IBannerService _bannerService;

        public ReportsController(ISessionRepo sessionRepo, IClassTourRepo classTourRepo, IBannerService bannerService)
        {
            _sessionRepo = sessionRepo;
            _classTourRepo = classTourRepo;
            _bannerService = bannerService;
        }

        [HttpGet("weekly-visits")]
        public IActionResult Get([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(ReportsBusinessLogic.WeeklyVisits(_sessionRepo.GetAll(), start, end));
        }

        [HttpGet("peakhours")]
        public IActionResult PeakHours([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(ReportsBusinessLogic.PeakHours(_sessionRepo.GetAll(), start, end));
        }

        [HttpGet("classtours")]
        public IActionResult ClassTours([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(ReportsBusinessLogic.ClassTours(_classTourRepo.GetAll(), start, end));
        }

        [HttpGet("volunteers")]
        public IActionResult Volunteers([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(ReportsBusinessLogic.Volunteers(_sessionRepo.GetAll(), start, end));
        }

        [HttpGet("reasons")]
        public IActionResult Reasons([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(ReportsBusinessLogic.Reasons(_sessionRepo.GetAll(), start, end));
        }

        /*
            *Success Report Rules*
            Success Report must look up all the sessions for the semester Code passed in that were only sessions for students.
            Then it will call banner to get the grade for each class visited for during a session within the semester.
            The ClassGrade returned by banner will be passed off to SuccessReport for it to do the final calculations and that will be returned to the frontend.
        */
        [HttpGet("success/{semesterCode}")]
        public async Task<IActionResult> SuccessReport([FromRoute]int semesterCode)
        {
            var tasks = _sessionRepo.GetAll(x => x.SemesterCode == semesterCode
                && x.Person.PersonType == PersonType.Student)
                .SelectMany(x => x.SessionClasses.Select(s => new { x.PersonId, s.Class.CRN }))
                .Distinct()
                .Select(x => _bannerService.GetStudentGrade(x.PersonId, x.CRN, semesterCode));

            var classGrades = await Task.WhenAll(tasks);

            return Ok(ReportsBusinessLogic.SuccessReport(classGrades));
        }
    }
}