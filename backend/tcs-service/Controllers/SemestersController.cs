using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers {

    /// <summary>Semesters API</summary>
    [Route ("api/[controller]")]
    [ApiController]
    public class SemestersController : ControllerBase {
        readonly private ISemesterRepo _iRepo;

        /// <summary>Semesters Controller Constructor</summary>
        public SemestersController (ISemesterRepo iRepo) {
            _iRepo = iRepo;
        }

        /// <summary>Retrieves all semesters</summary>
        /// <response code="200">Returns list of Semesters</response>
        [HttpGet]
        [ProducesResponseType (typeof (IEnumerable<Semester>), 200)]
        public IActionResult Semesters () {
            return Ok (_iRepo.GetAll ());
        }
    }
}