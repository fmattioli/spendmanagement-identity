using System.ComponentModel.DataAnnotations;

namespace SpendManagement.Identity.Application.Requests
{
    public class SignUpUserRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "The field {0} is mandatory")]
        public string? Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string? Password { get; set; }

        [Compare(nameof(Password))]
        public string? PasswordConfirmation { get; set; }
    }
}
