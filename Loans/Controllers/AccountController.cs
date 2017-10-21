using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Loans.DataTransferObjects.Account;
using Loans.Extensions;
using Loans.Models;
using Loans.Models.VK;
using Loans.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        [HttpGet("authenticate")]
        public async Task<IActionResult> Authenticate(
            [FromServices]IServiceProvider services,
            [FromServices]IHostingEnvironment env,
            [FromServices]IOptions<VKSettings> vkSettings,
            [FromServices]IOptions<JwtSettings> jwtSettings,
            [FromQuery]string error,
            [FromQuery(Name = "error_description")] string errorDescription,
            [FromQuery]string code,
            [FromQuery]string state)
        {
            VKSettings vk = vkSettings.Value;

            if (code != null)
            {
                var client = await Client.AuthenticateFromCode(vk, code);
                var userJson = (await client.PostAsync("users.get", ("fields", "screen_name,photo_max")))["response"][0];

                var user = new ApplicationUser
                {
                    Id = userJson.Value<int>("id"),

                    UserName = userJson.Value<string>("screen_name"),
                    Email = client.Email,

                    FirstName = userJson.Value<string>("first_name"),
                    LastName = userJson.Value<string>("last_name")
                };

                // HAAAAAAAAAAAAAAAAAAAAAAAX
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var store = (
                    UserStore<
                        ApplicationUser,
                        ApplicationRole,
                        LoansContext,
                        int,
                        IdentityUserClaim<int>,
                        IdentityUserRole<int>,
                        IdentityUserLogin<int>,
                        IdentityUserToken<int>,
                        IdentityRoleClaim<int>
                    >)_userManager
                    .GetType()
                    .GetProperty("Store", flags)
                    .GetValue(_userManager);

                await store.Context.Database.OpenConnectionAsync();
                IdentityResult result = null;
                try
                {
                    store.Context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.AspNetUsers ON");
                    result = await _userManager.CreateAsync(user);
                    store.Context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.AspNetUsers OFF");
                }
                catch (InvalidOperationException)
                {

                }
                finally
                {
                    store.Context.Database.CloseConnection();
                }

                if (result?.Succeeded == false)
                {
                    foreach (IdentityError err in result.Errors)
                    {
                        ModelState.AddModelError("Identity", err.Description);
                    }

                    return BadRequest(ModelState);
                }

                using (var webClient = new WebClient())
                {
                    await webClient.DownloadFileTaskAsync(
                        userJson.Value<string>("photo_max"),
                        Path.Combine(env.WebRootPath, "avatars", user.Id + ".jpg")
                    );
                }

                Claim[] claims =
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var creds = new SigningCredentials(
                    jwtSettings.Value.SigningKey,
                    SecurityAlgorithms.HmacSha256
                );

                var token = new JwtSecurityToken(
                    jwtSettings.Value.ValidIssuer,
                    jwtSettings.Value.ValidAudience,
                    claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );

                string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                if (string.IsNullOrEmpty(state))
                {
                    return Ok(tokenString);
                }

                return Redirect(state + "?token=" + WebUtility.UrlEncode(tokenString));
            }

            if (error != null)
            {
                return BadRequest(new { error, errorDescription });
            }

            var uri = new StringBuilder("https://oauth.vk.com/authorize?response_type=code")
                .AppendQuery("client_id", vk.ClientId)
                .AppendQuery("display", vk.Display)
                .AppendQuery("redirect_uri", vk.RedirectUri)
                .AppendQuery("scope", vk.Scope)
                .AppendQuery("v", vk.Scope)
                .AppendQuery("state", state)
                .ToString();

            return Redirect(uri);
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
                    Id = model.Id,

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