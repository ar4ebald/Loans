using System.ComponentModel.DataAnnotations;

namespace Loans.DataTransferObjects.Account
{
    public class RegisterViewModel
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, RegularExpression("^[a-z][a-z0-9_]{2,15}$")]
        public string UserName { get; set; }

        [Required, StringLength(16, MinimumLength = 1)]
        public string FirstName { get; set; }
        [Required, StringLength(16, MinimumLength = 1)]
        public string LastName { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password), Compare(nameof(Password))]
        public string PasswordConfirm { get; set; }
    }
}
