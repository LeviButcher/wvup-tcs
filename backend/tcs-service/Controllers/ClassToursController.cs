using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers {

    /// <summary>Class Tour API</summary>
    [Route ("api/ClassTours")]
    [ApiController]
    [Authorize]
    public class ClassToursController : ControllerBase {
        private readonly IClassTourRepo _iRepo;

        /// <summary>ClassTour Constructor</summary>
        public ClassToursController (IClassTourRepo iRepo) {
            _iRepo = iRepo;
        }

        /// <summary>
        /// Retrieves a class tour by unique id
        /// </summary>
        /// <response code="200">Class Tour Returned</response>
        /// <response code="404">Class Tour doe not exist</response>
        [HttpGet ("{id}")]
        [ProducesResponseType (typeof (ClassTour), 200)]
        [ProducesResponseType (404)]
        public async Task<IActionResult> GetClassTour ([FromRoute] int id) {
            var classTour = await _iRepo.Find (x => x.Id == id);

            if (classTour == null) {
                return NotFound ();
            }

            return Ok (classTour);
        }

        /// <summary>
        /// Retrieves a list of class tours between a start and end date
        /// </summary>
        /// <response code="200">Class Tours Returned</response>
        [HttpGet]
        [ProducesResponseType (typeof (Paging<ClassTour>), 200)]
        public IActionResult Get ([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] int page = 1) {
            var pageResult = new Paging<ClassTour> (page,
                _iRepo.GetAll (a => a.DayVisited >= start && a.DayVisited <= end));

            return Ok (pageResult);
        }

        /// <summary>
        /// Updates a ClassTour with the associated Id
        /// </summary>
        /// <response code="200">Class Tours Updated</response>
        /// <response code="500">Class Tour Update Failed, Error Messages Returned</response>
        [HttpPut ("{id}")]
        [ProducesResponseType (typeof (ClassTour), 200)]
        [ProducesResponseType (typeof (ErrorMessage), 500)]
        public async Task<IActionResult> PutClassTour ([FromRoute] int id, [FromBody] ClassTour classTour) {
            if (id != classTour.Id) {
                throw new TCSException ("Class Tour does not exist");
            }

            try {
                await _iRepo.Update (classTour);
                return Ok (classTour);
            } catch (DbUpdateConcurrencyException) {
                return NotFound (new { message = "Something went wrong" });
            }

        }

        /// <summary>
        /// Creates a new ClassTour
        /// </summary>
        /// <response code="201">Class Tour Created</response>
        /// <response code="500">Class Tour Creation Failed</response>
        [HttpPost]
        [ProducesResponseType (typeof (ClassTour), 201)]
        [ProducesResponseType (typeof (ErrorMessage), 500)]
        public async Task<IActionResult> PostClassTour ([FromBody] ClassTour classTour) {
            await _iRepo.Create (classTour);

            return Created ($"classtours/{classTour.Id}", classTour);
        }

        /// <summary>
        /// Delete a ClassTour
        /// </summary>
        /// <response code="200">Class Tour Deleted</response>
        /// <response code="404">Class Tour Not Found</response>
        /// <response code="505">Class Tour Deletion Failed</response>
        [HttpDelete ("{id}")]
        [ProducesResponseType (typeof (ClassTour), 200)]
        [ProducesResponseType (typeof (ErrorMessage), 500)]
        [ProducesResponseType (typeof (ErrorMessage), 404)]
        public async Task<IActionResult> DeleteClassTour ([FromRoute] int id) {
            if (!await _iRepo.Exist (x => x.Id == id)) {
                return NotFound (new { message = "Something went wrong" });
            }
            var tour = await _iRepo.Remove (x => x.Id == id);

            return Ok (tour);
        }
    }
}