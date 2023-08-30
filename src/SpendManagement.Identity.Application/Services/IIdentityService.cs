using SpendManagement.Identity.Application.Requests;
using SpendManagement.Identity.Application.Responses;

namespace SpendManagement.Identity.Application.Services
{
    public interface IIdentityService
    {
        Task<UserSignInResponse> SignUp(SignUpUserRequest usuarioCadastro);
        Task<UserLoginResponse> Login(SignInUserRequest usuarioLogin);
        Task<UserLoginResponse> LoginWithoutPassword(string usuarioId);
    }
}
