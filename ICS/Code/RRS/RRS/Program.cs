using System;
using RRS.Login_Features;
using RRS.Factory;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; //For the rupee symbol 

            while (true)
            {
                Console.Clear(); // Clear the console at the start of each main loop
                Console.WriteLine("\n=== Railway Reservation System ===");
                Console.WriteLine("\n1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("\nSelect option: ");
                string opt = Console.ReadLine();
                Console.Clear();

                if (opt == "1")
                {
                    All_Factory.Create("register").Execute();
                    PauseAndClear();
                }
                else if (opt == "2")
                {
                    // Ask user type
                    Console.Write("Are you logging in as User or Admin? (user/admin): ");
                    string userType = Console.ReadLine().Trim().ToLower();

                    if (userType == "admin")
                    {
                        All_Factory.Create("adminlogin").Execute();

                        if (AdminLogin.LoggedInAdminId == null)
                        {
                            Console.WriteLine("Login failed or not an admin.");
                            PauseAndClear();
                        }
                        else
                        {
                            All_Factory.Create("adminmenu").Execute();
                        }
                    }
                    else if (userType == "user")
                    {
                        All_Factory.Create("login").Execute();

                        if (Login.LoggedInUserId == null)
                        {
                            Console.WriteLine("Login failed.");
                            PauseAndClear();
                        }
                        else
                        {
                            All_Factory.Create("usermenu").Execute();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Enter user or Admin...");
                        PauseAndClear();
                    }
                }
                else if (opt == "3")
                {
                    
                    Console.WriteLine("Exiting system...");
                    PauseAndClear();
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid option.");
                    PauseAndClear();
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
