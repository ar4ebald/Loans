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
            UserModel userModel = await _context.Users
                .Where(user => user.Id == id)
                .Select(UserModel.FromQuery)
                .FirstOrDefaultAsync();

            if (userModel == null)
            {
                ModelState.AddModelError("User", "Invalid user id");
                return NotFound(ModelState);
            }

            var response = new UserDetailedModel
            {
                Info = userModel,
                Requisites = _context.Requisites
                    .Where(requisite => requisite.Owner.Id == userModel.Id)
                    .Select(RequisiteModel.FromQuery)
            };

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
            IQueryable<ApplicationUser> users = _context.Users;

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


        /// <summary>
        /// Add requisite to current user
        /// </summary>
        /// <param name="model">New requisite model</param>
        [HttpPost("self/requisite")]
        public async Task AddRequisite([FromBody]CreateRequisiteRequest model)
        {
            await _context.Requisites.AddAsync(new Requisite
            {
                Description = model.Description,
                OwnerId = User.GetIdentifier(),
            });

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update requisite of current user
        /// </summary>
        /// <param name="id">Requisite identifier</param>
        /// <param name="model">Requisite model</param>
        /// <response code="200">Successfully updated</response>
        /// <response code="404">Update failed</response>
        [HttpPut("self/requisite/{id}")]
        public async Task<IActionResult> UpdateRequisite(int id, [FromBody] CreateRequisiteRequest model)
        {
            var requisite = await _context.Requisites.FindAsync(id);

            if (requisite == null || requisite.OwnerId != User.GetIdentifier())
            {
                ModelState.AddModelError("Update", "Specified requisite does not exist");
                return NotFound(new SerializableError(ModelState));
            }

            requisite.Description = model.Description;

            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Delete requisite by id for current user
        /// </summary>
        /// <param name="id">Requisite identifier</param>
        /// <response code="200">Successfully deleted</response>
        /// <response code="404">Deletion failed</response>
        [HttpDelete("self/requisite/{id}")]
        public async Task<IActionResult> DeleteRequisite(int id)
        {
            _context.Requisites.Remove(new Requisite
            {
                Id = id,
                OwnerId = User.GetIdentifier()
            });

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("Delete", "Specified requisite does not exist");
                return NotFound(new SerializableError(ModelState));
            }

            return Ok();
        }

    }
}