using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System_Mart
{
    public sealed class DataBaseConnection
    {
        private static readonly DataBaseConnection instance = new DataBaseConnection();
        private SqlConnection connection;

        string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=MartDB;Integrated Security=True";

        private DataBaseConnection() 
        {
            connection = new SqlConnection(connectionString);
        }

        //Global access point
        public static DataBaseConnection Instance
        {
            get {
                return instance;
            }
        }

        public SqlConnection GetConnection() 
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
            return connection;
        }
    }
}
