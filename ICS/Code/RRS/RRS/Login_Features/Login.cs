using System;
using System.Data.SqlClient;
using RRS.User_Features; //for Booking,viewBooking,CancelBooking
using RRS.Login_Factory;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RRS.Login_Features
{
    public class Login
    {
        public static int? LoggedInUserId { get; set; }
        public static string LoggedInUserName { get; set; }

        public static void login()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("==== Welcome To Railways ====");
                Console.Write("\nEmail: ");
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
                        return;
                        //UserMenu.userMenu();
                    }

                    //else
                    //{
                    //    Console.WriteLine("\n❌ Login failed: Invalid email or password.");
                    //    LoggedInUserId = null;
                    //    LoggedInUserName = null;
                    //}
                }
                else
                {
                    Console.Clear();
                    
                    LoggedInUserId = null;
                    LoggedInUserName = null;
                    Console.WriteLine("Please check your credentials.");
                }
            }
            catch (Exception ex)
            {
                LoggedInUserId = null;
                LoggedInUserName = null;
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
