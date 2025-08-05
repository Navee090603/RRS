using System;
using System.Data;
using System.Data.SqlClient;
using ConsoleApp1;

namespace ConsoleApp1.User_Features
{
    public class AdminSetUserActive
    {
        public static void adminSetUserActive(int? loggedInAdminId = null)
        {
            try
            {
                Console.Write("Enter User ID: ");
                int userId = int.Parse(Console.ReadLine());

                Console.Write("Activate? (yes/no): ");
                string yn = Console.ReadLine().Trim().ToLower();
                int active = (yn == "yes" || yn == "y") ? 1 : 0;

                var dt = DataAccess.Instance.ExecuteTable("sp_setuseractive",
                    new SqlParameter("@userid", userId),
                    new SqlParameter("@active", active),
                    new SqlParameter("@admin_id", loggedInAdminId ?? 1)
                );

                if (dt.Rows.Count > 0)
                {
                    Console.WriteLine($"User {dt.Rows[0]["name"]} (ID: {dt.Rows[0]["user_id"]}) is now {(active == 1 ? "Active" : "Inactive")}");
                }
                else
                {
                    Console.WriteLine("User not found.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input format. Please enter a valid User ID.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
