using System;
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

        /// <summary>
        /// Search for users
        /// </summary>
        /// <param name="pattern">Search query</param>
        /// <param name="offset">Amount of users to skip</param>
        /// <param name="count">Amount of users to return</param>
        /// <remarks>
        /// Returns users that contains <paramref name="pattern"/> in their FirstName, LastName or UserName
        /// </remarks>
        [HttpGet("search")]
        public async Task<IActionResult> Search(string pattern, int offset = 0, int count = 20)
        {
            IQueryable<ApplicationUser> users =  _context.Users;

            if (!string.IsNullOrEmpty(pattern))
            {
                users = users.Where(u =>
                    u.UserName.StartsWith(pattern) ||
                    u.FirstName.StartsWith(pattern) ||
                    u.LastName.StartsWith(pattern)
                );
            }

            var response = new UserSearchResponse
            {
                Count = await users.CountAsync(),
                Users = users.OrderBy(user => user.UserName)
                    .Skip(Math.Max(0, offset))
                    .Take(Math.Max(0, Math.Min(100, count)))
                    .Select(UserModel.FromQuery)
            };

            return Ok(response);
        }

        [HttpPost("self/requesite")]
        public async Task<IActionResult> AddRequesite([FromBody]RequisiteModel model)
        {
            var userId = User.GetIdentifier();

            var currentUser = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);

            var newRequesite = new Requisite
            {
                Description = model.Description,
                Owner = currentUser
            };

            _context.Requisites.Add(newRequesite);

            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpDelete("self/requesite/{requesiteId}")]
        public async Task<IActionResult> DeleteRequesite(int requesiteId)
        {
            var userId = User.GetIdentifier();

            var currentRequesite = await _context.Requisites
                .FirstOrDefaultAsync(requesite => requesite.Id == requesiteId && requesite.Owner.Id == userId);

            if (currentRequesite == null)
                return NotFound();
            _context.Requisites.Remove(currentRequesite);

            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}