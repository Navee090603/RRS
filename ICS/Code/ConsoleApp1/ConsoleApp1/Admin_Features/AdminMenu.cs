using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.User_Features;

namespace ConsoleApp1.Admin_Features
{
    public class AdminMenu
    {
        public static void adminMenu()
        {
            while (AdminLogin.LoggedInAdminId != null)
            {
                Console.Clear(); // Clear screen for admin menu
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
                Console.Clear();

                switch (opt)
                {
                    case "1":
                        FeatureFactory.Create("viewallusers").Execute();
                        PauseAndClear();
                        break;
                    case "2":
                        FeatureFactory.Create("setuseractive").Execute();
                        PauseAndClear();
                        break;
                    case "3":
                        FeatureFactory.Create("addtrain").Execute();
                        PauseAndClear();
                        break;
                    case "4":
                        FeatureFactory.Create("viewalltrains").Execute();
                        PauseAndClear();
                        break;
                    case "5":
                        FeatureFactory.Create("viewallbookings").Execute();
                        PauseAndClear();
                        break;
                    case "6":
                        FeatureFactory.Create("viewallpayments").Execute();
                        PauseAndClear();
                        break;
                    case "7":
                        FeatureFactory.Create("viewseatavailability").Execute();
                        PauseAndClear();
                        break;
                    case "8":
                        FeatureFactory.Create("viewstations").Execute();
                        PauseAndClear();
                        break;
                    case "9":
                        AdminLogin.LoggedInAdminId = null;
                        AdminLogin.LoggedInAdminName = null;
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

        static void PauseAndClear()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            Console.Clear();
        }
    }
}
