using System.ComponentModel.DataAnnotations;

namespace Loans.Models
{
    class Requisite
    {
        public int Id { get; set; }

        [Required]
        public ApplicationUser Owner { get; set; }

        public string Description { get; set; }
    }
}