using ConsoleApp1.Features;
using ConsoleApp1.User_Features;
using ConsoleApp1.User_Feature;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n=== Railway Reservation System ===");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("Select option: ");
                string opt = Console.ReadLine();

                if (opt == "1")
                {
                    FeatureFactory.Create("register").Execute();
                }
                else if (opt == "2")
                {
                    // Ask user type
                    Console.Write("Are you logging in as User or Admin? (user/admin): ");
                    string userType = Console.ReadLine().Trim().ToLower();

                    if (userType == "admin")
                    {
                        FeatureFactory.Create("adminlogin").Execute();
                        AdminMenu();
                    }
                    else
                    {
                        FeatureFactory.Create("login").Execute();
                        UserMenu();
                    }
                }
                else if (opt == "3")
                {
                    Console.WriteLine("Exiting system...");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid option.");
                }
            }
        }

        static void UserMenu()
        {
            while (LoginUser.LoggedInUserId != null)
            {
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
                    case "1": FeatureFactory.Create("bookticket").Execute(); break;
                    case "2": FeatureFactory.Create("viewbookings").Execute(); break;
                    case "3": FeatureFactory.Create("cancelbooking").Execute(); break;
                    case "4": FeatureFactory.Create("viewstations").Execute(); break;
                    case "5":
                        LoginUser.LoggedInUserId = null;
                        LoginUser.LoggedInUserName = null;
                        Console.WriteLine("Logged out.");
                        break;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }

        static void AdminMenu()
        {
            while (AdminLogin.LoggedInAdminId != null)
            {
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
                    case "1": FeatureFactory.Create("viewallusers").Execute(); break;
                    case "2": FeatureFactory.Create("setuseractive").Execute(); break;
                    case "3": FeatureFactory.Create("addtrain").Execute(); break;
                    case "4": FeatureFactory.Create("viewalltrains").Execute(); break;
                    case "5": FeatureFactory.Create("viewallbookings").Execute(); break;
                    case "6": FeatureFactory.Create("viewallpayments").Execute(); break;
                    case "7": FeatureFactory.Create("viewseatavailability").Execute(); break;
                    case "8": FeatureFactory.Create("viewstations").Execute(); break;
                    case "9":
                        AdminLogin.LoggedInAdminId = null;
                        AdminLogin.LoggedInAdminName = null;
                        Console.WriteLine("Logged out.");
                        break;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }
    }
}