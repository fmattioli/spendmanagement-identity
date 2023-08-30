using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendManagement.Identity.Application.Requests;
using SpendManagement.Identity.Application.Responses;
using SpendManagement.Identity.Application.Services.Interfaces;
using System.Security.Claims;

namespace SpendManagement.Identity.API.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public UserController(IIdentityService identityService) => _identityService = identityService;

        /// <summary>
        /// Cadastro de usuário.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="signUp">Dados de cadastro do usuário</param>
        /// <returns></returns>
        /// <response code="200">Usuário criado com sucesso</response>
        /// <response code="400">Retorna erros de validação</response>
        /// <response code="500">Retorna erros caso ocorram</response>
        [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost("cadastro")]
        public async Task<IActionResult> SignUp(SignUpUserRequest signUp)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var resultado = await _identityService.CadastrarUsuario(signUp);

            if (resultado.Sucesso)
            {
                return Created("/cadastro", resultado);
            }

            return BadRequest(resultado.Erros);
        }

        /// <summary>
        /// Login do usuário via usuário/senha.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="login">Dados de login do usuário</param>
        /// <returns></returns>
        /// <response code="200">Login realizado com sucesso</response>
        /// <response code="400">Retorna erros de validação</response>
        /// <response code="401">Erro caso usuário não esteja autorizado</response>
        /// <response code="500">Retorna erros caso ocorram</response>
        [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponse>> Login([FromBody] SignInUserRequest login)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var resultado = await _identityService.Login(login);

            if (resultado.Sucesso)
                return Ok(resultado);

            return Unauthorized();
        }

        /// <summary>
        /// Login do usuário via refresh token.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Login realizado com sucesso</response>
        /// <response code="400">Retorna erros de validação</response>
        /// <response code="401">Erro caso usuário não esteja autorizado</response>
        /// <response code="500">Retorna erros caso ocorram</response>
        [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpPost("refresh-login")]
        public async Task<ActionResult<UserLoginResponse>> RefreshLogin()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var usuarioId = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (usuarioId == null)
                return BadRequest();

            var resultado = await _identityService.LoginSemSenha(usuarioId);
            if (resultado.Sucesso)
                return Ok(resultado);

            return Unauthorized();
        }
    }
}
