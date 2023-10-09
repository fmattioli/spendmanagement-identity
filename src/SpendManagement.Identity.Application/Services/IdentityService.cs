using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Serilog;
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
        private readonly ILogger _logger;

        public IdentityService(SignInManager<IdentityUser> signInManager,
                               UserManager<IdentityUser> userManager,
                               IOptions<JwtOptions> jwtOptions,
                               ILogger logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
        }

        public async Task<UserResponse> SignUp(SignUpUserRequest usuarioCadastro)
        {
            var identityUser = new IdentityUser
            {
                UserName = usuarioCadastro.Email,
                Email = usuarioCadastro.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, usuarioCadastro.Password);

            if (result.Succeeded)
                await _userManager.SetLockoutEnabledAsync(identityUser, false);

            var userSignedResponse = new UserResponse(result.Succeeded);

            if (!result.Succeeded && result.Errors.Any())
                userSignedResponse.AddError(result.Errors.Select(r => r.Description));

            _logger.Information("User created with successfully", usuarioCadastro.Email);

            return userSignedResponse;
        }

        public async Task<UserLoginResponse> Login(SignInUserRequest usuarioLogin)
        {
            var result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Password, false, true);

            if (result.Succeeded)
                return await GenerateCredentials(usuarioLogin.Email);

            var usuarioLoginResponse = new UserLoginResponse();

            if (!result.Succeeded)
            {
                usuarioLoginResponse = result switch
                {
                    _ when result.IsLockedOut.Equals(true) => usuarioLoginResponse.AddErrors("This account is locked."),
                    _ when result.IsNotAllowed.Equals(true) => usuarioLoginResponse.AddErrors("This account is locked."),
                    _ when result.RequiresTwoFactor.Equals(true) => usuarioLoginResponse.AddErrors("This account is locked."),
                    _ => usuarioLoginResponse.AddErrors("User or password incorrect"),
                };
            }

            return usuarioLoginResponse;
        }

        public async Task<IEnumerable<Claim>> GetUserClaims(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is not null)
                return await _userManager.GetClaimsAsync(user);

            return Enumerable.Empty<Claim>();
        }

        public async Task<UserResponse> AddUserInClaim(AddUserInClaim userInClaim)
        {
            var user = await _userManager.FindByEmailAsync(userInClaim.Email);

            if (user != null && userInClaim.Claims?.Any() == true)
            {
                var claimsToAdd = userInClaim.Claims
                    .Select(claim => new Claim(claim.ClaimType.ToString(), claim.ClaimValue.ToString()))
                    .ToList();

                if (claimsToAdd.Any())
                {
                    var identityResult = await _userManager.AddClaimsAsync(user, claimsToAdd);

                    if (identityResult.Succeeded)
                    {
                        _logger.Information("User added in claim with success", userInClaim.Email);
                        return new UserResponse(true);
                    }
                }
            }

            _logger.Information("Failed to add user in claim", userInClaim.Email);
            return new UserResponse(false);
        }

        public async Task<UserLoginResponse> LoginWithoutPassword(string usuarioId)
        {
            var usuarioLoginResponse = new UserLoginResponse();
            var usuario = await _userManager.FindByIdAsync(usuarioId);

            return _userManager switch
            {
                _ when await _userManager.IsLockedOutAsync(usuario) => usuarioLoginResponse.AddErrors("This account is locked."),
                _ when !await _userManager.IsEmailConfirmedAsync(usuario) => usuarioLoginResponse.AddErrors("This account is locked."),
                _ => await GenerateCredentials(usuario.Email)
            };
        }

        private async Task<UserLoginResponse> GenerateCredentials(string? email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var accessTokenClaims = await GetClaims(user, true);
            var refreshTokenClaims = await GetClaims(user, false);

            var dataExpiracaoAccessToken = DateTime.Now.AddSeconds(_jwtOptions.AccessTokenExpiration);
            var dataExpiracaoRefreshToken = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpiration);

            var accessToken = GerarToken(accessTokenClaims, dataExpiracaoAccessToken);
            var refreshToken = GerarToken(refreshTokenClaims, dataExpiracaoRefreshToken);

            _logger.Information("User logged with successfully", email);

            return new UserLoginResponse
            (
                accessToken: accessToken,
                refreshToken: refreshToken
            );
        }

        private string GerarToken(IEnumerable<System.Security.Claims.Claim> claims, DateTime dataExpiracao)
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

        private async Task<IList<Claim>> GetClaims(IdentityUser user, bool adicionarClaimsUsuario)
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
