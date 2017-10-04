using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Loans.Models
{
    public class LoanSummary
    {
        [Required]
        public ApplicationUser From { get; set; }
        public int FromId { get; set; }

        [Required]
        public ApplicationUser To { get; set; }
        public int ToId { get; set; }

        /// <summary>
        /// Total debt from <see cref="From"/> to <see cref="To"/>
        /// </summary>
        public long TotalDebt { get; set; }

        public ICollection<Loan> Loans { get; set; }
    }

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