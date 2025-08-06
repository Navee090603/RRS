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
                Console.Write("Admin Email: ");
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

        public static void AdminMenu()
        {
            while (LoggedInAdminId != null)
            {
                Console.Clear();
                Console.WriteLine("\n--- Admin Menu ---");
                Console.WriteLine("1. View All Users");
                Console.WriteLine("2. Activate/Deactivate User");
                Console.WriteLine("3. Add Train");
                Console.WriteLine("4. View All Trains");
                Console.WriteLine("5. View All Bookings");
                Console.WriteLine("6. View All Payments");
                Console.WriteLine("7. View Seat Availability");
                Console.WriteLine("8. View All Stations");
                Console.WriteLine("9. Logout");
                Console.Write("Select option: ");
                string opt = Console.ReadLine();

                switch (opt)
                {
                    case "1": ConsoleHelper.ExecuteFeature("viewallusers"); break;
                    case "2": ConsoleHelper.ExecuteFeature("setuseractive"); break;
                    case "3": ConsoleHelper.ExecuteFeature("addtrain"); break;
                    case "4": ConsoleHelper.ExecuteFeature("viewalltrains"); break;
                    case "5": ConsoleHelper.ExecuteFeature("viewallbookings"); break;
                    case "6": ConsoleHelper.ExecuteFeature("viewallpayments"); break;
                    case "7": ConsoleHelper.ExecuteFeature("viewseatavailability"); break;
                    case "8": ConsoleHelper.ExecuteFeature("viewstations"); break;
                    case "9":
                        LoggedInAdminId = null;
                        LoggedInAdminName = null;
                        Console.WriteLine("Logged out.");
                        ConsoleHelper.PauseAndClear();
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        ConsoleHelper.PauseAndClear();
                        break;
                }
            }
        }

    }
}




