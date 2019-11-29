using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReasonsController : ControllerBase
    {
        private IReasonRepo _iRepo;

        public ReasonsController(IReasonRepo iRepo)
        {
            _iRepo = iRepo;
        }

        [HttpGet]
        public IActionResult GetReasons() => Ok(_iRepo.GetAll());

        [HttpGet("{id}")]
        [Produces(typeof(Reason))]
        public async Task<IActionResult> GetReason([FromRoute] int id)
        {
            var reason = await _iRepo.Find(x => x.Id == id);

            if (reason == null)
            {
                return NotFound();
            }

            return Ok(reason);
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public IActionResult GetActive() => Ok(_iRepo.GetAll(x => x.Deleted == false));

        [HttpPost]
        public async Task<IActionResult> PostReason([FromBody] Reason reason)
        {
            await _iRepo.Create(reason);
            return Created($"reasons/{reason.Id}", reason);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReason([FromRoute] int id, [FromBody]Reason reason)
        {
            if (id != reason.Id)
            {
                return BadRequest();
            }

            try
            {
                await _iRepo.Update(reason);
                return Ok(reason);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound(new { message = "Something went terribly wrong" });
            }
        }
    }
}