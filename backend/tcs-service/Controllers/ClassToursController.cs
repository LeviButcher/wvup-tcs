using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers
{
    [Produces("application/json")]
    [Route("api/ClassTours")]
    [ApiController]
    public class ClassToursController : ControllerBase
    {
        private readonly IClassTourRepo _classTourRepo;

        public ClassToursController(IClassTourRepo classTourRepo)
        {
            _classTourRepo = classTourRepo;
        }

        private async Task<bool> ClassTourExists(int id)
        {
            return await _classTourRepo.Exist(id);
        }

        [HttpGet]
        [Produces(typeof(DbSet<ClassTour>))]
        public IActionResult GetClassTour()
        {
            var results = new ObjectResult(_classTourRepo.GetAll())
            {
                StatusCode = (int)HttpStatusCode.OK
            };

            //Request.HttpContext.Response.Headers.Add("X-Total-Count", _classTourRepo.GetAll().Count().ToString());

            return results;
        }
            
        [HttpGet("{id}")]
        [Produces(typeof(ClassTour))]
        public async Task<IActionResult> GetClassTour([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var classTour = await _classTourRepo.Find(id);

            if (classTour == null)
            {
                return NotFound();
            }

            return Ok(classTour);
        }

        [HttpPut("{id}")]
        [Produces(typeof(ClassTour))]
        public async Task<IActionResult> PutClassTour([FromRoute] int id, [FromBody] ClassTour classTour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != classTour.ID)
            {
                return BadRequest();
            }

            try
            {
                await _classTourRepo.Update(classTour);
                return Ok(classTour);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ClassTourExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

        }

        [HttpPost]
        [Produces(typeof(ClassTour))]
        public async Task<IActionResult> PostClassTour([FromBody] ClassTour classTour)
        {
            if (!ModelState.IsValid || classTour.Name == null)
            {
                return BadRequest(ModelState);
            }

            await _classTourRepo.Add(classTour);

            return CreatedAtAction("GetClassTour", new { id = classTour.ID }, classTour);
        }

        [HttpDelete("{id}")]
        [Produces(typeof(ClassTour))]
        public async Task<IActionResult> DeleteClassTour([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await ClassTourExists(id))
            {
                return NotFound();
            }

            var tour = await _classTourRepo.Remove(id);

            return Ok(tour);
        }
    }
}