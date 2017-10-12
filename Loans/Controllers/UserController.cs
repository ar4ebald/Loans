using System.Linq;
using System.Threading.Tasks;
using Loans.DataTransferObjects.Requisite;
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
        /// <returns></returns>
        [HttpGet("self")]
        public Task<UserDetailedModel> GetCurrent()
        {
            var userId = User.GetIdentifier();

            return _context.Users
                .Where(user => user.Id == userId)
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
                .SingleAsync();
        }
    }
}