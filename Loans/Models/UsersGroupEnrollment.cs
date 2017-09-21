using System.ComponentModel.DataAnnotations;

namespace Loans.Models
{
    public class UsersGroupEnrollment
    {
        [Required]
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }

        [Required]
        public UsersGroup Group { get; set; }
        public int GroupId { get; set; }
    }
}