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

        public SignInsController(ISignInRepo iRepo)
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
        public async Task<IActionResult> PostSignIn([FromBody] SignInViewModel signInViewModel, bool teacher)
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

            if (!await _iRepo.PersonExist(signInViewModel.PersonId))
            {
                var person = new Person
                {
                    ID = signInViewModel.PersonId,
                    PersonType = signInViewModel.Type,
                    FirstName = signInViewModel.FirstName,
                    LastName = signInViewModel.LastName,
                    Email = signInViewModel.Email
                };

                await _iRepo.AddPerson(person);
            }

            if(!teacher)
            {
                if (signInViewModel.Tutoring && signInViewModel.Courses == null)
                {
                    return BadRequest("Must select one or more courses");
                }

                if (!signInViewModel.Tutoring && signInViewModel.Reasons == null)
                {
                    return BadRequest("Must select one or more reason for visit");
                }

                if (signInViewModel.Courses != null)
                {

                    foreach (Course course in signInViewModel.Courses)
                    {
                        course.DepartmentID = course.Department.Code;
                        if (await _iRepo.DepartmentExist(course.Department.Code))
                        {
                            course.Department = null;
                        }

                        var signInCourse = new SignInCourse
                        {
                            SignInID = signIn.ID,
                            CourseID = course.CRN
                        };


                        if (!await _iRepo.CourseExist(course.CRN))
                        {
                            await _iRepo.AddCourse(course);
                        }

                        signIn.Courses.Add(signInCourse);
                    }
                }

                if (signInViewModel.Reasons != null)
                {
                    foreach (Reason reason in signInViewModel.Reasons)
                    {
                        if (!await _iRepo.ReasonExist(reason.ID))
                        {
                            await _iRepo.AddReason(reason);
                        }

                        var signInReason = new SignInReason
                        {
                            SignInID = signIn.ID,
                            ReasonID = reason.ID
                        };

                        signIn.Reasons.Add(signInReason);
                    }
                }
            }
            
            await _iRepo.Add(signIn);
            return Created("GetSignIn", new { id = signIn.ID });
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

        [HttpPut("{id}/IDsignOut")]
        public async Task<IActionResult> SignOut([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var signIn = await GetMostRecentById(id);

            if(signIn == null)
            {
                return NotFound();
            }

            if (signIn.OutTime != null)
            {
                return BadRequest("Student is not signed in");
            }

            signIn.OutTime = DateTime.Now;
            await _iRepo.Update(signIn);
            return Ok(signIn);
        }

        [HttpPut("{email}/SignOut")]
        public async Task<IActionResult> SignOut([FromRoute] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var signIn = await GetMostRecentByEmail(email);

            if (signIn == null)
            {
                return NotFound(new {message="Student is not signed in"});
            }

            if (signIn.OutTime != null)
            {
                return BadRequest("Student is not signed in");
            }

            signIn.OutTime = DateTime.Now;
            await _iRepo.Update(signIn);
            return Ok(signIn);
        }

        [HttpGet("{studentID}/id")]
        public StudentInfoViewModel GetStudentInfoWithID([FromRoute] int studentID)
        {
            return _iRepo.GetStudentInfoWithID(studentID);
        }

        // GET: api/SignIns/student@wvup.edu/email
        [HttpGet("{studentEmail}/email")]
        public StudentInfoViewModel GetStudentInfoWithEmail([FromRoute] string studentEmail)
        {
            return _iRepo.GetStudentInfoWithEmail(studentEmail);
        }
       
        private async Task<SignIn> GetMostRecentById(int id)
        {
            if(! await _iRepo.PersonExist(id))
            {
                return null;
            }

            return  _iRepo.GetMostRecentSignInByID(id);
        }

        private async Task<SignIn> GetMostRecentByEmail(string email)
        {
            if (!await _iRepo.PersonExist(email))
            {
                return null;
            }

            return _iRepo.GetMostRecentSignInByEmail(email);
        }
    }
}