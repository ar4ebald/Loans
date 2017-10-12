using System;
using System.Linq;
using System.Threading.Tasks;
using Loans.DataTransferObjects.Loan;
using Loans.DataTransferObjects.LoanSummary;
using Loans.DataTransferObjects.User;
using Loans.Extensions;
using Loans.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loans.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/loan")]
    public class LoanController : Controller
    {
        private readonly LoansContext _context;

        public LoanController(LoansContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns loan summaries for current user
        /// </summary>
        /// <remarks>
        /// Credits contains all summaries for users 
        /// where their total debt to current user is greater 
        /// than total debt from current
        /// Debts is the opposite of above
        /// </remarks>
        [HttpGet("summary")]
        public async Task<LoanSummaryResponse> GetSummary()
        {
            var userId = User.GetIdentifier();

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

        /// <summary>
        /// Returns all transactions history between current and specified users
        /// </summary>
        /// <param name="id">Target user id</param>
        [HttpGet("user/{id}")]
        public LoanHistoryResponse GetHistory(int id)
        {
            var userId = User.GetIdentifier();

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

        /// <summary>
        /// Creates new credit from specified to current user
        /// </summary>
        /// <param name="creditorId">Creditor identifier</param>
        /// <param name="model">Credit model</param>
        /// <response code="200">Loan creation succeed</response>
        /// <response code="400">Loan creation failed</response>
        [HttpPost("user/{creditorId}")]
        public async Task<IActionResult> CreateLoan(int creditorId, [FromBody]LoanCreateRequest model)
        {
            var debtorId = User.GetIdentifier();

            if (creditorId == debtorId)
            {
                ModelState.AddModelError("Debtor", "You can't create a credit to yourself");
                return BadRequest(ModelState);
            }

            if (await _context.Users.AllAsync(user => user.Id != creditorId))
            {
                ModelState.AddModelError("Creditor", "Invalid creditor");
                return BadRequest(ModelState);
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
