using System.ComponentModel.DataAnnotations;

namespace Loans.Models
{
    public class Requisite
    {
        public int Id { get; set; }

        [Required]
        public ApplicationUser Owner { get; set; }

        public int OwnerId { get; set; }

        public string Description { get; set; }
    }
}