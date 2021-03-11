using FamousQuoteQuiz.Models;
using FamousQuoteQuiz.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamousQuoteQuiz.Services
{
    public class QuoteService : BaseService
    {
        private readonly QuoteRepository quoteRepository;
        public QuoteService(IConfiguration configuration) : base(configuration)
        {
            this.quoteRepository = new QuoteRepository(configuration);
        }
        public async Task<GenericResponse<List<QuestionModel>>> GetQuestions(bool? isActiveQuestion = true)
        {
            var tw = GetTransactionWrapperWithoutTransaction();

            var question = await quoteRepository.GetQuestions(tw);

            if (isActiveQuestion.HasValue && !isActiveQuestion.Value)
            {
                return new GenericResponse<List<QuestionModel>>(StatusCode.SUCCESS, question);
            }

            return new GenericResponse<List<QuestionModel>>(StatusCode.SUCCESS, question.Where(i => i.IsActive).ToList());
        }

        public async Task<GenericResponse<List<AnswerModel>>> GetAnswersByQuestionID(int questionID, bool? isActiveAnswer = true)
        {
            var tw = GetTransactionWrapperWithoutTransaction();

            var question = await quoteRepository.GetAnswersByQuestionID(questionID, tw);

            if (isActiveAnswer.HasValue && !isActiveAnswer.Value)
            {
                return new GenericResponse<List<AnswerModel>>(StatusCode.SUCCESS, question);
            }

            return new GenericResponse<List<AnswerModel>>(StatusCode.SUCCESS, question.Where(i => i.IsActive).ToList());
        }


        internal async Task EditQuestion(string question, int questionID)
        {

            using (var tw = GetTransactionWrapper())
            {
                await quoteRepository.EditQuestion(question, questionID, tw);

                tw.Commit();

            }

        }

        internal async Task<GenericResponse<AnswerModel>> AddAnswer(int questionID, string answer, bool isTrue)
        {
            using (var tw = GetTransactionWrapper())
            {
                var questionAnswers = quoteRepository.GetAnswersByQuestionID(questionID, tw);

                if (questionAnswers.Result.Exists(i => i.IsTrue) && isTrue)
                {
                    return new GenericResponse<AnswerModel>(StatusCode.ONE_QUESTION_CAN_ONLY_HAVE_ONE_ANSWER);
                }

                var insertedAnswer = await quoteRepository.AddAnswer(questionID, answer, isTrue, tw);

                tw.Commit();

                return new GenericResponse<AnswerModel>(StatusCode.SUCCESS, insertedAnswer);
            }
        }

        internal async Task<GenericResponse<AnswerModel>> EditAnswer(int answerID, string answer, bool isTrue)
        {
            using (var tw = GetTransactionWrapper())
            {
                var question = await quoteRepository.GetQuestionByAnswerID(answerID, tw);

                var questionAnswers = quoteRepository.GetAnswersByQuestionID(question.ID, tw);

                if (questionAnswers.Result.Exists(i => i.IsTrue) && isTrue)
                {
                    return new GenericResponse<AnswerModel>(StatusCode.ONE_QUESTION_CAN_ONLY_HAVE_ONE_ANSWER);
                }
                else if (questionAnswers.Result.Exists(i => i.Answer.Equals(answer, StringComparison.OrdinalIgnoreCase)))
                {
                    return new GenericResponse<AnswerModel>(StatusCode.THIS_ANSWER_ALREADY_EXISTS);
                }
                await quoteRepository.EditAnswer(answerID, answer, isTrue, tw);

                tw.Commit();

                return new GenericResponse<AnswerModel>(StatusCode.SUCCESS);
            }
        }

        internal async Task<GenericResponse<QuestionModel>> DeleteQuestion(int questionID)
        {
            using (var tw = GetTransactionWrapper())
            {
                var question = await quoteRepository.GetQuestionByID(questionID, tw);

                if (question == null || !question.IsActive)
                {
                    return new GenericResponse<QuestionModel>(StatusCode.SUCH_QUESTION_DOES_NOT_EXIST);
                }
                await quoteRepository.DeleteQuestion(questionID, tw);

                tw.Commit();

                return new GenericResponse<QuestionModel>(StatusCode.SUCCESS);
            }
        }
        internal async Task DeleteAnswer(int answerID)
        {
            using (var tw = GetTransactionWrapper())
            {
                await quoteRepository.DeleteAnswer(answerID, tw);

                tw.Commit();
            }
        }

        public async Task<QuestionModel> AddQuestion(string question, int creatorUserID)
        {
            using (var tw = GetTransactionWrapper())
            {
                var insertedQuestion = await quoteRepository.AddQuestion(question, creatorUserID, tw);

                tw.Commit();

                return insertedQuestion;

            }
        }
        internal async Task<GenericResponse<Dictionary<Tuple<int, string>, List<AnswerList>>>> GetQuestionsForUser(bool? isbinary = true)
        {
            var tw = GetTransactionWrapperWithoutTransaction();

            var questAnswers = GetQuestionsAndAnswers();

            var dict = new Dictionary<Tuple<int, string>, List<AnswerList>>();

            foreach (var item in questAnswers.Result.Response)
            {
                List<AnswerList> lists = new List<AnswerList>();

                if (isbinary.Value)
                {
                    lists.Add(new AnswerList
                    {
                        Answer = item.Value.ElementAt(0).Answer,
                        AnswernID = item.Value.ElementAt(0).ID
                    });
                }
                else
                {
                    foreach (var answ in item.Value)
                    {
                        lists.Add(new AnswerList
                        {
                            Answer = answ.Answer,
                            AnswernID = answ.ID
                        });

                    }
                }
                dict.Add(Tuple.Create(

                    item.Key.Item1,
                    item.Key.Item2
                ),
                lists);
            }

            return new GenericResponse<Dictionary<Tuple<int, string>, List<AnswerList>>>(StatusCode.SUCCESS, dict);
        }
        public async Task<GenericResponse<Dictionary<Tuple<int, string>, List<AnswerModel>>>> GetQuestionsAndAnswers(bool? isActiveQuestion = true, bool? isActiveAnswer = true)
        {
            var tw = GetTransactionWrapperWithoutTransaction();

            var question = await GetQuestions(isActiveQuestion);
            var dict = new Dictionary<Tuple<int, string>, List<AnswerModel>>();

            foreach (var q in question.Response)
            {
                var answers = await GetAnswersByQuestionID(q.ID, isActiveAnswer);

                if (answers.Response.Count > 0)
                {
                    dict.Add(Tuple.Create(q.ID, q.Question), answers.Response);
                }

            }

            return new GenericResponse<Dictionary<Tuple<int, string>, List<AnswerModel>>>(StatusCode.SUCCESS, dict);
        }
    }
}

