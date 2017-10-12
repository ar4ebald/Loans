using System.Linq;
using System.Threading.Tasks;
using Loans.DataTransferObjects.Requisite;
using Loans.DataTransferObjects.User;
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
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly LoansContext _context;

        public UserController(LoansContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns current user detailed information
        /// </summary>
        /// <response code="200"></response>
        /// <response code="404"></response>
        [ProducesResponseType(typeof(UserDetailedModel), StatusCodes.Status200OK)]
        [HttpGet("self")]
        public Task<IActionResult> GetCurrent()
        {
            return Get(User.GetIdentifier());
        }

        /// <summary>
        /// Returns user detailed information
        /// </summary>
        /// <response code="200"></response>
        /// <response code="404"></response>
        [ProducesResponseType(typeof(UserDetailedModel), StatusCodes.Status200OK)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            UserDetailedModel response = await _context.Users
                .Where(user => user.Id == id)
                .Select(user => new UserDetailedModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Requisites = user.Requisites.Select(requesite => new RequisiteModel
                    {
                        Id = requesite.Id,
                        Description = requesite.Description
                    })
                })
                .FirstOrDefaultAsync();

            if (response == null)
            {
                ModelState.AddModelError("User", "Invalid user id");
                return NotFound(ModelState);
            }

            return Ok(response);
        }
    }
}