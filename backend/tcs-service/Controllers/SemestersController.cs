#pragma warning disable 1591

using Microsoft.AspNetCore.Mvc;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class SemestersController : ControllerBase {
        private ISemesterRepo _iRepo;

        public SemestersController (ISemesterRepo iRepo) {
            _iRepo = iRepo;
        }

        [HttpGet]
        public IActionResult Semesters () {
            return Ok (_iRepo.GetAll ());
        }
    }
}