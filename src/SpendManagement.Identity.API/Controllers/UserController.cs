using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using SpendManagement.Identity.Application.Requests;
using SpendManagement.Identity.Application.Responses;
using SpendManagement.Identity.Application.Services;
using System.Security.Claims;

namespace SpendManagement.Identity.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly Tracer _tracer;
        public UserController(IIdentityService identityService, Tracer tracer)
        {
            _identityService = identityService;
            _tracer = tracer;
        }

        /// <summary>
        /// SignUp Users
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="signUp">Datas from signed users</param>
        /// <returns></returns>
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

            using var span = _tracer.StartActiveSpan("SayHello");
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
        /// <remarks>
        /// </remarks>
        /// <param name="login">User's login data</param>
        /// <returns></returns>
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

        [HttpPost]
        [Route("addUserInClaim", Name = nameof(AddUserInClaim))]
        [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserLoginResponse>> AddUserInClaim([FromBody] AddUserInClaim userClaim)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _identityService.AddUserInClaim(userClaim);
            return Ok(await _identityService.GetUserClaims(user));
        }

        /// <summary>
        /// User login by refresh token.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">User logged with sucessfully</response>
        /// <response code="400">Validation errors</response>
        /// <response code="401">Authentication error</response>
        /// <response code="500">Internal error</response>
        [HttpPost]
        [Route("refreshLogin", Name = nameof(RefreshLogin))]
        [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize]
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
