using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamousQuoteQuiz.Models
{
    public class QuestionAndAnswers
    {
        public QuestionModel questionModel { get; set; }

        public List<AnswerModel> answers { get; set; }
    }

    public class QuestionModel
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public bool IsActive { get; set; }

    }

    public class AnswerModel
    {
        public int ID { get; set; }
        public int QuestionID { get; set; }
        public bool IsTrue { get; set; }
        public string Answer { get; set; }
        public bool IsActive { get; set; }

    }

    public class QuestionList
    {
        public int QuestionID { get; set; }
        public string Question { get; set; }
    }
    public class AnswerList
    {
        public int AnswernID { get; set; }
        public string Answer { get; set; }
    }

    public class CreateQuote
    {
        public string Quote { get; set; }
    }

    public class EditQuote
    {
        public string Quote { get; set; }
        public int QuoteID { get; set; }
    }
    public class AddAnswerModel
    {
        public int QuestionID { get; set; }
        public string Answer { get; set; }
        public bool IsTrue { get; set; }
    }
    public class EditAnswerModel
    {
        public int AnswerID { get; set; }
        public string Answer { get; set; }
        public bool IsTrue { get; set; }

    }
    public class DeleteQuoteModel
    {
        public int QuoteID { get; set; }
    }
    public class DeleteAnswerModel
    {
        public int AnswerID { get; set; }
    }

    public class UserQuestionAnswerModel
    {
        public int QuestionID { get; set; }
        public int AnswerID { get; set; }
        public bool isBinary { get; set; }
        public bool? isUserChoiceTrue { get; set; }
    }

}
