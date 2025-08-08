using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS.User_Features
{
    public class RegisterUser
    {
        public static void registerUser()
        {
            try
            {
                Console.WriteLine("=== Registration Process ===");
                string name, email, phone, password, dobStr, gender;
                DateTime dob;

                Console.Write("\n\n---> Type 'back' to cancel or Proceed <---\n\nName: ");
                name = Console.ReadLine();
                if (name?.Trim().ToLower() == "back") return;


                // Email validation
                while (true)
                {
                    Console.Write("\nEmail: ");
                    email = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(email) && email.Contains("@"))
                        break;
                    Console.WriteLine("Invalid email. Please include '@' in the email address.");
                }

                // Phone number validation (10 digits)
                while (true)
                {
                    Console.Write("\nPhone (10 digits): ");
                    phone = Console.ReadLine();
                    if (phone.All(char.IsDigit) && phone.Length == 10)
                        break;
                    Console.WriteLine("Invalid phone number. Must be exactly 10 digits.");
                }

                // Password validation
                while (true)
                {
                    Console.Write("\nPassword (min 6 characters): ");
                    password = Console.ReadLine();
                    if (!string.IsNullOrEmpty(password) && password.Length >= 6)
                        break;
                    Console.WriteLine("Password must be at least 6 characters long.");
                }

                // Date of Birth validation
                while (true)
                {
                    Console.Write("\nDate of Birth (YYYY-MM-DD): ");
                    dobStr = Console.ReadLine();
                    if (DateTime.TryParse(dobStr, out dob))
                        break;
                    Console.WriteLine("Invalid date format. Please enter again in YYYY-MM-DD format.");
                }

                // Gender validation
                while (true)
                {
                    Console.Write("\nGender (male/female/other): ");
                    gender = Console.ReadLine()?.Trim().ToLower();
                    if (gender == "male" || gender == "female" || gender == "other")
                        break;
                    Console.WriteLine("Invalid gender. Please enter 'male', 'female', or 'other'.");
                }

                // Final confirmation
                Console.Write("\nDo you want to proceed with registration? (yes/no): ");
                string confirm = Console.ReadLine()?.Trim().ToLower();
                if (confirm != "yes")
                {
                    Console.WriteLine("Registration cancelled by user.");
                    return;
                }

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
                    Console.WriteLine($"{message}");
                }
                else
                {
                    Console.WriteLine("Registration failed. Please try again.");
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 2627:
                        Console.WriteLine("Email already exists.");
                        break;
                    default:
                        Console.WriteLine($"SQL Error ({ex.Number}): {ex.Message}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

    }
}
