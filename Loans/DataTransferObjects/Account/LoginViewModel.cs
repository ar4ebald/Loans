using System.ComponentModel.DataAnnotations;

namespace Loans.DataTransferObjects.Account
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
