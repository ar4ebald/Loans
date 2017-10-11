using System.ComponentModel.DataAnnotations;

namespace Loans.Models
{
    public class CommunityEnrollment
    {
        [Required]
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }

        [Required]
        public Community Community { get; set; }
        public int CommunityId { get; set; }
    }
}