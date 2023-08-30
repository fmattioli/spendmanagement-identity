using System.ComponentModel.DataAnnotations;

namespace SpendManagement.Identity.Application.Requests
{
    public class SignInUserRequest
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [EmailAddress(ErrorMessage = "O campo {0} é inválido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string? Senha { get; set; }
    }
}
