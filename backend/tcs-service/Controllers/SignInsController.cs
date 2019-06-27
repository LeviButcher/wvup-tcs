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
        private readonly TCSContext _context;
        private ISignInRepo _iRepo;

        public SignInsController(TCSContext context, ISignInRepo iRepo)
        {
            _context = context;
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
    }
}