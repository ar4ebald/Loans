using System;
using System.Linq.Expressions;

namespace Loans.DataTransferObjects.Loan
{
    public class LoanModel
    {
        public static readonly Expression<Func<Models.Loan, LoanModel>> Select = loan => new LoanModel
        {
            Id = loan.Id,
            Time = loan.Time,
            Description = loan.Description,
            Amount = loan.Amount
        };

        public int Id { get; set; }

        public long Amount { get; set; }

        public DateTimeOffset Time { get; set; }

        public string Description { get; set; }
    }
}
