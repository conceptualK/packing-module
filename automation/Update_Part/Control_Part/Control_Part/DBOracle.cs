using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace Control_Part
{
    public static class DBOracle
    {
        private static readonly string connectionString = "Data Source=10.19.150.15:1521/xe;User Id=SYSTEM;Password=1234;persist security info=false;Connection Timeout=120;";


        public static DataTable Select(string query)
        {
            DataTable dt = new DataTable();

            using (var connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (var cmd = new OracleCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return dt;
        }

        public static void Execute(string query)
        {
            using (var connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (var cmd = new OracleCommand(query, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
