using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tcs_service.EF;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignInsController : ControllerBase
    {
        private ISignInRepo _iRepo;

        public SignInsController( ISignInRepo iRepo )
        {
            _iRepo = iRepo;
        }

        [HttpGet]
        [Produces(typeof(DbSet<SignIn>))]
        public IActionResult GetSignIn()
        {
            var results = new ObjectResult(_iRepo.GetAll())
            {
                StatusCode = (int)HttpStatusCode.OK
            };

            return results;
        }

        [HttpGet("{id}")]
        [Produces(typeof(SignIn))]
        public async Task<IActionResult> GetSignIn([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var signIn = await _iRepo.Find(id);

            if (signIn == null)
            {
                return NotFound();
            }

            return Ok(signIn);
        }

        [HttpPost]
        public async Task<IActionResult> PostSignIn([FromBody] SignInViewModel signInViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SignIn signIn = new SignIn();
            signIn.PersonId = signInViewModel.PersonId;
            signIn.SemesterId = signInViewModel.SemesterId;
            signIn.Tutoring = signInViewModel.Tutoring;
            signIn.InTime = DateTime.Now;

            foreach(Course course in signInViewModel.Courses)
            {
                var signInCourse = new SignInCourse
                {
                    SignInID = signIn.ID,
                    CourseID = course.CRN
                };

                await _iRepo.AddCourse(course);

                signIn.Courses.Add(signInCourse);
            }

            foreach (Reason reason in signInViewModel.Reasons)
            {
                await _iRepo.AddReason(reason);

                var signInReason = new SignInReason
                {
                    SignInID = signIn.ID,
                    ReasonID = reason.ID
                };

                signIn.Reasons.Add(signInReason);
            }

            await _iRepo.Add(signIn);
            return CreatedAtAction("GetSignIn", new { id = signIn.ID }, signIn);
        }

        [HttpPut("/signIns/{id}")]
        public async Task<IActionResult> PutSignIn([FromRoute] int id, [FromBody] SignIn signIn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != signIn.ID)
            {
                return BadRequest();
            }

            try
            {
                await _iRepo.Update(signIn);
                return Ok(signIn);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _iRepo.Exist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //PUT: api/SignIns/5/SignOut
        [HttpPut("{id}")]
        public async Task<IActionResult> SignOut([FromRoute] int id, [FromBody] SignIn signIn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != signIn.ID)
            {
                return BadRequest("IDs do not match");
            }
            if(signIn.InTime == null)
            {
                return BadRequest("Student is not signed in");
            }

            signIn.OutTime = DateTime.Now;
            await _iRepo.Update(signIn);
            return Ok(signIn);
        }
    }
}