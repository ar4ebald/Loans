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
    public class InvoiceController:Controller
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
        public async Task<IActionResult> CreateInvoice(int debtorId, [FromBody]InvoiceModel model)
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
        /// <param name="id">Target user id</param>
        [HttpGet("user/{id}")]
        public InvoiceHistoryResponse GetHistory(int id)
        {
            var userId = User.GetIdentifier();

            return new InvoiceHistoryResponse
            {
                Invoices = _context.Invoices
                    .Where(invoice => invoice.DebtorId == userId && invoice.CreditorId == id)
                    .Select(InvoiceModel.Select)
            };
        }
    }
}
