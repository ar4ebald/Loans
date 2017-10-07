using System.Collections.Generic;

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

        public long TotalAmount { get; set; }
    }

    public class LoanSummaryResponse
    {
        public IEnumerable<LoanSummaryModel> Credits { get; set; }

        public IEnumerable<LoanSummaryModel> Debts { get; set; }
    }
}
