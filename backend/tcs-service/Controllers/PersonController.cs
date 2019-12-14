using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tcs_service.UnitOfWorks;

namespace tcs_service.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IUnitOfWorkPerson unitPerson;

        public PersonController(IUnitOfWorkPerson unitPerson)
        {
            this.unitPerson = unitPerson;
        }

        [HttpGet("{identifier}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetInfo(string identifier)
        {
            return Ok(await unitPerson.GetPersonInfo(identifier, DateTime.Now));
        }
    }
}