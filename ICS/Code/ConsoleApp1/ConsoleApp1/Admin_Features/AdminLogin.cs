using System;
using System.Data;
using System.Data.SqlClient;

namespace ConsoleApp1.User_Features
{
    public class AdminLogin
    {
        public static int? LoggedInAdminId { get; private set; }
        public static string LoggedInAdminName { get; private set; }

        private static int? loggedInAdminId = null;
        private static string loggedInAdminName = null;
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
                        loggedInAdminId = LoggedInAdminId;
                        loggedInAdminName = LoggedInAdminName;
                        Console.WriteLine($"Admin login successful. Welcome, {LoggedInAdminName}!");
                        AdminMenu();
                    }
                    else
                    {
                        Console.WriteLine("Login failed or not an admin.");
                        LoggedInAdminId = null;
                        LoggedInAdminName = null;
                        loggedInAdminId = null;
                        loggedInAdminName = null;
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

        private static void AdminMenu()
        {
            while (loggedInAdminId != null)
            {
                Console.WriteLine("\n=== Admin Menu ===");
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
                try
                {
                    switch (opt)
                    {
                        case "1": AdminViewAllUsers.adminViewAllUsers(); break;
                        case "2": AdminSetUserActive.adminSetUserActive(loggedInAdminId); break;
                        case "3": AdminAddTrain.adminAddTrain(); break;
                        case "4": AdminViewTrains.adminViewTrains(); break;
                        case "5": AdminViewAllBookings.adminViewAllBookings(); break;
                        case "6": AdminViewAllPayments.adminViewAllPayments(); break;
                        case "7": AdminViewSeatAvailability.adminViewSeatAvailability(); break;
                        case "8": AdminViewStations.adminViewStations(); break;
                        case "9":
                            loggedInAdminId = null;
                            loggedInAdminName = null;
                            Console.WriteLine("Logged out from admin mode.");
                            break;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
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
