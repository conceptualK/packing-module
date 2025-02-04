using System;
using System.Data;
using Npgsql;

namespace Control_ERP
{
    public static class DBConnect
    {
        private static readonly string connectionString = "Host=10.19.5.107;Port=5432;Username=mes_admin;Password=amt@mes;Database=mesdb";


        public static DataTable Select(string query)
        {
            DataTable dt = new DataTable();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand(query, connection))
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
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand(query, connection))
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
