using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SpendManagement.Identity.Application.Requests;
using SpendManagement.Identity.Application.Responses;
using SpendManagement.Identity.Data.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SpendManagement.Identity.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public IdentityService(SignInManager<IdentityUser> signInManager,
                               UserManager<IdentityUser> userManager,
                               IOptions<JwtOptions> jwtOptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<UserSignInResponse> SignUp(SignUpUserRequest usuarioCadastro)
        {
            var identityUser = new IdentityUser
            {
                UserName = usuarioCadastro.Email,
                Email = usuarioCadastro.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, usuarioCadastro.Passsword);
            if (result.Succeeded)
                await _userManager.SetLockoutEnabledAsync(identityUser, false);

            var usuarioCadastroResponse = new UserSignInResponse(result.Succeeded);
            if (!result.Succeeded && result.Errors.Any())
                usuarioCadastroResponse.AddError(result.Errors.Select(r => r.Description));

            return usuarioCadastroResponse;
        }

        public async Task<UserLoginResponse> Login(SignInUserRequest usuarioLogin)
        {
            var result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Password, false, true);
            if (result.Succeeded)
                return await GerarCredenciais(usuarioLogin.Email);

            var usuarioLoginResponse = new UserLoginResponse();
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    usuarioLoginResponse.AddError("Essa conta está bloqueada");
                else if (result.IsNotAllowed)
                    usuarioLoginResponse.AddError("Essa conta não tem permissão para fazer login");
                else if (result.RequiresTwoFactor)
                    usuarioLoginResponse.AddError("É necessário confirmar o login no seu segundo fator de autenticação");
                else
                    usuarioLoginResponse.AddError("Usuário ou senha estão incorretos");
            }

            return usuarioLoginResponse;
        }

        public async Task<UserLoginResponse> LoginWithoutPassword(string usuarioId)
        {
            var usuarioLoginResponse = new UserLoginResponse();
            var usuario = await _userManager.FindByIdAsync(usuarioId);

            if (await _userManager.IsLockedOutAsync(usuario))
                usuarioLoginResponse.AddError("Essa conta está bloqueada");
            else if (!await _userManager.IsEmailConfirmedAsync(usuario))
                usuarioLoginResponse.AddError("Essa conta precisa confirmar seu e-mail antes de realizar o login");

            if (usuarioLoginResponse.Success)
                return await GerarCredenciais(usuario.Email);

            return usuarioLoginResponse;
        }

        private async Task<UserLoginResponse> GerarCredenciais(string? email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var accessTokenClaims = await ObterClaims(user, adicionarClaimsUsuario: true);
            var refreshTokenClaims = await ObterClaims(user, adicionarClaimsUsuario: false);

            var dataExpiracaoAccessToken = DateTime.Now.AddSeconds(_jwtOptions.AccessTokenExpiration);
            var dataExpiracaoRefreshToken = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpiration);

            var accessToken = GerarToken(accessTokenClaims, dataExpiracaoAccessToken);
            var refreshToken = GerarToken(refreshTokenClaims, dataExpiracaoRefreshToken);

            return new UserLoginResponse
            (
                accessToken: accessToken,
                refreshToken: refreshToken
            );
        }

        private string GerarToken(IEnumerable<Claim> claims, DateTime dataExpiracao)
        {
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: dataExpiracao,
                signingCredentials: _jwtOptions.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task<IList<Claim>> ObterClaims(IdentityUser user, bool adicionarClaimsUsuario)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString())
            };

            if (adicionarClaimsUsuario)
            {
                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);

                claims.AddRange(userClaims);

                foreach (var role in roles)
                    claims.Add(new Claim("role", role));
            }

            return claims;
        }
    }
}
