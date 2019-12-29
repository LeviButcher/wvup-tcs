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

namespace tcs_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private ISessionRepo _sessionRepo;
        private IClassTourRepo _classTourRepo;
        private IBannerService _bannerService;

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

        [HttpGet("success/{semesterId}")]
        public async Task<IActionResult> SuccessReport(int semesterId)
        {
            var studentCourses = from item in _sessionRepo.GetAll()
                                 from course in item.SessionClasses
                                 where item.SemesterCode == semesterId
                                 select new
                                 {
                                     item.PersonId,
                                     course.Class,
                                     course.Class.Department,
                                 };

            List<ClassWithGradeDTO> classessWithGrades = new List<ClassWithGradeDTO>();

            foreach (var item in studentCourses.Distinct())
            {
                try
                {
                    var grade = await _bannerService.GetStudentGrade(item.PersonId, item.Class.CRN, semesterId);
                    classessWithGrades.Add(grade);
                }
                catch
                {
                    throw new TCSException("Something went wrong");
                }
            }

            return Ok(ReportsBusinessLogic.SuccessReport(classessWithGrades));
        }
    }
}
