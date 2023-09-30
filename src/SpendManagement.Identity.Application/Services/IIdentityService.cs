using Microsoft.AspNetCore.Identity;

using SpendManagement.Identity.Application.Requests;
using SpendManagement.Identity.Application.Responses;
using System.Security.Claims;

namespace SpendManagement.Identity.Application.Services
{
    public interface IIdentityService
    {
        Task<UserSignedInResponse> SignUp(SignUpUserRequest usuarioCadastro);
        Task<UserLoginResponse> Login(SignInUserRequest usuarioLogin);
        Task<IdentityUser?> AddUserInClaim(AddUserInClaim userInClaim);
        Task<IEnumerable<Claim>> GetUserClaims(IdentityUser? user);
        Task<UserLoginResponse> LoginWithoutPassword(string usuarioId);
    }
}
