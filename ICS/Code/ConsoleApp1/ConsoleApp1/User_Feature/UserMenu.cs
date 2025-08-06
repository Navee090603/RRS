using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.User_Features;

namespace ConsoleApp1.User_Feature
{
    public class UserMenu
    {
        public static void userMenu()
        {
            while (LoginUser.LoggedInUserId != null)
            {
                Console.Clear(); // Clear screen for user menu
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
                    case "1":
                        FeatureFactory.Create("bookticket").Execute();
                        PauseAndClear();
                        break;
                    case "2":
                        FeatureFactory.Create("viewbookings").Execute();
                        PauseAndClear();
                        break;
                    case "3":
                        FeatureFactory.Create("cancelbooking").Execute();
                        PauseAndClear();
                        break;
                    case "4":
                        FeatureFactory.Create("viewstations").Execute();
                        PauseAndClear();
                        break;
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

        static void PauseAndClear()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            Console.Clear();
        }
    }
}
