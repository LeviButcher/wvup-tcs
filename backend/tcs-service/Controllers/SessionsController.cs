using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Models.DTOs;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private ISessionRepo _iRepo;
        private IMapper _mapper;

        public SessionsController(ISessionRepo iRepo, IMapper mapper)
        {
            _mapper = mapper;
            _iRepo = iRepo;
        }

        [HttpGet]
        public IActionResult GetSessions([FromQuery] int page = 1)
        {
            var pageResult = new Paging<Session>(page, _iRepo.GetAll());
            return Ok(pageResult);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession([FromRoute] int id)
        {
            var session = await _iRepo.Find(x => x.ID == id);

            if (session == null)
            {
                return NotFound();
            }

            return Ok(session);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] SessionCreateDTO sessionDTO)
        {
            var session = _mapper.Map<Session>(sessionDTO);
            await _iRepo.Create(session);
            return Ok(session);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSession([FromRoute] int id, [FromBody] Session session)
        {
            if (id != session.ID)
            {
                return BadRequest();
            }
            try
            {
                var result = await _iRepo.Update(session);
                return Ok(result);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound(new { message = "Something went wrong" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession([FromRoute] int id)
        {
            var session = await _iRepo.Remove(x => x.ID == id);
            return Ok(session);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {            
            await _iRepo.Remove(x => x.ID == id);
            return Ok();    
        }
    }
}