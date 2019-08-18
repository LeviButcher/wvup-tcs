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
    public class SignInsController : ControllerBase
    {
        private ISignInRepo _iRepo;
        private IMapper _mapper;

        public SignInsController(ISignInRepo iRepo, IMapper mapper)
        {
            _mapper = mapper;
            _iRepo = iRepo;
        }

        [HttpGet]
        [Produces(typeof(DbSet<SignIn>))]
        public IActionResult GetSignIn() => Ok(_iRepo.GetAll());

        [HttpGet("{id}")]
        [Produces(typeof(SignInViewModel))]
        public async Task<IActionResult> GetSignIn([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var signIn = await _iRepo.GetSignInViewModel(id);

            if (signIn == null)
            {
                return NotFound();
            }

            return Ok(signIn);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostSignIn([FromBody] SignInViewModel signInViewModel, bool teacher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var signIn = _mapper.Map<SignIn>(signInViewModel);
            signIn.InTime = DateTimeOffset.UtcNow;

            var recent = await GetMostRecentById(signIn.PersonId);

            if (recent != null && recent.OutTime == null)
            {
                throw new TCSException("You are already signed in");
            }

            if (!teacher)
            {
                if (signInViewModel.Courses == null || signInViewModel.Courses.Count < 1)
                {
                    throw new TCSException("Must select one or more courses");
                }

                if (!signInViewModel.Tutoring && signInViewModel.Reasons == null)
                {
                    throw new TCSException("Must select one or more reason for visit");
                }
            }

            await _iRepo.Add(signIn);
            return Created("GetSignIn", new { id = signIn.ID });
        }

        [HttpPost("admin")]
        public async Task<IActionResult> PostSignInAdmin([FromBody] SignInViewModel signInViewModel, bool teacher)
        {
            var signIn = _mapper.Map<SignIn>(signInViewModel);
            await _iRepo.Add(signIn);

            if (signIn.OutTime == null)
            {
                throw new Exception("Must Select an OutTime");
            }
            return Created("GetSignIn", new { id = signIn.ID });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSignIn([FromRoute] int id, [FromBody] SignInViewModel signInViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != signInViewModel.Id)
            {
                return BadRequest();
            }
            var signIn = _mapper.Map<SignIn>(signInViewModel);
            try
            {
                var result = await _iRepo.Update(signIn);
                return Ok(result);
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


        [AllowAnonymous]
        [HttpPut("{id:int}/SignOut")]
        public async Task<IActionResult> SignOut([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var signIn = await GetMostRecentById(id);

            if (signIn == null || signIn.OutTime != null)
            {
                throw new TCSException("You are not signed in");
            }

            signIn.OutTime = DateTimeOffset.UtcNow;
            await _iRepo.Update(signIn);
            return Ok(signIn);
        }

        [AllowAnonymous]
        [HttpPut("{email}/SignOut")]
        public async Task<IActionResult> SignOut([FromRoute] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var signIn = await GetMostRecentByEmail(email);

            if (signIn == null || signIn.OutTime != null)
            {
                throw new TCSException("You are not signed in");
            }

            signIn.OutTime = DateTimeOffset.UtcNow;
            await _iRepo.Update(signIn);
            return Ok(signIn);
        }

        [AllowAnonymous]
        [HttpGet("{studentID}/id")]
        public async Task<IActionResult> GetStudentInfoWithID([FromRoute] int studentID)
        {
            return Ok(await _iRepo.GetStudentInfoWithID(studentID));
        }

        // GET: api/SignIns/student@wvup.edu/email
        [AllowAnonymous]
        [HttpGet("{studentEmail}/email")]
        public async Task<IActionResult> GetStudentInfoWithEmail([FromRoute] string studentEmail)
        {
            return Ok(await _iRepo.GetStudentInfoWithEmail(studentEmail));
        }

        [AllowAnonymous]
        [HttpGet("{teacherID}/teacher/id")]
        public async Task<IActionResult> GetTeacherInfoWithID([FromRoute] int teacherID)
        {
            return Ok(await _iRepo.GetTeacherInfoWithID(teacherID));
        }

        [AllowAnonymous]
        // GET: api/SignIns/teacher@wvup.edu/teacher/email
        [HttpGet("{teacherEmail}/teacher/email")]
        public async Task<IActionResult> GetTeacherInfoWithEmail([FromRoute] string teacherEmail)
        {
            return Ok(await _iRepo.GetTeacherInfoWithEmail(teacherEmail));
        }

        private async Task<SignIn> GetMostRecentById(int id)
        {
            if (!await _iRepo.PersonExist(id))
            {
                return null;
            }

            return await _iRepo.GetMostRecentSignInByID(id);
        }

        private async Task<SignIn> GetMostRecentByEmail(string email)
        {
            if (!await _iRepo.PersonExist(email))
            {
                return null;
            }

            return await _iRepo.GetMostRecentSignInByEmail(email);
        }
    }
}