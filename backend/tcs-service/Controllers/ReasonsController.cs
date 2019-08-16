using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        [Produces(typeof(DbSet<Reason>))]
        public IActionResult GetReason()
        {
            var results = new ObjectResult(_iRepo.GetAll())
            {
                StatusCode = (int)HttpStatusCode.OK
            };

            return results;
        }

        [HttpGet("{id}")]
        [Produces(typeof(Reason))]
        public async Task<IActionResult> GetReason([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reason = await _iRepo.Find(id);

            if (reason == null)
            {
                return NotFound();
            }

            return Ok(reason);
        }

        [HttpGet("active")]
        public IActionResult GetActive()
        {
            return Ok(_iRepo.GetActive());
        }

        [HttpPost]
        public async Task<IActionResult> PostReason([FromBody] Reason reason)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _iRepo.Add(reason);
            return Created("GetReason", new { id = reason.ID });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReason([FromRoute] int id, [FromBody]Reason reason)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reason.ID)
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
                if (!await _iRepo.Exist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
    }
}