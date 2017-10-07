using System;
using System.ComponentModel.DataAnnotations;

namespace Loans.Models
{
    public class Loan
    {
        public int Id { get; set; }

        [Required]
        public LoanSummary Summary { get; set; }

        public long Amount { get; set; }

        public DateTimeOffset Time { get; set; }

        public string Description { get; set; }
    }
}