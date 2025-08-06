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
                Console.Clear();
                Console.WriteLine("\n=== Railway Reservation System ===");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("Select option: ");
                string opt = Console.ReadLine();

                Console.Clear();

                if (opt == "1")
                {
                    ConsoleHelper.ExecuteFeature("register");
                    ConsoleHelper.PauseAndClear();
                }
                else if (opt == "2")
                {
                    while (true)
                    {
                        Console.Write("Are you logging in as User or Admin? (user/admin) or type 'back' to return: ");
                        string userType = Console.ReadLine().Trim().ToLower();

                        bool loginSuccess = false;

                        if (userType == "admin")
                        {
                            ConsoleHelper.ExecuteFeature("adminlogin");
                            loginSuccess = AdminLogin.LoggedInAdminId != null;
                            if (loginSuccess)
                            {
                                AdminLogin.AdminMenu();
                                break;
                            }
                            else
                            {
                                ConsoleHelper.PauseAndClear(); // already printed failure message
                            }
                        }
                        else if (userType == "user")
                        {
                            ConsoleHelper.ExecuteFeature("login");
                            loginSuccess = LoginUser.LoggedInUserId != null;
                            if (loginSuccess)
                            {
                                LoginUser.UserMenu();
                                break;
                            }
                            else
                            {
                                ConsoleHelper.PauseAndClear();
                            }
                        }
                        else if (userType == "back")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid login type. Please enter 'user' or 'admin'.");
                            ConsoleHelper.PauseAndClear();
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
                    ConsoleHelper.PauseAndClear();
                }
            }
        }
    }
}