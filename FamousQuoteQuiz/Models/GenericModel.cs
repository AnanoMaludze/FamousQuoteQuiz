using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamousQuoteQuiz.Models
{
    public enum StatusCode
    {
        SUCCESS = 1,

        ERROR = -1,
        USERNAME_ALREADY_EXISTS = -2,
        INCORRECT_USERNAME = -3,
        INCORRECT_PASSWORD = -4,
        PASSWORD_MISSMATCH = -5,
        USERNAME_CANNOT_BE_EMPTY = -6,
        PASSWORD_CANNOT_BE_EMPTY = -7,
        ONE_QUESTION_CAN_ONLY_HAVE_ONE_ANSWER = -8,
        THIS_ANSWER_ALREADY_EXISTS = -9,
        SUCH_QUESTION_DOES_NOT_EXIST = -10
    }
    public class GenericResponse<T>
    {
        private StatusCode status;
        private T response;
        [JsonProperty(PropertyName = "status")]
        public StatusCode Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }
        [JsonProperty(PropertyName = "response")]
        public T Response
        {
            get
            {
                return response;
            }
            set
            {
                response = value;
            }
        }


        public GenericResponse(StatusCode status)
        {
            this.status = status;
        }
        public GenericResponse(StatusCode status, T response)
        {
            this.status = status;
            this.response = response;

        }


    }
}

