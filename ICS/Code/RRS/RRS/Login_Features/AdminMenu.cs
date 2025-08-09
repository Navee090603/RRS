using System;
using RRS.Admin_Features;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS.Login_Features
{
    public class AdminMenu
    {
        public static void adminMenu()
        {
            while (AdminLogin.LoggedInAdminId != null)
            {
                Console.Clear(); 
                Console.WriteLine("\n--- Admin Menu ---");
                Console.WriteLine("\n1. View All Users");
                Console.WriteLine("2. Activate/Deactivate User");
                Console.WriteLine("3. Add Train");
                Console.WriteLine("4. View All Trains");
                Console.WriteLine("5. View All Bookings");
                Console.WriteLine("6. View All Payments");
                Console.WriteLine("7. View Seat Availability");
                Console.WriteLine("8. View All Stations");
                Console.WriteLine("9. View Revenue Report");
                Console.WriteLine("10. View All Passengers");
                Console.WriteLine("11. Logout");
                Console.Write("\nSelect option: ");
                string opt = Console.ReadLine();
                Console.Clear();

                switch (opt)
                {
                    case "1":
                        Factory.All_Factory.Create("viewallusers").Execute();
                        PauseAndClear();
                        break;
                    case "2":
                        Factory.All_Factory.Create("setuseractive").Execute();
                        PauseAndClear();
                        break;
                    case "3":
                        Factory.All_Factory.Create("addtrain").Execute();
                        PauseAndClear();
                        break;
                    case "4":
                        Factory.All_Factory.Create("viewalltrains").Execute();
                        PauseAndClear();
                        break;
                    case "5":
                        Factory.All_Factory.Create("viewallbookings").Execute();
                        PauseAndClear();
                        break;
                    case "6":
                        Factory.All_Factory.Create("viewallpayments").Execute();
                        PauseAndClear();
                        break;
                    case "7":
                        Factory.All_Factory.Create("viewseatavailability").Execute();
                        PauseAndClear();
                        break;
                    case "8":
                        Factory.All_Factory.Create("viewstations").Execute();
                        PauseAndClear();
                        break;
                    case "9":
                        Factory.All_Factory.Create("viewreport").Execute();
                        PauseAndClear();
                        break;
                    case "10":
                        Factory.All_Factory.Create("viewpassengers").Execute();
                        PauseAndClear();
                        break;

                    case "11":
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
