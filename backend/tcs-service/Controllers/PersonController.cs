using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tcs_service.Helpers;
using tcs_service.Models.DTOs;
using tcs_service.Repos.Interfaces;
using tcs_service.UnitOfWorks.Interfaces;

namespace tcs_service.Controllers {

    /// <summary>Person API</summary>
    [Route ("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PersonController : ControllerBase {
        private readonly IUnitOfWorkPerson unitPerson;
        private readonly IScheduleRepo scheduleRepo;

        /// Person Controller Constructor
        public PersonController (IUnitOfWorkPerson unitPerson, IScheduleRepo scheduleRepo) {
            this.unitPerson = unitPerson;
            this.scheduleRepo = scheduleRepo;
        }

        /// <summary>Get a Person Info by their Email OR Id</summary>
        /// <remark>Only returns this person's current schedule</remark>
        /// <response code="200">Person Info Returned</response>
        /// <response code="500">Student Information could not be found</response>
        [HttpGet ("{identifier}")]
        [ProducesResponseType (typeof (PersonInfoDTO), 200)]
        [ProducesResponseType (typeof (ErrorMessage), 500)]
        [AllowAnonymous]
        public async Task<IActionResult> GetInfo (string identifier) {
            return Ok (await unitPerson.GetPersonInfo (identifier));
        }

        /// <summary>Get a Person Info by their Email OR Id FOR ADMINS</summary>
        /// <remark>This api returns all the courses a person has ever had
        ///     during a semester they tried to sign in
        /// </remark>
        /// <response code="200">Person Info Returned</response>
        /// <response code="500">Student Information could not be found</response>
        [HttpGet ("{identifier}/admin")]
        [ProducesResponseType (typeof (PersonInfoDTO), 200)]
        [ProducesResponseType (typeof (ErrorMessage), 500)]
        public async Task<IActionResult> GetInfoAdmin (string identifier) {
            var personInfo = await unitPerson.GetPersonInfo (identifier);
            var schedules = scheduleRepo.GetAll (x => x.PersonId == personInfo.Id);
            personInfo.Schedule = schedules.Select (x => x.Class);

            return Ok (personInfo);
        }
    }
}