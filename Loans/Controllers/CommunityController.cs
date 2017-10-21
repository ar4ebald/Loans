using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loans.DataTransferObjects.Community;
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
    [Route("api/community")]
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
        /// <response code="200">Successfully created</response>
        /// <response code="400">Creation failed</response>
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
        public IEnumerable<CommunityViewModel> GetPersonalCommunities()
        {
            var userId = User.GetIdentifier();

            return _context.CommunitiesEnrollments
                .Where(enrollment => enrollment.UserId == userId)
                .Select(enrollment => new CommunityViewModel
                {
                    Id = enrollment.Community.Id,
                    Name = enrollment.Community.Name
                });
        }

        /// <summary>
        /// Returns community information and members by identifier
        /// </summary>
        /// <param name="id">Requested community identifier</param>
        /// <response code="200">Community information</response>
        /// <response code="400">Invalid community or you are not a member of it</response>
        [ProducesResponseType(typeof(GetCommunityResponse), StatusCodes.Status200OK)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = User.GetIdentifier();

            var community = await _context.CommunitiesEnrollments
                .Where(e => e.CommunityId == id && e.UserId == userId)
                .Select(e => new GetCommunityResponse
                {
                    Id = e.Community.Id,
                    Name = e.Community.Name
                })
                .FirstOrDefaultAsync();

            if (community == null)
            {
                ModelState.AddModelError("Community", "Invalid community or you are not a member of it");
                return NotFound();
            }

            community.Members = _context.CommunitiesEnrollments
                .Where(e => e.CommunityId == community.Id)
                .Select(e => e.User)
                .Select(UserModel.FromQuery);

            return Ok(community);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string pattern, int offset = 0, int count = 20)
        {
            IQueryable<Community> communities = _context.Communities;

            if (!string.IsNullOrEmpty(pattern))
            {
                communities = communities
                    .Where(u => u.Name.StartsWith(pattern));
            }

            var response = new CommunitySearchResponse
            {
                Count = await communities.CountAsync(),
                Communities = communities.OrderBy(communuty => communuty.Name)
                    .Skip(Math.Max(0, offset))
                    .Take(Math.Max(0, Math.Min(100, count)))
                    .Select(CommunityViewModel.FromQuery)
            };

            return Ok(response);
        }


        /// <summary>
        /// Joins specified community
        /// </summary>
        /// <param name="id">Community identifier</param>
        /// <response code="200">Successfully joined</response>
        /// <response code="400">Invalid community id or user is already a member of this community</response>
        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinCommunity(int id)
        {
            await _context.CommunitiesEnrollments.AddAsync(new CommunityEnrollment
            {
                UserId = User.GetIdentifier(),
                CommunityId = id
            });

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("Join", "Invalid community or you are already a member of it");
                return BadRequest(ModelState);
            }


            return Ok();
        }

        /// <summary>
        /// Leaves specified community
        /// </summary>
        /// <param name="id">Community identifier</param>
        /// <response code="200">Successfully left</response>
        /// <response code="400">Invalid community or you have already left this community</response>
        [HttpPost("{id}/leave")]
        public async Task<IActionResult> LeaveCommunity(int id)
        {
            _context.CommunitiesEnrollments.Remove(new CommunityEnrollment
            {
                UserId = User.GetIdentifier(),
                CommunityId = id
            });

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("Leave", "Invalid community or you have already left this community");
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpPost("{id}/graph")]
        public async Task<string> GetGraph(int id)
        {
            IQueryable<ApplicationUser> users = _context.CommunitiesEnrollments
                .Where(e => e.CommunityId == id)
                .Select(e => e.User);

            //IQueryable<ApplicationUser> users = _context.Users;

            var edges = await _context.LoanSummaries.Join(
                users,
                summary => summary.CreditorId,
                user => user.Id,
                (summary, user) => summary
            ).Join(
                users,
                summary => summary.DebtorId,
                user => user.Id,
                (summary, user) => new
                {
                    Creditor = summary.Creditor.UserName,
                    Debtor = summary.Debtor.UserName,
                    summary.TotalAmount
                }
            ).ToListAsync();


            var dict = new Dictionary<(string creditor, string debtor), long>();

            foreach (var edge in edges)
            {
                long total = edge.TotalAmount;

                if (dict.TryGetValue((edge.Debtor, edge.Creditor), out long amount))
                {
                    total -= amount;
                    dict.Remove((edge.Debtor, edge.Creditor));
                }

                if (total > 0)
                {
                    dict[(edge.Creditor, edge.Debtor)] = total;
                }
                else if (total < 0)
                {
                    dict[(edge.Debtor, edge.Creditor)] = -total;
                }
            }

            return $"digraph G {{\n{string.Join("\n", dict.Select(e => $"{e.Key.debtor} -> {e.Key.creditor}  [ label = \"{e.Value}\" ]"))}\n}}\n";
        }

        [HttpPost("{id}/optimize")]
        public async Task<IActionResult> OptimizeOperations(int id)
        {
            IQueryable<ApplicationUser> users = _context.CommunitiesEnrollments
                .Where(e => e.CommunityId == id)
                .Select(e => e.User);

            //IQueryable<ApplicationUser> users = _context.Users;

            var edges = (await _context.LoanSummaries.Join(
                    users,
                    summary => summary.CreditorId,
                    user => user.Id,
                    (summary, user) => summary
                ).Join(
                    users,
                    summary => summary.DebtorId,
                    user => user.Id,
                    (summary, user) => new
                    {
                        Debtor = summary.DebtorId,
                        Creditor = summary.CreditorId,
                        summary.TotalAmount
                    }
                ).ToListAsync())
                .Select(edge => (debtor: edge.Debtor, creditor: edge.Creditor, amount: edge.TotalAmount))
                .ToArray();

            var ids = edges
                .Select(e => e.debtor).Concat(edges.Select(e => e.creditor))
                .Distinct()
                .ToArray();

            var ind = ids
                .Select((value, index) => (value, index))
                .ToDictionary(i => i.value, i => i.index);

            var graph = new long[ind.Count, ind.Count];

            ref long g(int i, int j)
            {
                if (i > j)
                {
                    (i, j) = (j, i);
                }

                return ref graph[i, j];
            }

            void add(int i, int j, long value)
            {
                if (i < j)
                    graph[i, j] += value;
                else
                    graph[j, i] -= value;
            }

            foreach (var (debtor, creditor, amount) in edges)
            {
                add(ind[debtor], ind[creditor], amount);
            }

            bool found;
            do
            {
                found = false;

                for (int i = 0; i < ind.Count; ++i)
                {
                    for (int j = 0; j < ind.Count; ++j)
                    {
                        var (v1, v2, amountIn) = (i, j, g(i, j));

                        if (v1 == v2 || amountIn <= 0)
                        {
                            continue;
                        }

                        for (int k = 0; k < ind.Count; ++k)
                        {
                            var (v3, amountOut) = (k, g(v2, k));

                            if (v3 == v1 || v3 == v2 || amountOut <= 0)
                            {
                                continue;
                            }

                            long amount = Math.Min(amountIn, amountOut);

                            add(v1, v2, -amount);
                            add(v2, v3, -amount);
                            add(v1, v3, amount);

                            found = true;
                        }
                    }
                }
            } while (found);

            var sb = new StringBuilder("digraph G {").AppendLine();
            for (int i = 0; i < ind.Count; ++i)
            {
                for (int j = i + 1; j < ind.Count; ++j)
                {
                    var (u, v, w) = (ids[i], ids[j], graph[i, j]);

                    if (w == 0)
                    {
                        continue;
                    }

                    if (w < 0)
                    {
                        (u, v, w) = (v, u, -w);
                    }

                    sb.AppendLine($"{u} -> {v} [ label = \"{w}\" ]");
                }
            }
            sb.AppendLine("}");

            return Ok(sb.ToString());
        }
    }
}