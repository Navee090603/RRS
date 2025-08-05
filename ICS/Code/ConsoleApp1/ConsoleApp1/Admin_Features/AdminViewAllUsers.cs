using System;
using System.Data;
using ConsoleApp1;

namespace ConsoleApp1.User_Features
{
    public class AdminViewAllUsers
    {
        public static void adminViewAllUsers()
        {
            try
            {
                var dt = DataAccess.Instance.ExecuteTable("select user_id, name, email, phone, user_type, is_active from users");

                Console.WriteLine("Users:");
                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"ID:{row["user_id"]}, Name:{row["name"]}, Email:{row["email"]}, Phone:{row["phone"]}, Type:{row["user_type"]}, Active:{row["is_active"]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
