using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loans.DataTransferObjects.Loan;
using Loans.Models;
using Microsoft.EntityFrameworkCore;

namespace Loans.Helpers
{
    public class LoanHelper
    {
        private readonly LoansContext _context;

        public LoanHelper(LoansContext context)
        {
            _context = context;
        }

        public async Task<CreateLoanResult> TryCreateLoan(int debtorId, int creditorId, LoanCreateRequest model)
        {
            if (creditorId == debtorId)
            {
                return new CreateLoanResult("Debtor", "You can't create a credit to yourself");
            }

            if (await _context.Users.AllAsync(user => user.Id != creditorId))
            {
                return new CreateLoanResult("Creditor", "Invalid creditor");
            }

            LoanSummary summary = await _context.LoanSummaries
                .FirstOrDefaultAsync(i =>
                    i.CreditorId == creditorId && i.DebtorId == debtorId
                );

            if (summary == null)
            {
                summary = new LoanSummary
                {
                    CreditorId = creditorId,
                    DebtorId = debtorId
                };

                await _context.LoanSummaries.AddAsync(summary);
            }

            summary.TotalAmount += model.Amount;

            _context.Loans.Add(new Loan
            {
                Summary = summary,
                Amount = model.Amount,
                Time = DateTimeOffset.Now,
                Description = model.Description,
            });

            return new CreateLoanResult();
        }

        public class CreateLoanResult
        {
            public CreateLoanResult()
            {
                Success = true;
            }

            public CreateLoanResult(string error, string errorDescription)
            {
                Error = error;
                ErrorDescription = errorDescription;
            }

            public bool Success { get; }

            public string Error { get; }

            public string ErrorDescription { get; }
        }
    }
}
