using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Loans.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Requisite> Requisites { get; set; }

        public ICollection<CommunityEnrollment> CommunitiesEnrollments { get; set; }

        [InverseProperty("Debtor")]
        public ICollection<LoanSummary> Debts { get; set; }

        [InverseProperty("Creditor")]
        public ICollection<LoanSummary> Credits { get; set; }
    }

    public class ApplicationRole : IdentityRole<int>
    {
    }
}