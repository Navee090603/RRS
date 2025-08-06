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

        public static void UserMenu()
        {
            while (LoginUser.LoggedInUserId != null)
            {
                Console.Clear();
                Console.WriteLine("\n--- User Menu ---");
                Console.WriteLine("1. Book Ticket");
                Console.WriteLine("2. View My Ticket");
                Console.WriteLine("3. Cancel Ticket");
                Console.WriteLine("4. View All Stations");
                Console.WriteLine("5. Logout");
                Console.Write("Select option: ");
                string opt = Console.ReadLine();

                switch (opt)
                {
                    case "1": ExecuteFeature("bookticket"); break;
                    case "2": ExecuteFeature("viewbookings"); break;
                    case "3": ExecuteFeature("cancelbooking"); break;
                    case "4": ExecuteFeature("viewstations"); break;
                    case "5":
                        LoginUser.LoggedInUserId = null;
                        LoginUser.LoggedInUserName = null;
                        Console.WriteLine("Logged out.");
                        PauseAndClear();
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        PauseAndClear();
                        break;
                }
            }
        }

        // Helper method for pause and clear
        static void PauseAndClear()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            Console.Clear();
        }

        // Helper method to execute features with screen clear
        static void ExecuteFeature(string featureKey)
        {
            Console.Clear();
            FeatureFactory.Create(featureKey).Execute();
            PauseAndClear();
        }
    }
}