using System.ComponentModel.DataAnnotations;

namespace Loans.Models
{
    public class Loan
    {
        [Required]
        public ApplicationUser Borrower { get; set; }
        public int BorrowerId { get; set; }

        [Required]
        public ApplicationUser Debtor { get; set; }
        public int DebtorId { get; set; }

        public string Description { get; set; }

        public long Amount { get; set; }
    }
}