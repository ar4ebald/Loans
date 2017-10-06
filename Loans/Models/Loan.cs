using System.ComponentModel.DataAnnotations;

namespace Loans.Models
{
    public class Loan
    {
        public int Id { get; set; }

        [Required]
        public LoanSummary Summary { get; set; }
        public int SummaryId { get; set; }

        public string Description { get; set; }

        public long Amount { get; set; }
    }
}