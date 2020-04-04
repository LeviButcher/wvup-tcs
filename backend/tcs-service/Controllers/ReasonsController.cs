using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers {

    /// <summary>Reasons API</summary>
    [Route ("api/[controller]")]
    [ApiController]
    public class ReasonsController : ControllerBase {
        readonly private IReasonRepo _iRepo;

        /// <summary>Reasons Controller Constructor</summary>
        public ReasonsController (IReasonRepo iRepo) {
            _iRepo = iRepo;
        }

        /// <summary>Returns a list of all reasons</summary>
        /// <response code="200">All Reasons</response>
        [HttpGet]
        [ProducesResponseType (typeof (IEnumerable<Reason>), 200)]
        public IActionResult GetReasons () => Ok (_iRepo.GetAll ());

        /// <summary>Returns a reason with the matching Id</summary>
        /// <response code="200">Reason</response>
        /// <response code="404">No reason exists with Id</response>
        [HttpGet ("{id}")]
        [ProducesResponseType (typeof (Reason), 200)]
        [ProducesResponseType (404)]
        public async Task<IActionResult> GetReason ([FromRoute] int id) {
            var reason = await _iRepo.Find (x => x.Id == id);

            if (reason == null) {
                return NotFound ();
            }

            return Ok (reason);
        }

        /// <summary>Returns a list of all active reasons</summary>
        /// <response code="200">Returns All Active Reasons</response>
        [ProducesResponseType (typeof (IEnumerable<Reason>), 200)]
        [AllowAnonymous]
        [HttpGet ("active")]
        public IActionResult GetActive () => Ok (_iRepo.GetAll (x => x.Deleted == false));

        /// <summary>Creates a new reason</summary>
        /// <response code="201">Reason was created</response>
        /// <response code="500">Reason failed to create</response>
        [ProducesResponseType (typeof (Reason), 201)]
        [ProducesResponseType (typeof (ErrorMessage), 500)]
        [HttpPost]
        public async Task<IActionResult> PostReason ([FromBody] Reason reason) {
            await _iRepo.Create (reason);
            return Created ($"reasons/{reason.Id}", reason);
        }

        /// <summary>Updates a reason with the associated Id</summary>
        /// <response code="200">Reason was updated</response>
        /// <response code="500">Reason failed to update</response>
        [ProducesResponseType (typeof (Reason), 200)]
        [ProducesResponseType (typeof (ErrorMessage), 500)]
        [HttpPut ("{id}")]
        public async Task<IActionResult> PutReason ([FromRoute] int id, [FromBody] Reason reason) {
            if (id != reason.Id) {
                return BadRequest ();
            }

            try {
                await _iRepo.Update (reason);
                return Ok (reason);
            } catch (DbUpdateConcurrencyException) {
                return NotFound (new { message = "Something went terribly wrong" });
            }
        }
    }
}