using SpendManagement.Identity.Application.Requests;
using SpendManagement.Identity.Application.Responses;

namespace SpendManagement.Identity.Application.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<UserSignInResponse> CadastrarUsuario(SignUpUserRequest usuarioCadastro);
        Task<UserLoginResponse> Login(SignInUserRequest usuarioLogin);
        Task<UserLoginResponse> LoginSemSenha(string usuarioId);
    }
}
