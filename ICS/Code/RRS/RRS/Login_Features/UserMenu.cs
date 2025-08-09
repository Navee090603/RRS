using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS.Login_Features
{
    public class UserMenu
    {
        public static void userMenu()
        {
            while (Login.LoggedInUserId != null)
            {
                Console.Clear(); // Clear screen for user menu
                Console.WriteLine("\n--- User Menu ---");
                Console.WriteLine("\n1. Book Ticket");
                Console.WriteLine("2. View My Bookings");
                Console.WriteLine("3. Download My Ticket");
                Console.WriteLine("4. Cancel Ticket");
                Console.WriteLine("5. Logout");
                Console.Write("\nSelect option: ");
                string opt = Console.ReadLine();

                switch (opt)
                {
                    case "1":
                        Factory.All_Factory.Create("bookticket").Execute();
                        PauseAndClear();
                        break;
                    case "2":
                        Factory.All_Factory.Create("viewbookings").Execute();
                        PauseAndClear();
                        break;
                    case "3":
                        Factory.All_Factory.Create("downloadticket").Execute();
                        PauseAndClear();
                        break;
                    case "4":
                        Factory.All_Factory.Create("cancelbooking").Execute();
                        PauseAndClear();
                        break;
                    case "5":
                        Login.LoggedInUserId = null;
                        Login.LoggedInUserName = null;
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
