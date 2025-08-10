using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace RRS.Admin_Features
{
    public class AdminSetUserActive 
    {
        public static void adminSetUserActive(int? loggedInAdminId = null)
        {
            try
            {
                // Display all users
                AdminViewAllUsers.adminViewAllUsers();
                Console.WriteLine();

                // valid User ID
                int userId;
                while (true)
                {
                    Console.WriteLine();
                    Console.Write("Enter User ID: ");
                    string input = Console.ReadLine();
                    if (int.TryParse(input, out userId))
                        break;

                    Console.WriteLine("Invalid input. Please enter a numeric User ID.");
                }

                Console.WriteLine();
                Console.Write("Activate? (yes/no) or (c)ancel: ");
                string yn = Console.ReadLine().Trim().ToLower();

                if (yn == "c" || yn == "cancel")
                {
                    Console.WriteLine("Operation cancelled by admin.");
                    return;
                }

                int active;
                if (yn == "yes" || yn == "y")
                {
                    active = 1;
                }
                else if (yn == "no" || yn == "n")
                {
                    active = 0;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Operation aborted.");
                    return;
                }

                // Prevent admin from deactivating themselves
                int actualAdminId = loggedInAdminId ?? 1;
                if (active == 0 && userId == actualAdminId)
                {
                    Console.WriteLine("Error: You cannot deactivate your own admin account.");
                    return;
                }

                // Call stored procedure to update user status
                var dt = DataAccess.Instance.ExecuteTable("sp_setuseractive",
                    new SqlParameter("@userid", userId),
                    new SqlParameter("@active", active),
                    new SqlParameter("@admin_id", actualAdminId)
                );

                // Display result 
                if (dt.Rows.Count > 0)
                {
                    string name = dt.Rows[0]["name"].ToString();
                    string userIdStr = dt.Rows[0]["user_id"].ToString();
                    string status = (active == 1 ? "Active" : "Inactive");

                    string separator = new string('-', 45);

                    Console.WriteLine();
                    Console.WriteLine(separator);
                    Console.WriteLine($"{ "User ID",-10} { "Name",-20} { "Status",-10}");
                    Console.WriteLine(separator);
                    Console.WriteLine($"{ userIdStr,-10} { name,-20} { status,-10}");
                    Console.WriteLine(separator);
                }
                else
                {
                    Console.WriteLine("User not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
