using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS.Login_Features
{
    public class AdminLogin
    {
        public static int? LoggedInAdminId { get; set; }
        public static string LoggedInAdminName { get; set; }

        public static void adminLogin()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("==== Welcome To Railways Admin ====");

                Console.Write("\n\nAdmin Email: ");
                string email = Console.ReadLine();

                Console.Write("Password: ");
                string password = Console.ReadLine();

                var dt = DataAccess.Instance.ExecuteTable(
                    "sp_loginuser",
                    new SqlParameter("@email", email),
                    new SqlParameter("@password", password)
                );

                if (dt.Rows.Count > 0)
                {
                    int success = Convert.ToInt32(dt.Rows[0]["success"]);
                    string userType = dt.Rows[0]["user_type"].ToString().ToLower();

                    if (success == 1 && userType == "admin")
                    {
                        LoggedInAdminId = Convert.ToInt32(dt.Rows[0]["user_id"]);
                        LoggedInAdminName = dt.Rows[0]["name"].ToString();

                        Console.WriteLine($"\nAdmin login successful. Welcome, {LoggedInAdminName}!");
                        return; // Program.cs will route to AdminMenu
                    }
                }

                // Any failure: just clear identity here; no messages (Program.cs will show one)
                LoggedInAdminId = null;
                LoggedInAdminName = null;
                Console.Clear();
            }
            catch
            {
                // On exception, also clear and stay silent
                LoggedInAdminId = null;
                LoggedInAdminName = null;
            }
        }
    }
}
