using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loans.DataTransferObjects.Loan;
using Loans.Extensions;
using Loans.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loans.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/invoice")]
    public class InvoiceController : Controller
    {
        private readonly LoansContext _context;

        public InvoiceController(LoansContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Creates new invoice from current to specified user
        /// </summary>
        /// <param name="debtorId">Debtor identifier</param>
        /// <param name="model">Invoice model</param>
        /// <response code="200">Invoice creation succeed</response>
        /// <response code="400">Invoice creation failed</response>
        [HttpPost("user/{debtorId}")]
        public async Task<IActionResult> CreateInvoice(int debtorId, [FromBody]InvoiceCreateRequest model)
        {
            var creditorId = User.GetIdentifier();

            if (debtorId == creditorId)
            {
                ModelState.AddModelError("Creditor", "You can't create a credit to yourself");
                return BadRequest(ModelState);
            }

            if (await _context.Users.AllAsync(user => user.Id != debtorId))
            {
                ModelState.AddModelError("Debtor", "Invalid debtor");
                return BadRequest(ModelState);
            }

            _context.Invoices.Add(new Invoice
            {
                Amount = model.Amount,
                Time = DateTimeOffset.Now,
                Description = model.Description,
                DebtorId = debtorId,
                CreditorId = creditorId
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Returns all invoice history between current and specified users
        /// </summary>
        [HttpGet("user")]
        public InvoiceHistoryResponse GetHistory()
        {
            var userId = User.GetIdentifier();

            return new InvoiceHistoryResponse
            {
                Invoices = _context.Invoices
                    .Where(invoice => invoice.DebtorId == userId)
                    .Select(InvoiceModel.Select)
            };
        }

        /// <summary>
        /// Method to accept invoice by current user by id
        /// </summary>
        /// <param name="id">invoice identifier</param>
        /// <response code="200">Invoice acception succeed</response>
        /// <response code="400">Invoice acception failed</response>
        [HttpPost("accept/{id}")]
        public async Task<IActionResult> AcceptInvoice(int id)
        {
            var userId = User.GetIdentifier();

            var currentInvoice = await _context.Invoices.SingleAsync(invoice => invoice.Id == id);

            _context.Invoices.Remove(currentInvoice);

            var targetId = currentInvoice.CreditorId;

            await CreateLoan(targetId, new LoanCreateRequest
            {
                Amount = currentInvoice.Amount,
                Description = currentInvoice.Description,
                Time = currentInvoice.Time
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task CreateLoan(int creditorId, LoanCreateRequest model)
        {
            var debtorId = User.GetIdentifier();

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
        }

        /// <summary>
        /// Method to decline invoice by current user by id
        /// </summary>
        /// <param name="id">invoice identifier</param>
        /// <response code="200">Invoice declineing succeed</response>
        /// <response code="400">Invoice declineing failed</response>
        [HttpPost("decline/{id}")]
        public async Task<IActionResult> DeclineInvoice(int id)
        {
            var userId = User.GetIdentifier();

            var currentInvoice = await _context.Invoices.SingleAsync(invoice => invoice.Id == id);

            _context.Invoices.Remove(currentInvoice);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
