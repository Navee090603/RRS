using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace RRS
{
    public class DataAccess
    {
        private readonly string _connStr;

        // Singleton instance 
        private static readonly Lazy<DataAccess> _instance = new Lazy<DataAccess>(() => new DataAccess());

        public static DataAccess Instance => _instance.Value;

        private DataAccess()
        {
            _connStr = ConfigurationManager.ConnectionStrings["RailwayDb"].ConnectionString;
        }

        // Returns a DataTable from a SQL query or stored procedure
        public DataTable ExecuteTable(string procOrSql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(procOrSql, conn))
            {
                if (procOrSql.Trim().ToLower().StartsWith("select"))
                    cmd.CommandType = CommandType.Text;
                else
                    cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        // Returns a DataSet from a SQL query or stored procedure
        public DataSet ExecuteDataSet(string procOrSql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(procOrSql, conn))
            {
                if (procOrSql.Trim().ToLower().StartsWith("select"))
                    cmd.CommandType = CommandType.Text;
                else
                    cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                return ds;
            }
        }

        // Executes a non-query SQL or stored procedure and returns affected rows
        public int ExecuteNonQuery(string procOrSql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(procOrSql, conn))
            {
                var sql = procOrSql.Trim().ToLower();
                if (sql.StartsWith("insert") || sql.StartsWith("update") || sql.StartsWith("delete"))
                    cmd.CommandType = CommandType.Text;
                else
                    cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // Executes a scalar SQL or stored procedure
        public object ExecuteScalar(string procOrSql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(procOrSql, conn))
            {
                if (procOrSql.Trim().ToLower().StartsWith("select"))
                    cmd.CommandType = CommandType.Text;
                else
                    cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}
