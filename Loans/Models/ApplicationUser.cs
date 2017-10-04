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

        public ICollection<UsersGroupEnrollment> Groups { get; set; }

        [InverseProperty("To")]
        public ICollection<LoanSummary> From { get; set; }

        [InverseProperty("From")]
        public ICollection<LoanSummary> To { get; set; }
    }

    public class ApplicationRole : IdentityRole<int>
    {
    }
}