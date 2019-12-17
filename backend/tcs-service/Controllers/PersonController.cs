using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tcs_service.UnitOfWorks.Interfaces;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IUnitOfWorkPerson unitPerson;
        private readonly IScheduleRepo scheduleRepo;

        public PersonController(IUnitOfWorkPerson unitPerson, IScheduleRepo scheduleRepo)
        {
            this.unitPerson = unitPerson;
            this.scheduleRepo = scheduleRepo;
        }

        [HttpGet("{identifier}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetInfo(string identifier)
        {
            return Ok(await unitPerson.GetPersonInfo(identifier, DateTime.Now));
        }

        [HttpGet("{identifier}/admin")]
        public async Task<IActionResult> GetInfoAdmin(string identifier)
        {
            var personInfo = await unitPerson.GetPersonInfo(identifier, DateTime.Now);
            var schedules = scheduleRepo.GetAll(x => x.PersonId == personInfo.Id);
            personInfo.Schedule = schedules.Select(x => x.Class);

            return Ok(personInfo);
        }
    }
}