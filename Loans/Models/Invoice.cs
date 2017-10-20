using System;
using System.ComponentModel.DataAnnotations;

namespace Loans.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public ApplicationUser Creditor { get; set; }
        public int CreditorId { get; set; }

        [Required]
        public ApplicationUser Debtor { get; set; }
        public int DebtorId { get; set; }

        public long Amount { get; set; }

        public DateTimeOffset Time { get; set; }

        public string Description { get; set; }

    }
}
