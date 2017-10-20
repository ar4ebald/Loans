using System.Collections.Generic;

namespace Loans.DataTransferObjects.Loan
{
    public class InvoiceHistoryResponse
    {
        public IEnumerable<InvoiceModel> Invoices { get; set; }
    }
}