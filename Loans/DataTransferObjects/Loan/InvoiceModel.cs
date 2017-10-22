using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Loans.DataTransferObjects.Loan
{
    public class InvoiceModel
    {
        public static readonly Expression<Func<Models.Invoice, InvoiceModel>> Select = loan => new InvoiceModel
        {
            Id = loan.Id,
            Time = loan.Time,
            Description = loan.Description,
            Amount = loan.Amount,
            DebtorId = loan.DebtorId,
            CreditorId = loan.CreditorId
        };

        public int Id { get; set; }

        public int CreditorId { get; set; }

        public int DebtorId { get; set; }

        public long Amount { get; set; }

        public DateTimeOffset Time { get; set; }

        public string Description { get; set; }
    }
}
