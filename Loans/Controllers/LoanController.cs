using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loans.DataTransferObjects;
using Loans.Extensions;
using Loans.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loans.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LoanController : Controller
    {
        private readonly LoansDbContext _context;

        public LoanController(LoansDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<LoanSummaryResponse> GetSummary()
        {
            var userId = User.GetUserId();

            var response = await _context.Users
                .Where(user => user.Id == userId)
                .Select(user => new LoanSummaryResponse
                {
                    Creditors = user.Creditors.Select(summary => new LoanSummaryModel
                    {
                        User = new UserModel
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName
                        },
                        Debt = summary.TotalDebt
                    }),
                    Borrowers = user.Borrowers.Select(summary => new LoanSummaryModel
                    {
                        User = new UserModel
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName
                        },
                        Debt = summary.TotalDebt
                    })
                })
                .FirstOrDefaultAsync();

            var creditorsById = response.Creditors.ToDictionary(i => i.User.Id);
            var borrowersById = response.Borrowers.ToDictionary(i => i.User.Id);

            foreach (var pair in borrowersById)
            {
                if (creditorsById.TryGetValue(pair.Key, out var summary))
                {
                    summary.Debt -= pair.Value.Debt;
                }
                else
                {
                    pair.Value.Debt = -pair.Value.Debt;
                    creditorsById.Add(pair.Key, pair.Value);
                }
            }

            return new LoanSummaryResponse
            {
                Creditors = creditorsById.Values.Where(i => i.Debt > 0),

                Borrowers = creditorsById.Values
                    .Where(i => i.Debt < 0)
                    .Select(i =>
                    {
                        i.Debt = -i.Debt;
                        return i;
                    })
            };
        }

        [HttpPost]
        public async Task<IActionResult> CreateLoan([FromBody]LoanCreateRequest model)
        {
            var borrowerId = User.GetUserId();

            if (model.CreditorId == borrowerId)
            {
                return BadRequest("Can't create loan to yourself");
            }

            if (await _context.Users.AllAsync(user => user.Id != model.CreditorId))
            {
                return BadRequest("Invalid creditor");
            }

            LoanSummary summary = await _context.LoanSummaries
                .FirstOrDefaultAsync(i =>
                    i.CreditorId == model.CreditorId && i.BorrowerId == borrowerId
                );

            if (summary == null)
            {
                summary = new LoanSummary
                {
                    BorrowerId = borrowerId,
                    CreditorId = model.CreditorId
                };

                await _context.LoanSummaries.AddAsync(summary);
            }

            summary.TotalDebt += model.Amount;

            _context.Loans.Add(new Loan
            {
                Amount = model.Amount,
                Description = model.Description,
                Summary = summary
            });

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
