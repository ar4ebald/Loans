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
        public Task<LoanSummaryResponse> GetSummary()
        {
            var userId = User.GetUserId();

            return _context.Users
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
                
            }

            return Ok();
        }
    }
}
