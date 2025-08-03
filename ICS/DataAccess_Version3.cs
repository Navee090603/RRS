using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace RailwayReservationWin
{
    public class DataAccess
    {
        private readonly string _connStr;
        public DataAccess()
        {
            _connStr = ConfigurationManager.ConnectionStrings["RailwayDb"].ConnectionString;
        }

        // For SELECT queries
        public DataTable ExecuteTable(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        // For INSERT/UPDATE/DELETE queries
        public int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }
}