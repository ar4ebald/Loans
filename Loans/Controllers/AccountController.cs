using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Loans.DataTransferObjects.Account;
using Loans.Models;
using Loans.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Loans.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Returns bearer token claims
        /// </summary>
        /// <remarks>
        /// This method is only for user's claims testing
        /// </remarks>
        [Authorize, HttpGet("claims")]
        public IActionResult GetCurrent()
        {
            return Json(User.Claims.Select(i => new { i.Type, i.Value }));
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="model">New user view model</param>
        /// <response code="200">Successfully registered</response>
        /// <response code="400">Registration failed</response>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,

                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Ok();
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("Identity", error.Description);
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Authenticates using specified credentials
        /// </summary>
        /// <param name="model">Credentials</param>
        /// <returns>Bearer token to be used in authenticated requests</returns>
        /// <remarks>
        /// To send authenticated requests you need to add Authentication header
        /// with value "Bearer {token}"
        /// </remarks>
        /// <response code="200">Bearer token</response>
        /// <response code="400">Invalid credentials</response>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken(
            [FromServices] IOptions<JwtSettings> settings,
            [FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                    if (result.Succeeded)
                    {
                        Claim[] claims =
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        };

                        var creds = new SigningCredentials(
                            settings.Value.SigningKey,
                            SecurityAlgorithms.HmacSha256
                        );

                        var token = new JwtSecurityToken(
                            settings.Value.ValidIssuer,
                            settings.Value.ValidAudience,
                            claims,
                            expires: DateTime.Now.AddMinutes(30),
                            signingCredentials: creds
                        );

                        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    }
                }

                ModelState.AddModelError("Credentials", "Invalid email or password");
            }

            return BadRequest(ModelState);
        }
    }
}