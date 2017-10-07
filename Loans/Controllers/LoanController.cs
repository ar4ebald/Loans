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
                    Debts = user.Debts.Select(summary => new LoanSummaryModel
                    {
                        User = new UserModel
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName
                        },
                        TotalAmount = summary.TotalAmount
                    }),
                    Credits = user.Credits.Select(summary => new LoanSummaryModel
                    {
                        User = new UserModel
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName
                        },
                        TotalAmount = summary.TotalAmount
                    })
                })
                .FirstOrDefaultAsync();

            var creditsById = response.Credits.ToDictionary(i => i.User.Id);

            foreach (LoanSummaryModel summary in response.Debts)
            {
                if (creditsById.TryGetValue(summary.User.Id, out var oldSummary))
                {
                    oldSummary.TotalAmount -= summary.TotalAmount;
                }
                else
                {
                    summary.TotalAmount = -summary.TotalAmount;
                    creditsById.Add(summary.User.Id, summary);
                }
            }

            return new LoanSummaryResponse
            {
                Credits = creditsById.Values.Where(i => i.TotalAmount > 0),

                Debts = creditsById.Values
                    .Where(i => i.TotalAmount < 0)
                    .Select(i =>
                    {
                        i.TotalAmount = -i.TotalAmount;
                        return i;
                    })
            };
        }

        [HttpGet("user/{id}")]
        public LoanHistoryResponse GetHistory(int id)
        {
            var userId = User.GetUserId();

            return new LoanHistoryResponse
            {
                Credits = _context.Loans
                    .Where(loan => loan.Summary.CreditorId == userId && loan.Summary.DebtorId == id)
                    .Select(LoanModel.Select),

                Debts = _context.Loans
                .Where(loan => loan.Summary.DebtorId == userId && loan.Summary.CreditorId == id)
                .Select(LoanModel.Select)
            };
        }

        [HttpPost("user/{creditorId}")]
        public async Task<IActionResult> CreateLoan(int creditorId, [FromBody]LoanCreateRequest model)
        {
            var debtorId = User.GetUserId();

            if (creditorId == debtorId)
            {
                return BadRequest("Can't owe to yourself");
            }

            if (await _context.Users.AllAsync(user => user.Id != creditorId))
            {
                return BadRequest("Invalid creditor");
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

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
