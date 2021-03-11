using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamousQuoteQuiz.Models
{
    public class UpdateUserPasswordModel
    {
        public string Password { get; set; }
    }
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }

    }

    public class UserQuestionAnswerHistory
    {
        public int UserID { get; set; }
        public int QuestionID { get; set; }
        public int AnswerID { get; set; }
        public bool answeredTrue { get; set; }
        public DateTime CreateDate { get; set; }
        public bool isBinary { get; set; }
    }
}
