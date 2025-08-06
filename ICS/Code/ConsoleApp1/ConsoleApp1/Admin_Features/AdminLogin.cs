using System;
using System.Data;
using ConsoleApp1.Features;
using System.Data.SqlClient;

namespace ConsoleApp1.User_Features
{
    public class AdminLogin
    {
        public static int? LoggedInAdminId { get; set; }
        public static string LoggedInAdminName { get; set; }

        public static void adminLogin()
        {
            try
            {
                Console.Write("\n\nAdmin Email: ");
                string email = Console.ReadLine();

                Console.Write("Password: ");
                string password = Console.ReadLine();

                var dt = DataAccess.Instance.ExecuteTable("sp_loginuser",
                    new SqlParameter("@email", email),
                    new SqlParameter("@password", password)
                );

                if (dt.Rows.Count > 0)
                {
                    int success = Convert.ToInt32(dt.Rows[0]["success"]);
                    string message = dt.Rows[0]["message"].ToString();
                    string userType = dt.Rows[0]["user_type"].ToString().ToLower();

                    if (success == 1 && userType == "admin")
                    {
                        LoggedInAdminId = Convert.ToInt32(dt.Rows[0]["user_id"]);
                        LoggedInAdminName = dt.Rows[0]["name"].ToString();

                        Console.WriteLine($"Admin login successful. Welcome, {LoggedInAdminName}!");
                    }
                    else if(success == 1 && userType == "customer")
                    {
                        Console.WriteLine("You are a customer. Please use the User Login option.");
                        LoggedInAdminId = null;
                        LoggedInAdminName = null;
                    }
                    else
                    {
                        Console.WriteLine($"Login failed: {message}");
                        LoggedInAdminId = null;
                        LoggedInAdminName = null;
                    }
                }
                else
                {
                    Console.WriteLine("Login failed. Please check your credentials.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        

    }
}




