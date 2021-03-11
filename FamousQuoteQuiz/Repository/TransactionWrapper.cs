using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace CoreLib.Repository
{
    public class TransactionWrapper : IDisposable
    {
        public SqlConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }
        public SelectHint? Hint { get; set; }

        public void Commit()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
            }
        }

        public void Dispose()
        {
            if (Transaction != null)
            {
                Transaction.Dispose();
            }

            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }

            Connection.Dispose();

            GC.SuppressFinalize(this);
        }
    }
    public enum SelectHint
    {
        ROWLOCK,
        UPDLOCK,
        ROWLOCK_UPDLOCK
    }
}
