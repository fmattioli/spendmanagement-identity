using Microsoft.AspNetCore.Identity;

using SpendManagement.Identity.Application.Requests;
using SpendManagement.Identity.Application.Responses;
using System.Security.Claims;

namespace SpendManagement.Identity.Application.Services
{
    public interface IIdentityService
    {
        Task<UserResponse> SignUp(SignUpUserRequest usuarioCadastro);
        Task<UserLoginResponse> Login(SignInUserRequest usuarioLogin);
        Task<UserResponse> AddUserInClaim(AddUserInClaim userInClaim);
        Task<IEnumerable<Claim>> GetUserClaims(string email);
        Task<UserLoginResponse> LoginWithoutPassword(string usuarioId);
    }
}
