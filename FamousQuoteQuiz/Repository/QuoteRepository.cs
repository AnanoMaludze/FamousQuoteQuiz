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
    public class QuoteRepository
    {
        private readonly IConfiguration configuration;
        public QuoteRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<QuestionModel> GetQuestionByID(int ID, TransactionWrapper tw)
        {
            var question = await tw.Connection.QuerySingleOrDefaultAsync<QuestionModel>(@"
                                                                            SELECT * 
                                                                            FROM   dbo.Questions 
                                                                            WHERE  [id] = @ID 
                                                                            ",
                new
                {
                    ID = ID
                }, transaction: tw.Transaction);

            return question;
        }

        public async Task<List<QuestionModel>> GetQuestions(TransactionWrapper tw)
        {
            var question = await tw.Connection.QueryAsync<QuestionModel>(@"
                                                                            SELECT * 
                                                                            FROM dbo.Questions 
                                                                            ",
                 transaction: tw.Transaction);

            return question.ToList();
        }

        internal async Task<List<AnswerModel>> GetAnswersByQuestionID(int questionID, TransactionWrapper tw)
        {
            var question = await tw.Connection.QueryAsync<AnswerModel>(@"
                                                                            SELECT * 
                                                                            FROM dbo.Answers 
                                                                               where questionID = @questionID
                                                                            ",
                                                                            new
                                                                            {
                                                                                questionID = questionID
                                                                            },
                transaction: tw.Transaction);

            return question.ToList();
        }
        internal async Task<QuestionModel> GetQuestionByAnswerID(int answerID, TransactionWrapper tw)
        {
            var question = await tw.Connection.QueryFirstOrDefaultAsync<QuestionModel>(@"
                                                                            SELECT a.* 
                                                                            FROM dbo.questions as a
                                                                            join dbo.answers as b
                                                                            on a.ID = b.questionID
                                                                            where b.ID = @answerID

                                                                            ",
                                                                            new
                                                                            {
                                                                                answerID = answerID
                                                                            },
                transaction: tw.Transaction);

            return question;
        }
        internal async Task EditAnswer(int answerID, string answer, bool? isTrue, TransactionWrapper tw)
        {
            await tw.Connection.QueryAsync<QuestionModel>(@"
                                                            Update dbo.answers
                                                            set answer = @answer,
                                                                isTrue = @isTrue
                                                            where ID = @answerID
                                                        ",
                                                        new
                                                        {
                                                            answerID,
                                                            answer,
                                                            isTrue
                                                        },
                transaction: tw.Transaction);

        }

        internal async Task DeleteQuestion(int questionID, TransactionWrapper tw)
        {
            await tw.Connection.QueryAsync<QuestionModel>(@"
                                                            Update dbo.questions
                                                            set IsActive = 'False'
                                                            where ID = @questionID
                                                        ",
                                                       new
                                                       {
                                                           questionID = questionID,

                                                       },
               transaction: tw.Transaction);
        }

        internal async Task DeleteAnswer(int answerID, TransactionWrapper tw)
        {
            await tw.Connection.QueryAsync<QuestionModel>(@"
                                                            Update dbo.answers
                                                            set IsActive = 'False'
                                                            where ID = @answerID
                                                        ",
                                                       new
                                                       {
                                                           answerID = answerID,

                                                       },
               transaction: tw.Transaction);
        }

        internal async Task EditQuestion(string question, int questionID, TransactionWrapper tw)
        {

            await tw.Connection.QueryAsync<QuestionModel>(@"
                                                            Update dbo.questions
                                                            set question = @question
                                                            where ID = @questionID
                                                        ",
                                                        new
                                                        {
                                                            questionID = questionID,
                                                            question = question
                                                        },
                transaction: tw.Transaction);
        }



        internal async Task<AnswerModel> AddAnswer(int questionID, string answer, bool? isTrue, TransactionWrapper tw)
        {
            var inserted = await tw.Connection.QueryFirstAsync<AnswerModel>(@"
                                                                                DECLARE @id int; 

                                                                                INSERT INTO [dbo].[Answers] 
                                                                                            (
                                                                                                [QuestionID],
                                                                                                [Answer],
                                                                                                [IsTrue]
                                                                                             ) 
                                                                                output      inserted.* 
                                                                                VALUES      (@questionID,
                                                                                             @answer, 
                                                                                             @isTrue
                                                                                            ) 

                                                                                SET @id = Scope_identity(); 

                                                                                SELECT @id; 
                                                                              ", new
            {
                questionID,
                answer,
                isTrue

            }, transaction: tw.Transaction);

            return inserted;
        }

        public async Task<QuestionModel> AddQuestion(string question, int creatorUserID, TransactionWrapper tw)
        {
            var inserted = await tw.Connection.QueryFirstAsync<QuestionModel>(@"
                                                                                DECLARE @id int; 

                                                                                INSERT INTO [dbo].[Questions] 
                                                                                            (
                                                                                                [Question],
                                                                                                [CreatorUserID]
                                                                                             ) 
                                                                                output      inserted.* 
                                                                                VALUES      (@question, 
                                                                                             @creatorUserID
                                                                                            ) 

                                                                                SET @id = Scope_identity(); 

                                                                                SELECT @id; 
                                                                              ", new
            {
                question,
                creatorUserID

            }, transaction: tw.Transaction);

            return inserted;
        }
    }
}
