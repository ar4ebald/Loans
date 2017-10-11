using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loans.DataTransferObjects;
using Loans.DataTransferObjects.Community;
using Loans.DataTransferObjects.UsersGroup;
using Loans.Extensions;
using Loans.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loans.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CommunityController : Controller
    {
        private readonly LoansContext _context;

        public CommunityController(LoansContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateCommunityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var enrollment = new CommunityEnrollment { UserId = User.GetIdentifier() };
                var community = new Community
                {
                    Name = model.Name,
                    Members = new[] { enrollment }
                };

                await _context.Communities.AddAsync(community);
                await _context.CommunitiesEnrollments.AddAsync(enrollment);
                await _context.SaveChangesAsync();

                return Ok(ValidationResultModel.Success);
            }

            return BadRequest(new ValidationResultModel(ModelState));
        }

        [HttpGet]
        public Task<IEnumerable<CommunityViewModel>> GetPersonalCommunities()
        {
            var userId = User.GetIdentifier();

            return _context.Users
                .Where(user => user.Id == userId)
                .Select(user => user.CommunitiesEnrollments.Select(enrollment => new CommunityViewModel
                {
                    Id = enrollment.CommunityId,
                    Name = enrollment.Community.Name
                }))
                .FirstOrDefaultAsync();
        }

        //[HttpPost("join/{id}")]
        //public async Task<IActionResult> JoinGroup(int id)
        //{

        //}
    }
}