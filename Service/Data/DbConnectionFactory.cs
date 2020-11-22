using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ServiceIntegracao.Data
{
    public class DbConnectionFactory
    {
        private readonly string connectionString;

        public DbConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public SqlConnection Create()
        {
            var connection = new SqlConnection(this.connectionString);
            connection.Open();
            return connection;
        }

        public static DbConnectionFactory Create(string connectionString)
        {
            return new DbConnectionFactory(connectionString);
        }
    }
}
