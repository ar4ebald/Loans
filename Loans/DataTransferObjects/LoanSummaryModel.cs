using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Loans.DataTransferObjects
{
    public class UserModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class LoanSummaryModel
    {
        public UserModel User { get; set; }

        public long Debt { get; set; }
    }

    public class LoanSummaryResponse
    {
        public IEnumerable<LoanSummaryModel> Creditors { get; set; }

        public IEnumerable<LoanSummaryModel> Borrowers { get; set; }
    }
}
