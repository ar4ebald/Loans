using System.Collections.Generic;

namespace Loans.DataTransferObjects.LoanSummary
{
    public class LoanSummaryResponse
    {
        public IEnumerable<LoanSummaryModel> Credits { get; set; }

        public IEnumerable<LoanSummaryModel> Debts { get; set; }
    }
}