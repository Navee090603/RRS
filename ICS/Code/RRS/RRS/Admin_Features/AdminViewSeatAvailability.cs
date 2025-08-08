using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS.Admin_Features
{
    public class AdminViewSeatAvailability
    {
        public static void adminViewSeatAvailability()
        {
            AdminViewTrains.adminViewTrains();
            Console.WriteLine();

            int trainId;
            while (true)
            {
                Console.Write("Enter Train ID: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out trainId))
                    break;
                Console.Write("Invalid Train ID. \nPlease enter a valid number : ");
            }

            DateTime journeyDate;
            while (true)
            {
                Console.Write("Enter Journey Date (YYYY-MM-DD): ");
                string dateInput = Console.ReadLine();
                if (DateTime.TryParse(dateInput, out journeyDate))
                    break;
                Console.Write("Invalid Date Format. \nPlease use YYYY-MM-DD : ");
            }

            Console.WriteLine();

            try
            {
                var dt = DataAccess.Instance.ExecuteTable(
                    "select sa.train_id, sa.journey_date, sa.sleeper_available, sa.ac3_available, sa.ac2_available " +
                    "from seat_availability sa " +
                    "where sa.train_id=@tid and sa.journey_date=@jdate",
                    new SqlParameter("@tid", trainId),
                    new SqlParameter("@jdate", journeyDate)
                );

                if (dt.Rows.Count == 0)
                {
                    Console.WriteLine("No seat availability record found for this train and date.");
                    return;
                }

                Console.Clear();

                Console.WriteLine(new string('-', 80));
                Console.WriteLine(
                    $"{ "Train ID",-10} { "Date",-15} { "Sleeper",-10} { "AC 3 Tier",-10} { "AC 2 Tier",-10}");
                Console.WriteLine(new string('-', 80));

                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine(
                        $"{row["train_id"],-10} " +
                        $"{Convert.ToDateTime(row["journey_date"]).ToString("yyyy-MM-dd"),-15} " +
                        $"{row["sleeper_available"],-10} " +
                        $"{row["ac3_available"],-10} " +
                        $"{row["ac2_available"],-10}");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
