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

namespace tcs_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private IReportsRepo _iRepo;
        private ISessionRepo _sessionRepo;

        public ReportsController(IReportsRepo iRepo, ISessionRepo sessionRepo)
        {
            _iRepo = iRepo;
            _sessionRepo = sessionRepo;
        }

        [HttpGet("weekly-visits")]
        public async Task<ActionResult<IEnumerable<WeeklyVisitsDTO>>> Get([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return ReportsBusinessLogic.WeeklyVisits( _sessionRepo.GetAll(), start, end);
           // return Ok(await _iRepo.WeeklyVisits(start, end));
        }

        [HttpGet("peakhours")]
        public async Task<ActionResult<IEnumerable<PeakHoursDTO>>> PeakHours([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(await _iRepo.PeakHours(start, end));
        }

        [HttpGet("classtours")]
        public async Task<ActionResult<IEnumerable<ClassTourReportDTO>>> ClassTours([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(await _iRepo.ClassTours(start, end));
        }

        [HttpGet("volunteers")]
        public async Task<ActionResult<IEnumerable<TeacherSignInTimeDTO>>> Volunteers([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(await _iRepo.Volunteers(start, end));
        }

        [HttpGet("reasons")]
        public async Task<ActionResult<IEnumerable<ReasonWithClassVisitsDTO>>> Reasons([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(await _iRepo.Reasons(start, end));
        }

        [HttpGet("success/{semesterId}")]
        public async Task<IActionResult> SuccessReport(int semesterId)
        { 
            return Ok(ReportsBusinessLogic.SuccessReport(await _iRepo.SuccessReport(semesterId)));
        }
    }
}