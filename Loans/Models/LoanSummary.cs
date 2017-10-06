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
        public ApplicationUser Borrower { get; set; }
        public int BorrowerId { get; set; }

        /// <summary>
        /// Total debt from <see cref="Creditor"/> to <see cref="Borrower"/>
        /// </summary>
        public long TotalDebt { get; set; }

        public ICollection<Loan> Loans { get; set; }
    }
}