using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loans.DataTransferObjects.Loan;
using Loans.DataTransferObjects.LoanSummary;
using Loans.DataTransferObjects.User;
using Loans.Extensions;
using Loans.Helpers;
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

            IQueryable<LoanSummary> summaries = _context.LoanSummaries
                .Where(summary => summary.DebtorId == userId || summary.CreditorId == userId);

            var totals = new Dictionary<int, long>();

            await summaries.ForEachAsync(summary =>
            {
                var (otherId, amountToAdd) = summary.CreditorId == userId
                    ? (summary.DebtorId, +summary.TotalAmount)
                    : (summary.CreditorId, -summary.TotalAmount);

                totals.TryGetValue(otherId, out long value);
                totals[otherId] = value + amountToAdd;
            });

            var debtors = _context.LoanSummaries
                .Where(summary => summary.CreditorId == userId)
                .Select(summary => summary.Debtor);

            var creditors = _context.LoanSummaries
                .Where(summary => summary.DebtorId == userId)
                .Select(summary => summary.Creditor);

            Dictionary<int, UserModel> usersById = await debtors
                .Concat(creditors)
                .Distinct()
                .Select(UserModel.FromQuery)
                .ToDictionaryAsync(user => user.Id);

            return new LoanSummaryResponse
            {
                Credits = totals
                    .Where(pair => pair.Value > 0)
                    .Select(pair => new LoanSummaryModel
                    {
                        User = usersById[pair.Key],
                        TotalAmount = pair.Value
                    }),

                Debts = totals
                    .Where(pair => pair.Value < 0)
                    .Select(pair => new LoanSummaryModel
                    {
                        User = usersById[pair.Key],
                        TotalAmount = -pair.Value
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
                    .OrderByDescending(loan => loan.Time)
                    .Select(LoanModel.Select),

                Debts = _context.Loans
                    .Where(loan => loan.Summary.DebtorId == userId && loan.Summary.CreditorId == id)
                    .OrderByDescending(loan => loan.Time)
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
        public async Task<IActionResult> CreateLoan(
            [FromServices] LoanHelper helper,
            int creditorId,
            [FromBody] LoanCreateRequest model)
        {
            var result = await helper.TryCreateLoan(User.GetIdentifier(), creditorId, model);

            if (result.Success)
            {
                await _context.SaveChangesAsync();
                return Ok();
            }

            ModelState.AddModelError(result.Error, result.ErrorDescription);
            return BadRequest(ModelState);
        }
    }
}