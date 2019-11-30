using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PersonController : ControllerBase
    {
        readonly IPersonRepo personRepo;

        public PersonController(IPersonRepo personRepo)
        {
            this.personRepo = personRepo;
        }

        [HttpGet("{email}")]
        public IActionResult GetWithEmail(string email)
        {
            //Check

            return Ok();
        }

        [HttpGet("{id:int}")]
        public IActionResult GetWithId(int id)
        {
            return Ok();
        }
    }
}