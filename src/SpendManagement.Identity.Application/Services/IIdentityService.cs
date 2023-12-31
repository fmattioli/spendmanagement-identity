using SpendManagement.Identity.Application.Requests;
using SpendManagement.Identity.Application.Responses;
using System.Security.Claims;

namespace SpendManagement.Identity.Application.Services
{
    public interface IIdentityService
    {
        Task<UserResponse> SignUpAsync(SignUpUserRequest usuarioCadastro);
        Task<UserLoginResponse> LoginAsync(SignInUserRequest usuarioLogin);
        Task<UserResponse> AddUserInClaimAsync(AddUserInClaimRequest userInClaim);
        Task<IEnumerable<Claim>> GetUserClaimsAsync(string email);
        Task<UserLoginResponse> LoginWithoutPasswordAsync(string usuarioId);
    }
}
