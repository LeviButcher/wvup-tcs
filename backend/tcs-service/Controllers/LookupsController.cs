using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupsController : ControllerBase
    {
        private readonly ILookupRepo _iRepo;

        public LookupsController(ILookupRepo iRepo)
        {
            _iRepo = iRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SignIn>>> Get([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            return Ok(await _iRepo.Get(start, end, skip, take));
        }
    }
}