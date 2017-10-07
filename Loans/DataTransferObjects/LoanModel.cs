using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Loans.Models;

namespace Loans.DataTransferObjects
{
    public class LoanModel
    {
        public static readonly Expression<Func<Loan, LoanModel>> Select = loan => new LoanModel
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

    public class LoanHistoryResponse
    {
        public IEnumerable<LoanModel> Credits { get; set; }

        public IEnumerable<LoanModel> Debts { get; set; }
    }
}
