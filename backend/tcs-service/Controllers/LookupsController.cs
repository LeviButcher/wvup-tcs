using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tcs_service.Helpers;
using tcs_service.Models.ViewModels;
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
        public async Task<ActionResult<PagingModel<SignInViewModel>>> Get([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] int crn, [FromQuery] string email, [FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            var page = await _iRepo.Get(start, end, crn, email, skip, take);
            if (page.isNext)
            {
                Response.Headers.Add("Next", $"/api/lookups/?start={start}&end={end}&crn={crn}&email={email}&skip={page.Skip + page.Take}&take={page.Take}");
            }
            if (page.isPrev)
            {
                Response.Headers.Add("Prev", $"/api/lookups/?start={start}&end={end}&crn={crn}&email={email}&skip={page.Skip - page.Take}&take={page.Take}");
            }
            Response.Headers.Add("Total-Pages", $"{page.TotalPages}");
            Response.Headers.Add("Total-Records", $"{page.TotalDataCount}");
            Response.Headers.Add("Current-Page", $"{page.CurrentPage}");

            return Ok(page.data);
        }

        [HttpGet("daily")]
        public async Task<ActionResult<PagingModel<SignInViewModel>>> Daily([FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            var page = await _iRepo.Daily(skip, take);
            if (page.isNext)
            {
                Response.Headers.Add("Next", $"/api/lookups/?skip={page.Skip + page.Take}&take={page.Take}");
            }
            if (page.isPrev)
            {
                Response.Headers.Add("Prev", $"/api/lookups/?skip={page.Skip - page.Take}&take={page.Take}");
            }
            Response.Headers.Add("Total-Pages", $"{page.TotalPages}");
            Response.Headers.Add("Total-Records", $"{page.TotalDataCount}");
            Response.Headers.Add("Current-Page", $"{page.CurrentPage}");

            return Ok(page.data);
        }

        [HttpGet("crn/{crn}")]
        public async Task<ActionResult<PagingModel<SignInViewModel>>> CRN(int crn, [FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            var page = await _iRepo.GetByCRN(crn, start, end, skip, take);
            if (page.isNext)
            {
                Response.Headers.Add("Next", $"/api/lookups/?start={start}&end={end}&skip={page.Skip + page.Take}&take={page.Take}");
            }
            if (page.isPrev)
            {
                Response.Headers.Add("Prev", $"/api/lookups/?start={start}&end={end}&skip={page.Skip - page.Take}&take={page.Take}");
            }
            Response.Headers.Add("Total-Pages", $"{page.TotalPages}");
            Response.Headers.Add("Total-Records", $"{page.TotalDataCount}");
            Response.Headers.Add("Current-Page", $"{page.CurrentPage}");

            return Ok(page.data);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<PagingModel<SignInViewModel>>> Email(string email, [FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            var page = await _iRepo.GetByEmail(email, start, end, skip, take);
            if (page.isNext)
            {
                Response.Headers.Add("Next", $"/api/lookups/?start={start}&end={end}&skip={page.Skip + page.Take}&take={page.Take}");
            }
            if (page.isPrev)
            {
                Response.Headers.Add("Prev", $"/api/lookups/?start={start}&end={end}&skip={page.Skip - page.Take}&take={page.Take}");
            }
            Response.Headers.Add("Total-Pages", $"{page.TotalPages}");
            Response.Headers.Add("Total-Records", $"{page.TotalDataCount}");
            Response.Headers.Add("Current-Page", $"{page.CurrentPage}");

            return Ok(page.data);
        }
    }
}