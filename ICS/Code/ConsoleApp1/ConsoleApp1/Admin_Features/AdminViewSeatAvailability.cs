using System;
using System.Data;
using System.Data.SqlClient;
using ConsoleApp1;

namespace ConsoleApp1.User_Features
{
    public class AdminViewSeatAvailability
    {
        public static void adminViewSeatAvailability()
        {
            try
            {
                Console.Write("Enter Train ID: ");
                int trainId = int.Parse(Console.ReadLine());
                Console.Write("Enter Journey Date (YYYY-MM-DD): ");
                DateTime journeyDate = DateTime.Parse(Console.ReadLine());

                var dt = DataAccess.Instance.ExecuteTable(
                    @"select sa.train_id, sa.journey_date, sa.sleeper_available, sa.ac3_available, sa.ac2_available 
                    from seat_availability sa 
                    where sa.train_id=@tid and sa.journey_date=@jdate",
                    new SqlParameter("@tid", trainId),
                    new SqlParameter("@jdate", journeyDate)
                );
                foreach (DataRow row in dt.Rows)
                    Console.WriteLine($"Train:{row["train_id"]}, Date:{row["journey_date"]}, Sleeper:{row["sleeper_available"]}, AC3:{row["ac3_available"]}, AC2:{row["ac2_available"]}");
                if (dt.Rows.Count == 0)
                    Console.WriteLine("No seat availability record found for this train and date.");
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input format. Please enter valid Train ID and Date.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
