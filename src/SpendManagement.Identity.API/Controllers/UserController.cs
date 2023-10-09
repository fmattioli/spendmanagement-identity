using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendManagement.Identity.Application.Requests;
using SpendManagement.Identity.Application.Responses;
using SpendManagement.Identity.Application.Services;
using System.Security.Claims;

namespace SpendManagement.Identity.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IIdentityService _identityService;

        public UserController(IIdentityService identityService) => _identityService = identityService;

        /// <summary>
        /// SignUp Users
        /// </summary>
        /// <param name="signUp">Datas from signed users</param>
        /// <returns>Returns a created user</returns>
        /// <response code="200">User created with successfully</response>
        /// <response code="400">Validation errors</response>
        /// <response code="500">Internal errors</response>
        [HttpPost]
        [Route("signUp", Name = nameof(SignUp))]
        [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SignUp(SignUpUserRequest signUp)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var userSignedIn = await _identityService.SignUp(signUp);

            if (userSignedIn.Success)
            {
                return Created("/signUp", userSignedIn);
            }

            return BadRequest(userSignedIn.Errors);
        }

        /// <summary>
        /// User's login by e-mail and password
        /// </summary>
        /// <param name="login">User's login data</param>
        /// <returns>Returns a new JWT</returns>
        /// <response code="200">User logged with sucessfully</response>
        /// <response code="400">Validation errors</response>
        /// <response code="401">Authentication error</response>
        /// <response code="500">Internal error</response>
        [HttpPost]
        [Route("login", Name = nameof(Login))]
        [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserLoginResponse>> Login([FromBody] SignInUserRequest login)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var resultado = await _identityService.Login(login);

            if (resultado.Success)
                return Ok(resultado);

            return Unauthorized();
        }

        /// <summary>
        /// Add user in claim
        /// </summary>
        /// <param name="userClaim">Claim informations</param>
        /// <returns>Return users in claims</returns>
        /// <response code="201">User logged with sucessfully</response>
        /// <response code="400">Validation errors</response>
        /// <response code="401">Authentication error</response>
        /// <response code="500">Internal error</response>
        [HttpPost]
        [Route("addUserInClaim", Name = nameof(AddUserInClaim))]
        [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult<UserLoginResponse>> AddUserInClaim([FromBody] AddUserInClaim userClaim)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _identityService.AddUserInClaim(userClaim);
            var claims = await _identityService.GetUserClaims(user);
            return Created("addUserInClaim", claims);
        }

        /// <summary>
        /// User login by refresh token.
        /// </summary>
        /// <returns>Returns a new JWT refreshed</returns>
        /// <response code="200">User logged with sucessfully</response>
        /// <response code="400">Validation errors</response>
        /// <response code="401">Authentication error</response>
        /// <response code="500">Internal error</response>
        [ProducesResponseType(typeof(UserSignedInResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpPost("refresh-login")]
        public async Task<ActionResult<UserLoginResponse>> RefreshLogin()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var usuarioId = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (usuarioId is null)
                return BadRequest();

            var result = await _identityService.LoginWithoutPassword(usuarioId);
            if (result.Success)
                return Ok(result);

            return Unauthorized();
        }
    }
}
