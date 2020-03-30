using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using tcs_service.Exceptions;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Models.DTOs;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Controllers {

    /// <summary>Users API</summary>
    [ApiController]
    [Authorize]
    [Route ("api/[controller]")]
    public class UsersController : ControllerBase {
        readonly private IMapper _mapper;
        readonly private IUserRepo _userRepo;
        private readonly AppSettings _appSettings;

        /// <summary>Users Controller Constructor</summary>
        public UsersController (
            IUserRepo userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings) {
            _userRepo = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        /// <summary>Authenticate a user to the system</summary>
        /// <returns>User JWT Token</returns>
        /// <response code="200">User has been authenticated</response>
        /// <response code="400">Username or password incorrect</response>
        [AllowAnonymous]
        [HttpPost ("authenticate")]
        [ProducesResponseType (typeof (AuthDTO), 200)]
        [ProducesResponseType (typeof (ErrorMessage), 400)]
        public async Task<IActionResult> Authenticate ([FromBody] UserDto userParam) {
            var user = await _userRepo.Authenticate (userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest (new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler ();
            var key = Encoding.ASCII.GetBytes (_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (new Claim[] {
                new Claim (ClaimTypes.Name, user.Id.ToString ())
                }),
                Expires = DateTime.UtcNow.AddDays (7),
                SigningCredentials = new SigningCredentials (new SymmetricSecurityKey (key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken (tokenDescriptor);
            var tokenString = tokenHandler.WriteToken (token);

            // return basic user info (without password) and token to store client side
            return Ok (new AuthDTO {
                Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = tokenString
            });
        }

        /// <summary>Register a new User</summary>
        /// <response code="201">User has been created</response>
        /// <response code="400">User failed to be created</response>
        [HttpPost ("register")]
        [ProducesResponseType (typeof (User), 200)]
        [ProducesResponseType (typeof (ErrorMessage), 400)]
        public async Task<IActionResult> Register ([FromBody] UserDto userDto) {
            // map dto to entity
            var user = _mapper.Map<User> (userDto);

            try {
                // save
                var res = await _userRepo.Create (user, userDto.Password);
                return Created ($"api/Users/${res.Id}", res);
            } catch (AppException ex) {
                // return error message if there was an exception
                return BadRequest (new { message = ex.Message });
            }
        }

        /// <summary>Get all users</summary>
        /// <response code="200">Returns list of users</response>
        [HttpGet]
        [ProducesResponseType (typeof (IEnumerable<UserDto>), 200)]
        public IActionResult GetAll () {
            var users = _userRepo.GetAll ();
            var userDtos = _mapper.Map<IList<UserDto>> (users);
            return Ok (userDtos);
        }

        /// <summary>Get a user by with the associated id</summary>
        /// <response code="200">Returns user</response>
        [HttpGet ("{id}")]
        [ProducesResponseType (typeof (UserDto), 200)]
        public async Task<IActionResult> GetById (int id) {
            var user = await _userRepo.Find (x => x.Id == id);
            var userDto = _mapper.Map<UserDto> (user);
            return Ok (userDto);
        }

        /// <summary>Update a user with the associated id</summary>
        /// <response code="200">Returns updated user</response>
        /// <response code="400">Failed to update user</response>
        [HttpPut ("{id}")]
        [ProducesResponseType (typeof (UserDto), 200)]
        [ProducesResponseType (typeof (ErrorMessage), 400)]
        public async Task<IActionResult> Update (int id, [FromBody] UserDto userDto) {
            // map dto to entity and set id
            var user = _mapper.Map<User> (userDto);
            user.Id = id;

            try {
                // save
                var updatedUser = await _userRepo.Update (user, userDto.Password);
                return Ok (updatedUser);
            } catch (AppException ex) {
                // return error message if there was an exception
                return BadRequest (new { message = ex.Message });
            }
        }

        /// <summary>Delete a user with associated id</summary>
        /// <response code="200">Returns 200 if successful</response>
        [HttpDelete ("{id}")]
        [ProducesResponseType (200)]
        public async Task<IActionResult> Delete (int id) {
            await _userRepo.Remove (x => x.Id == id);
            return Ok ();
        }

        /// <summary>Returns true if the token of the Auth header is authenticated</summary>
        /// <response code="200">Returns {authenticated=true}</response>
        /// <response code="401">Not Authenticated</response>
        [Authorize]
        [HttpGet ("IsAuthenticate")]
        public IActionResult IsAuthenticated () {
            return Ok (new { authenticated = true });
        }
    }
}