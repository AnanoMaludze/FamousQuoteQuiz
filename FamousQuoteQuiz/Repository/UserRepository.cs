using CoreLib.Repository;
using Dapper;
using FamousQuoteQuiz.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamousQuoteQuiz.Repository
{
    public class UserRepository
    {
        private readonly IConfiguration configuration;
        public UserRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<User> GetUserByID(int ID, TransactionWrapper tw)
        {
            var User = await tw.Connection.QuerySingleAsync<User>(@"
                                                                            SELECT * 
                                                                            FROM   dbo.users 
                                                                            WHERE  [id] = @ID 
                                                                            ",
                new
                {
                    ID
                }, transaction: tw.Transaction);

            return User;
        }
        public async Task<List<User>> GetUsers(TransactionWrapper tw)
        {
            var Users = await tw.Connection.QueryAsync<User>(@"
                                                            SELECT * 
                                                            FROM   dbo.users 
                                                            WHERE  [isDeleted] = 'false'
                                                            "
                , transaction: tw.Transaction);

            return Users.ToList();
        }
        public async Task<List<UserQuestionAnswerHistory>> GetUserQuestionAnswers(int userID, TransactionWrapper tw)
        {
            var Users = await tw.Connection.QueryAsync<UserQuestionAnswerHistory>(@"
                                                            SELECT * 
                                                            FROM   dbo.UserQuestionAnswerHistory 
                                                            WHERE  [userID] = @userID
                                                            ",
                                                            new
                                                            {
                                                                userID
                                                            }
                , transaction: tw.Transaction);

            return Users.ToList();
        }

        internal async Task<User> CreateUser(string username, string password, TransactionWrapper tw)
        {
            var inserted = await tw.Connection.QueryFirstAsync<User>(@"
                                                                                DECLARE @id int; 

                                                                                INSERT INTO [dbo].[users] 
                                                                                            ([username], 
                                                                                             [password]
                                                                                             ) 
                                                                                output      inserted.* 
                                                                                VALUES      (@username, 
                                                                                             @password
                                                                                            ) 

                                                                                SET @id = Scope_identity(); 

                                                                                SELECT @id; 
                                                                              ", new
            {
                username,
                password,


            }, transaction: tw.Transaction);

            return inserted;
        }

        internal async Task<User> GetUserByUsername(string username, TransactionWrapper tw)
        {
            var User = await tw.Connection.QuerySingleOrDefaultAsync<User>(@"
                                                                    SELECT * 
                                                                    FROM   dbo.users 
                                                                    WHERE  [username] = @username 
                                                                    ",
               new
               {
                   username
               }, transaction: tw.Transaction);

            return User;
        }

        internal async Task UpdatePassword(int userID, string password, TransactionWrapper tw)
        {

            await tw.Connection.QueryAsync(@"
                                                            Update dbo.users
                                                            set password = @password
                                                            where userid = @userID
                                                        ",
                                                        new
                                                        {
                                                            password = password,
                                                            userID = userID
                                                        },
                transaction: tw.Transaction);

        }

        internal async Task DeleteUser(int userID, TransactionWrapper tw)
        {
            await tw.Connection.QueryAsync(@"
                                                            Update dbo.users
                                                            set isDeleted = 'true'
                                                            where userid = @userID
                                                        ",
                                                        new
                                                        {
                                                            userID = userID
                                                        },
                transaction: tw.Transaction);
        }

        internal async Task RegisterUserQuoteAnswer(UserQuestionAnswerHistory model, TransactionWrapper tw)
        {
            try
            {
                var inserted = await tw.Connection.ExecuteAsync(@"
                                                                                
                                                                                INSERT INTO [dbo].[userQuestionAnswerHistory] 
                                                                                            ([userID], 
                                                                                             [questionID],
                                                                                             [answerID],
                                                                                             [isCorrectlyAnswered],
                                                                                             [createDate],
                                                                                             [isbinary]
                                                                                             ) 
                                                                                VALUES      (@UserID, 
                                                                                             @QuestionID, 
                                                                                             @AnswerID, 
                                                                                             @answeredTrue, 
                                                                                             @CreateDate, 
                                                                                             @isBinary
                                                                                            ) 
                                                                              
                                                                              ", new
                {
                    UserID = model.UserID,
                    QuestionID = model.QuestionID,
                    AnswerID = model.AnswerID,
                    answeredTrue = model.answeredTrue,
                    CreateDate = model.CreateDate,
                    isBinary = model.isBinary


                }, transaction: tw.Transaction);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal async Task DisableUser(int userID, TransactionWrapper tw)
        {
            await tw.Connection.QueryAsync(@"
                                                Update dbo.users
                                                set isActive = 'False'
                                                where userid = @userID
                                            ",
                                                        new
                                                        {
                                                            userID = userID
                                                        },
                transaction: tw.Transaction);
        }
    }
}
