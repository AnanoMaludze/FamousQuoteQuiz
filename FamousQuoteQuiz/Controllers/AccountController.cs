using FamousQuoteQuiz.Models;
using FamousQuoteQuiz.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FamousQuoteQuiz.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        protected readonly IConfiguration configuration;

        protected UserServices userServices;
        protected QuoteService quoteService;
        public AccountController(IConfiguration configuration)
        {
            this.userServices = new UserServices(configuration);
            this.quoteService = new QuoteService(configuration);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        [Route(nameof(RegisterUser))]
        public async Task<IActionResult> RegisterUser(string username, string password, string repeatedPassword)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new OkObjectResult(new { StatusCode = Models.StatusCode.USERNAME_CANNOT_BE_EMPTY });
            }
            if (!password.Equals(repeatedPassword, StringComparison.OrdinalIgnoreCase))
            {
                return new OkObjectResult(new { StatusCode = Models.StatusCode.PASSWORD_MISSMATCH });
            }
            ClaimsIdentity identity = null;
            bool isAuthenticate = false;

            var user = await userServices.CreateUser(username, password);

            if (user.Status != Models.StatusCode.SUCCESS)
            {
                return new OkObjectResult(new { StatusCode = Models.StatusCode.ERROR });
            }

            return RedirectToAction("Login");
        }
        [HttpGet]
        [Route(nameof(Login))]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                return RedirectToAction("Login");
            }
            ClaimsIdentity identity = null;
            bool isAuthenticate = false;

            var user = await userServices.LoginUser(username, password);

            if (user.Status != Models.StatusCode.SUCCESS)
            {
                return RedirectToAction("Login");
            }
            if (user.Response.IsAdmin && user.Response.IsActive)
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name,username),
                    new Claim(ClaimTypes.Role,"Admin")
                }, CookieAuthenticationDefaults.AuthenticationScheme);
                isAuthenticate = true;
            }
            else if (user.Response.IsActive)
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name,username),
                    new Claim(ClaimTypes.Role,"User"),
                    new Claim(ClaimTypes.NameIdentifier,user.Response.ID.ToString())
                }, CookieAuthenticationDefaults.AuthenticationScheme);
                isAuthenticate = true;
            }
            if (isAuthenticate)
            {
                var principal = new ClaimsPrincipal(identity);
                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            }
            return Ok();
        }

        [HttpPut]
        [Authorize]
        [Route(nameof(Update))]
        public async Task<IActionResult> Update([FromBody] UpdateUserPasswordModel model)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                return new OkObjectResult(new { StatusCode = Models.StatusCode.PASSWORD_CANNOT_BE_EMPTY });
            }
            var user = await userServices.GetUserByUsername(User.Identity.Name);

            await userServices.UpdatePassword(model.Password, user.Response.ID);

            return Ok();
        }


        [HttpPost]
        [Authorize]
        [Route(nameof(RegisterUserQuoteAnswer))]
        public async Task<IActionResult> RegisterUserQuoteAnswer([FromBody] UserQuestionAnswerHistory model)
        {
            var user = await userServices.GetUserByUsername(User.Identity.Name);

            await userServices.RegisterUserQuoteAnswer(user.Response.ID, model);

            return Ok();
        }

        [HttpPost]
        [Authorize]
        [Route(nameof(ListUserQuestionAnswerHistory))]
        public async Task<IActionResult> ListUserQuestionAnswerHistory()
        {
            var user = await userServices.GetUserByUsername(User.Identity.Name);

            var history = await userServices.ListUserQuestionAnswerHistory(user.Response.ID);

            var json = JsonConvert.SerializeObject(history, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            
            return new OkObjectResult(json);

        }
    }
}
