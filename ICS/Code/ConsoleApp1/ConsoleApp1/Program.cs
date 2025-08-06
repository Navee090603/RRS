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
                    ExecuteFeature("register");
                }
                else if (opt == "2")
                {
                    Console.Write("Are you logging in as User or Admin? (user/admin): ");
                    string userType = Console.ReadLine().Trim().ToLower();

                    bool loginSuccess = false;

                    if (userType == "admin")
                    {
                        ExecuteFeature("adminlogin");
                        loginSuccess = AdminLogin.LoggedInAdminId != null;
                        if (loginSuccess)
                        {
                            User_Features.AdminLogin.AdminMenu();
                        }
                    }
                    else if (userType == "user")
                    {
                        ExecuteFeature("login");
                        loginSuccess = LoginUser.LoggedInUserId != null;
                        if (loginSuccess)
                        {
                            User_Features.LoginUser.UserMenu();
                        }
                    }

                    if (!loginSuccess)
                    {
                        Console.WriteLine("Login failed.");
                        PauseAndClear();
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

        // Helper method to execute features with screen clear
        static void ExecuteFeature(string featureKey)
        {
            Console.Clear();
            FeatureFactory.Create(featureKey).Execute();
            PauseAndClear();
        }

        public static void PauseAndClear()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            Console.Clear();
        }
    }
}
