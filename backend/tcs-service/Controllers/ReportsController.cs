using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Interfaces;

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
        public async Task<ActionResult<IEnumerable<ReportCountViewModel>>> Get([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(await _iRepo.WeeklyVisits(start, end));
        }

        [HttpGet("peakhours")]
        public async Task<ActionResult<IEnumerable<ReportCountViewModel>>> PeakHours([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(await _iRepo.PeakHours(start, end));
        }
    }
}