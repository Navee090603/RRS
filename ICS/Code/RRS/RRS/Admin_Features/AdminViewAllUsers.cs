using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS.Admin_Features
{
    public class AdminViewAllUsers
    {
        public static void adminViewAllUsers()
        {
            try
            {
                var dt = DataAccess.Instance.ExecuteTable("select user_id, name, email, phone, user_type, is_active from users");
                string separator = new string('-', 115);

                Console.WriteLine(separator);
                Console.WriteLine(
                    $"{ "ID",-5} { "Name",-20} { "Email",-35} { "Phone",-15} { "Type",-10} { "Active",-7}"
                );
                Console.WriteLine(separator);

                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine(
                        $"{ row["user_id"],-5} { row["name"],-20} { row["email"],-35} { row["phone"],-15} { row["user_type"],-10} { row["is_active"],-7}"
                    );
                }

                Console.WriteLine(separator);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
