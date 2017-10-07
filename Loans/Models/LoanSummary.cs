using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Loans.Models
{
    public class LoanSummary
    {
        [Required]
        public ApplicationUser Creditor { get; set; }
        public int CreditorId { get; set; }

        [Required]
        public ApplicationUser Debtor { get; set; }
        public int DebtorId { get; set; }

        public long TotalAmount { get; set; }

        public ICollection<Loan> Loans { get; set; }
    }
}