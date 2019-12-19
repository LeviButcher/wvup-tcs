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
    [Route("api/ClassTours")]
    [ApiController]
    public class ClassToursController : ControllerBase
    {
        private readonly IClassTourRepo _iRepo;

        public ClassToursController(IClassTourRepo iRepo)
        {
            _iRepo = iRepo;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClassTour([FromRoute] int id)
        {
            var classTour = await _iRepo.Find(x => x.Id == id);

            if (classTour == null)
            {
                return NotFound();
            }

            return Ok(classTour);
        }

        [HttpGet]
        public IActionResult Get([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] int page = 1)
        {
            var pageResult = new Paging<ClassTour>(page, _iRepo.GetBetweenDates(start, end));
            
            return Ok(pageResult);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutClassTour([FromRoute] int id, [FromBody] ClassTour classTour)
        {
            if (id != classTour.Id)
            {
                throw new TCSException("Class Tour does not exist");
            }

            try
            {
                await _iRepo.Update(classTour);
                return Ok(classTour);
            }
            catch (DbUpdateConcurrencyException)
            {
               return NotFound(new { message = "Something went wrong" });
            }

        }

        [HttpPost]
        public async Task<IActionResult> PostClassTour([FromBody] ClassTour classTour)
        {
            var tour = await _iRepo.Create(classTour);

            return Ok(tour);
        }

        [HttpDelete("{id}")]
        [Produces(typeof(ClassTour))]
        public async Task<IActionResult> DeleteClassTour([FromRoute] int id)
        {
            var tour = await _iRepo.Remove(x => x.Id == id);
            return Ok(tour);
        }
    }
}