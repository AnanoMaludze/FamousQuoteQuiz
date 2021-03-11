using FamousQuoteQuiz.Models;
using FamousQuoteQuiz.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;

namespace FamousQuoteQuiz.Services
{
    public class UserServices : BaseService
    {

        private readonly UserRepository userRepository;
        private readonly QuoteRepository quoteRepository;

        public UserServices(IConfiguration configuration) : base(configuration)
        {
            this.userRepository = new UserRepository(configuration);
            this.quoteRepository = new QuoteRepository(configuration);
        }

        public async Task<GenericResponse<User>> CreateUser(string username, string password)
        {
            using (var tw = GetTransactionWrapper())
            {
                var user = await userRepository.GetUserByUsername(username, tw);

                if (user != null)
                {
                    return new GenericResponse<User>(StatusCode.USERNAME_ALREADY_EXISTS);
                }

                var result = await userRepository.CreateUser(username, EncodePasswordToBase64(password), tw);

                tw.Commit();

                return new GenericResponse<User>(StatusCode.SUCCESS, result);
            }

        }


        public async Task<GenericResponse<User>> LoginUser(string username, string password)
        {
            var tw = GetTransactionWrapperWithoutTransaction();

            var user = await userRepository.GetUserByUsername(username, tw);

            if (user == null)
            {
                return new GenericResponse<User>(StatusCode.INCORRECT_USERNAME);
            }

            else if (!EncodePasswordToBase64(password).Equals(user.Password))
            {
                return new GenericResponse<User>(StatusCode.INCORRECT_PASSWORD);
            }
            return new GenericResponse<User>(StatusCode.SUCCESS, user);
        }


        public async Task<GenericResponse<User>> GetUser(int ID)
        {
            var tw = GetTransactionWrapperWithoutTransaction();

            var result = await userRepository.GetUserByID(ID, tw);

            return new GenericResponse<User>(StatusCode.SUCCESS, result);
        }

        public async Task<GenericResponse<User>> GetUserByUsername(string username)
        {
            var tw = GetTransactionWrapperWithoutTransaction();

            var result = await userRepository.GetUserByUsername(username, tw);

            return new GenericResponse<User>(StatusCode.SUCCESS, result);
        }

        public static string EncodePasswordToBase64(string password)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] inArray = HashAlgorithm.Create("SHA1").ComputeHash(bytes);
            return Convert.ToBase64String(inArray);
        }


        internal async Task UpdatePassword(string password, int userID)
        {
            using (var tw = GetTransactionWrapper())
            {
                var encodedPassword = EncodePasswordToBase64(password);

                await userRepository.UpdatePassword(userID, encodedPassword, tw);
            }

        }

        internal async Task DeleteUser(int userID)
        {
            using (var tw = GetTransactionWrapper())
            {
                await userRepository.DeleteUser(userID, tw);
            }

        }

        internal async Task DisableUser(int userID)
        {
            using (var tw = GetTransactionWrapper())
            {
                await userRepository.DisableUser(userID, tw);
            }

        }

        internal async Task RegisterUserQuoteAnswer(int userID, UserQuestionAnswerHistory model)
        {
            using (var tw = GetTransactionWrapper())
            {
                var questionAnswer = await quoteRepository.GetAnswersByQuestionID(model.QuestionID, tw);

                if (model.isBinary)
                {
                    var isCorrect = questionAnswer.Where(i => i.IsTrue).Select(i => i.ID).FirstOrDefault() == model.AnswerID;

                    if ((model.answeredTrue && isCorrect)
                        || (!model.answeredTrue && !isCorrect))
                    {
                        await userRepository.RegisterUserQuoteAnswer(new UserQuestionAnswerHistory
                        {
                            AnswerID = model.AnswerID,
                            answeredTrue = true,
                            CreateDate = DateTime.UtcNow,
                            isBinary = true,
                            UserID = userID,
                            QuestionID = model.QuestionID

                        }, tw);
                    }
                    if ((!model.answeredTrue && isCorrect) || ((model.answeredTrue && !isCorrect)))
                    {
                        await userRepository.RegisterUserQuoteAnswer(new UserQuestionAnswerHistory
                        {
                            AnswerID = model.AnswerID,
                            answeredTrue = false,
                            CreateDate = DateTime.UtcNow,
                            isBinary = true,
                            UserID = userID,
                            QuestionID = model.QuestionID

                        }, tw);
                    }
                    tw.Commit();
                }
                else
                {
                    var isCorrect = questionAnswer.Where(i => i.IsTrue).Select(i => i.ID).FirstOrDefault() == model.AnswerID;


                    await userRepository.RegisterUserQuoteAnswer(new UserQuestionAnswerHistory
                    {
                        AnswerID = model.AnswerID,
                        answeredTrue = isCorrect,
                        CreateDate = DateTime.UtcNow,
                        isBinary = false,
                        UserID = userID,
                        QuestionID = model.QuestionID

                    }, tw);

                    tw.Commit();
                }
            }

        }

        internal async Task<List<UserQuestionAnswerHistory>> ListUserQuestionAnswerHistory(int UserID)
        {
            var tw = GetTransactionWrapperWithoutTransaction();

            var list = await userRepository.GetUserQuestionAnswers(UserID, tw);

            return list;
        }
    }
}

