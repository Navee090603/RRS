using System;
using System.Data;
using System.Data.SqlClient;
using ConsoleApp1;
using ConsoleApp1.User_Feature;

namespace ConsoleApp1.User_Features
{
    public class LoginUser
    {
        public static int? LoggedInUserId { get; set; }
        public static string LoggedInUserName { get; set; }

        public static void loginUser()
        {
            try
            {
                Console.Write("Email: ");
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

                    if (success == 1 && userType == "customer")
                    {
                        LoggedInUserId = Convert.ToInt32(dt.Rows[0]["user_id"]);
                        LoggedInUserName = dt.Rows[0]["name"].ToString();

                        //loggedInUserId in User_Feature classes
                        BookTicket.loggedInUserId = LoggedInUserId;
                        ViewBookings.loggedInUserId = LoggedInUserId;
                        CancelBooking.loggedInUserId = LoggedInUserId;

                        Console.WriteLine($"Login successful. Welcome, {LoggedInUserName}!");
                    }
                    else if (success == 1 && userType == "admin")
                    {
                        Console.WriteLine("You are an admin. Please use the Admin Login option.");
                    }
                    else
                    {
                        Console.WriteLine($"Login failed: {message}");
                        LoggedInUserId = null;
                        LoggedInUserName = null;
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