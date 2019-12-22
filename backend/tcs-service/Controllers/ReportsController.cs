using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Interfaces;
using tcs_service.Services;

namespace tcs_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private IReportsRepo _iRepo;

        public ReportsController(IReportsRepo iRepo)
        {
            _iRepo = iRepo;
        }

        [HttpGet("weekly-visits")]
        public async Task<ActionResult<IEnumerable<WeeklyVisitsViewModel>>> Get([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(await _iRepo.WeeklyVisits(start, end));
        }

        [HttpGet("peakhours")]
        public async Task<ActionResult<IEnumerable<PeakHoursViewModel>>> PeakHours([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(await _iRepo.PeakHours(start, end));
        }

        [HttpGet("classtours")]
        public async Task<ActionResult<IEnumerable<ClassTourReportViewModel>>> ClassTours([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(await _iRepo.ClassTours(start, end));
        }

        [HttpGet("volunteers")]
        public async Task<ActionResult<IEnumerable<TeacherSignInTimeViewModel>>> Volunteers([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(await _iRepo.Volunteers(start, end));
        }

        [HttpGet("reasons")]
        public async Task<ActionResult<IEnumerable<ReasonWithClassVisitsViewModel>>> Reasons([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(await _iRepo.Reasons(start, end));
        }

        [HttpGet("success/{semesterId}")]
        public async Task<IActionResult> SuccessReport(int semesterId)
        {
            var courses = await _iRepo.SuccessReport(semesterId);
            
            return Ok(ReportsBusinessLogic.SuccessReport(courses));
        }
    }
}