using System;
using ConsoleApp1.User_Features;
using ConsoleApp1.User_Feature;
using System.Data.SqlClient;

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
                Console.WriteLine("2. User Login");
                Console.WriteLine("3. Admin Login");
                Console.WriteLine("4. Book Ticket (requires login)");
                Console.WriteLine("5. View My Bookings (requires login)");
                Console.WriteLine("6. Cancel Booking (requires login)");
                Console.WriteLine("7. Exit");
                Console.Write("Select option: ");
                string opt = Console.ReadLine();

                try
                {
                    switch (opt)
                    {
                        case "1":
                            RegisterUser.registerUser();
                            break;
                        case "2":
                            LoginUser.loginUser();
                            break;
                        case "3":
                            AdminLogin.adminLogin();
                            break;
                        case "4":
                            BookTicket.bookTicket();
                            break;
                        case "5":
                            ViewBookings.viewBookings();
                            break;
                        case "6":
                            CancelBooking.cancelBooking();
                            break;
                        case "7":
                            return;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input format. Please try again.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Error: {ex.Message}");
                }
            }
        }
    }
}