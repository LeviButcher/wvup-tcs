using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Helpers;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers
{
    [Produces("application/json")]
    [Route("api/ClassTours")]
    [Authorize]
    [ApiController]
    public class ClassToursController : ControllerBase
    {
        private readonly IClassTourRepo _iRepo;

        public ClassToursController(IClassTourRepo iRepo)
        {
            _iRepo = iRepo;
        }

        private async Task<bool> ClassTourExists(int id)
        {
            return await _iRepo.Exist(x => x.Id == id);
        }

        [HttpGet("{id}")]
        [Produces(typeof(ClassTour))]
        public async Task<IActionResult> GetClassTour([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var classTour = await _iRepo.Find(x => x.Id == id);

            if (classTour == null)
            {
                return NotFound();
            }

            return Ok(classTour);
        }

        [HttpGet]
        public ActionResult Get([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            var page = _iRepo.GetBetweenDates(start, end, skip, take);

            return Ok(page);
        }

        [HttpPut("{id}")]
        [Produces(typeof(ClassTour))]
        public async Task<IActionResult> PutClassTour([FromRoute] int id, [FromBody] ClassTour classTour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != classTour.Id)
            {
                return BadRequest("ID's do not match");
            }

            try
            {
                await _iRepo.Update(classTour);
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

            await _iRepo.Create(classTour);

            return CreatedAtAction("GetClassTour", new { id = classTour.Id }, classTour);
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

            var tour = await _iRepo.Remove(x => x.Id == id);

            return Ok(tour);
        }
    }
}