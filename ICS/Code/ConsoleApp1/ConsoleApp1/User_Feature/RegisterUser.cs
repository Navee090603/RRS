using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using ConsoleApp1;
using System.Data;


namespace ConsoleApp1.User_Feature
{
    public class RegisterUser
    {
        public static void registerUser()
        {
            try
            {
                Console.Write("Name: "); string name = Console.ReadLine();
                Console.Write("Email: "); string email = Console.ReadLine();
                Console.Write("Phone: "); string phone = Console.ReadLine();
                Console.Write("Password: "); string password = Console.ReadLine();
                Console.Write("Date of Birth (YYYY-MM-DD): "); string dobStr = Console.ReadLine();
                Console.Write("Gender (male/female/other): "); string gender = Console.ReadLine();
                DateTime dob;
                if (!DateTime.TryParse(dobStr, out dob))
                    dob = DateTime.Now;
                var dt = DataAccess.Instance.ExecuteTable("sp_registeruser",
                    new SqlParameter("@name", name),
                    new SqlParameter("@email", email),
                    new SqlParameter("@phone", phone),
                    new SqlParameter("@password", password),
                    new SqlParameter("@dateofbirth", dob),
                    new SqlParameter("@gender", gender)
                );
                if (dt.Rows.Count > 0)
                {
                    var success = Convert.ToInt32(dt.Rows[0]["success"]);
                    var message = dt.Rows[0]["message"].ToString();
                    Console.WriteLine(message);
                }
                else
                {
                    Console.WriteLine("Registration failed. Please try again.");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                    Console.WriteLine("Email already exists.");
                else
                    Console.WriteLine($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
