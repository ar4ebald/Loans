using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Loans.DataTransferObjects;
using Loans.Models;
using Loans.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Loans.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("random")]
        public int Random()
        {
            return new Random().Next();
        }

        [Authorize, HttpGet("random2")]
        public int Random2()
        {
            return new Random().Next();
        }

        [Authorize, HttpGet("current")]
        public IActionResult GetCurrent()
        {
            return Json(User.Identity.Name);
        }

        [HttpPost("register")]
        public async Task<ValidationResultModel> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Nickname,
                    Email = model.Email,

                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return ValidationResultModel.Success;
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return new ValidationResultModel(ModelState);
        }

        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken(
            [FromServices] IOptions<JwtSettings> settings,
            LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                    if (result.Succeeded)
                    {
                        var claims = await _userManager.GetClaimsAsync(user);
                        //var claims = new[]
                        //{
                        //    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        //};

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

                        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
                    }
                }

                return BadRequest("Invalid email or password");
            }

            return BadRequest("Could not create token");
        }
    }
}