using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loans.DataTransferObjects;
using Loans.DataTransferObjects.Community;
using Loans.Extensions;
using Loans.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;

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

        /// <summary>
        /// Creates new community
        /// </summary>
        /// <param name="model">New community model</param>
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
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

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Returns communities that contains current user
        /// </summary>
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

        //[HttpGet("{communityId}")]
        //public async Task<IActionResult> Get(int communityId)
        //{
        //    var userId = User.GetIdentifier();

        //    var result = await _context.Communities
        //        .Where(community => community.Id == communityId &&
        //                            community.Members.Any(enrollment => enrollment.UserId == userId))
        //        .Select(community => new GetCommunityResponse
        //        {
        //            Id = community.Id,
        //            Name = community.Name,
        //            //Members = community.Members.Select(enrollment => enrollment.User).Select(UserModel.FromQuery)
        //        })
        //        .FirstOrDefaultAsync();

        //    if (result == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(result);
        //}
        

        //[HttpPost("{id}/join")]
        //public async Task<IActionResult> JoinGroup(int id)
        //{
        //    var userId = User.GetIdentifier();

        //    await _context.CommunitiesEnrollments.AddAsync(new CommunityEnrollment
        //    {
        //        UserId = userId,
        //        CommunityId = id
        //    });
        //}
    }
}