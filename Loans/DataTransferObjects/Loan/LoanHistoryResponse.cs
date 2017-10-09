using System.Collections.Generic;

namespace Loans.DataTransferObjects.Loan
{
    public class LoanHistoryResponse
    {
        public IEnumerable<LoanModel> Credits { get; set; }

        public IEnumerable<LoanModel> Debts { get; set; }
    }
}