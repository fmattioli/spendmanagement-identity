using System.ComponentModel.DataAnnotations;

namespace SpendManagement.Identity.Application.Requests
{
    public class SignUpUserRequest
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [EmailAddress(ErrorMessage = "O campo {0} é inválido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 6)]
        public string? Passsword { get; set; }

        [Compare(nameof(Passsword), ErrorMessage = "As senhas devem ser iguais")]
        public string? PasswordConfirmation { get; set; }
    }
}
