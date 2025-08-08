using ConsoleApp1.Features;
using ConsoleApp1.User_Features;
using ConsoleApp1.Admin_Features;
using ConsoleApp1.User_Feature;
using System;

namespace ConsoleApp1
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
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("Select option: ");
                string opt = Console.ReadLine();
                Console.Clear();

                if (opt == "1")
                {
                    FeatureFactory.Create("register").Execute();
                    PauseAndClear();
                }
                else if (opt == "2")
                {
                    // Ask user type
                    Console.Write("Are you logging in as User or Admin? (user/admin): ");
                    string userType = Console.ReadLine().Trim().ToLower();

                    if (userType == "admin")
                    {
                        FeatureFactory.Create("adminLogin").Execute();

                        //AdminMenu.adminMenu();
                    }
                    else
                    {
                        FeatureFactory.Create("login").Execute();
                        if (LoginUser.LoggedInUserId == null)
                        {
                            Console.WriteLine("Login failed.");
                            PauseAndClear();
                        }
                        else
                        {
                            UserMenu.userMenu();
                        }
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





        // Helper method for pause and clear

    }
}
