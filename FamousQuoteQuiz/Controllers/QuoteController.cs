using FamousQuoteQuiz.Models;
using FamousQuoteQuiz.Services;
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
    public class QuoteController : Controller
    {
        protected readonly IConfiguration configuration;

        protected QuoteService quoteService;
        protected UserServices userServices;

        public QuoteController(IConfiguration configuration)
        {
            this.quoteService = new QuoteService(configuration);
            this.userServices = new UserServices(configuration);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        [Route(nameof(GetQuestions))]

        public async Task<IActionResult> GetQuestions()
        {
            var questions = await quoteService.GetQuestionsAndAnswers();

            var json = JsonConvert.SerializeObject(questions, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            
            return new OkObjectResult(json);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        [Route(nameof(GetQuestionsForUser))]
        public async Task<IActionResult> GetQuestionsForUser(bool? isbinary)
        {
            var questions = await quoteService.GetQuestionsForUser(isbinary ?? true);

            var json = JsonConvert.SerializeObject(questions, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return new OkObjectResult(json);
        }

        [HttpPost]
        [Route(nameof(AddQuestion))]
        public async Task<JsonResult> AddQuestion([FromBody] CreateQuote model)
        {
            var user = await userServices.GetUserByUsername(User.Identity.Name);

            return new JsonResult(await quoteService.AddQuestion(model.Quote, user.Response.ID));
        }

        [HttpPost]
        [Route(nameof(EditQuestion))]
        public async Task<IActionResult> EditQuestion([FromBody] EditQuote model)
        {
            await quoteService.EditQuestion(model.Quote, model.QuoteID);

            return new OkResult();
        }

        [HttpPost]
        [Route(nameof(AddAnswer))]
        public async Task<IActionResult> AddAnswer([FromBody] AddAnswerModel model)
        {
            var result = await quoteService.AddAnswer(model.QuestionID, model.Answer, model.IsTrue);

            var insertedAnswer = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return new OkObjectResult(insertedAnswer);
        }

        [HttpPost]
        [Route(nameof(EditAnswer))]
        public async Task<IActionResult> EditAnswer([FromBody] EditAnswerModel model)
        {
            await quoteService.EditAnswer(model.AnswerID, model.Answer, model.IsTrue);

            return new OkResult();
        }

        [HttpDelete]
        [Route(nameof(DeleteQuestion))]
        public async Task<IActionResult> DeleteQuestion([FromBody] DeleteQuoteModel model)
        {
            await quoteService.DeleteQuestion(model.QuoteID);

            return new OkResult();
        }

        [HttpDelete]
        [Route(nameof(DeleteAnswer))]
        public async Task<IActionResult> DeleteAnswer([FromBody] DeleteAnswerModel model)
        {
            await quoteService.DeleteAnswer(model.AnswerID);

            return new OkResult();
        }
    }
}
