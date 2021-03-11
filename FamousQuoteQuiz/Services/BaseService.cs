using CoreLib.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamousQuoteQuiz.Services
{
    public class BaseService
    {
        protected readonly IConfiguration _configuration;
        protected readonly string connectionString;

        protected static HttpClient httpClient = new HttpClient();


        public BaseService(IConfiguration configuration)
        {
            this._configuration = configuration;
            connectionString = _configuration.GetSection("ConnectionStrings")["Prod"];

        }

        public SqlConnection Connection
        {
            get
            {
                string connectionString = _configuration.GetSection("ConnectionStrings")["Prod"];

                return new SqlConnection(connectionString);
            }
        }

        public TransactionWrapper GetTransactionWrapper(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, SelectHint? hint = null)
        {
            var connection = this.Connection;

            var conn = new SqlConnection(connectionString);

            conn.Open();

            connection.Open();

            return new TransactionWrapper
            {
                Connection = conn,
                Transaction = conn.BeginTransaction(isolationLevel),
                Hint = hint
            };
        }
        public TransactionWrapper GetTransactionWrapperWithoutTransaction()
        {
            var connection = this.Connection;

            return new TransactionWrapper
            {
                Connection = connection,
                Transaction = null
            };
        }
    }
}
