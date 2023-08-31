using SpendManagement.Identity.Application.Requests;
using SpendManagement.Identity.Application.Responses;
using System.Security.Claims;

namespace SpendManagement.Identity.Application.Services
{
    public interface IIdentityService
    {
        Task<UserSignedInResponse> SignUp(SignUpUserRequest usuarioCadastro);
        Task<UserLoginResponse> Login(SignInUserRequest usuarioLogin);
        Task<IList<Claim>> AddUserInClaim(AddUserInClaim userInClaim);
        Task<UserLoginResponse> LoginWithoutPassword(string usuarioId);
    }
}
