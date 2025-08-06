using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace RRS.Admin_Features
{
    public class AdminAddTrain
    {
        public static void adminAddTrain()
        {
            try
            {
                Console.WriteLine();
                Console.Write("Train Number: ");
                string number = Console.ReadLine();

                Console.WriteLine();
                Console.Write("Train Name: ");
                string name = Console.ReadLine();

                Console.WriteLine();
                AdminViewStations.adminViewStations();

                List<int> validStationIds = GetValidStationIds();

                Console.WriteLine();
                int src;
                while (true)
                {
                    Console.Write("Source Station ID: ");
                    if (int.TryParse(Console.ReadLine(), out src) && validStationIds.Contains(src)) break;
                    Console.WriteLine("Invalid Source Station ID. Please enter a valid ID from the list.");
                }

                Console.WriteLine();
                int dst;
                while (true)
                {
                    Console.Write("Destination Station ID: ");
                    if (int.TryParse(Console.ReadLine(), out dst) && validStationIds.Contains(dst)) break;
                    Console.WriteLine("Invalid Destination Station ID. Please enter a valid ID from the list.");
                }

                Console.WriteLine();
                TimeSpan dep;
                while (true)
                {
                    Console.Write("Departure Time (HH:mm): ");
                    if (TimeSpan.TryParse(Console.ReadLine(), out dep)) break;
                    Console.WriteLine("Invalid time format. Please enter time as HH:mm.");
                }

                Console.WriteLine();
                TimeSpan arr;
                while (true)
                {
                    Console.Write("Arrival Time (HH:mm): ");
                    if (TimeSpan.TryParse(Console.ReadLine(), out arr)) break;
                    Console.WriteLine("Invalid time format. Please enter time as HH:mm.");
                }

                Console.WriteLine();
                string days;
                while (true)
                {
                    Console.Write("Running Days (e.g. 1111100 for Mon-Fri): ");
                    days = Console.ReadLine();
                    if (days.Length == 7 && days.All(c => c == '0' || c == '1')) break;
                    Console.WriteLine("Invalid format. Please enter exactly 7 digits using only 0 or 1.");
                }

                Console.WriteLine();
                int total = ReadInt("Total Seats");

                Console.WriteLine();
                int sleeper = ReadInt("Sleeper Seats");

                Console.WriteLine();
                int ac3 = ReadInt("AC3 Seats");

                Console.WriteLine();
                int ac2 = ReadInt("AC2 Seats");

                Console.WriteLine();
                decimal sleeperFare = ReadDecimal("Sleeper Fare");

                Console.WriteLine();
                decimal ac3Fare = ReadDecimal("AC3 Fare");

                Console.WriteLine();
                decimal ac2Fare = ReadDecimal("AC2 Fare");

                DataAccess.Instance.ExecuteNonQuery(
                    "insert into trains (train_number, train_name, source_station_id, destination_station_id, departure_time, arrival_time, running_days, total_seats, sleeper_seats, ac3_seats, ac2_seats, sleeper_fare, ac3_fare, ac2_fare) " +
                    "values (@tnum, @tname, @src, @dst, @dep, @arr, @days, @total, @sleeper, @ac3, @ac2, @sfare, @ac3fare, @ac2fare)",
                    new SqlParameter("@tnum", number),
                    new SqlParameter("@tname", name),
                    new SqlParameter("@src", src),
                    new SqlParameter("@dst", dst),
                    new SqlParameter("@dep", dep),
                    new SqlParameter("@arr", arr),
                    new SqlParameter("@days", days),
                    new SqlParameter("@total", total),
                    new SqlParameter("@sleeper", sleeper),
                    new SqlParameter("@ac3", ac3),
                    new SqlParameter("@ac2", ac2),
                    new SqlParameter("@sfare", sleeperFare),
                    new SqlParameter("@ac3fare", ac3Fare),
                    new SqlParameter("@ac2fare", ac2Fare)
                );

                // Fetch station names
                string srcName = GetStationNameById(src);
                string dstName = GetStationNameById(dst);

                Console.WriteLine("\n Train added successfully.\n");
                Console.WriteLine("\nAdded Train Details:");
                Console.WriteLine($"\nTrain Number       : {number}");
                Console.WriteLine($"Train Name         : {name}");
                Console.WriteLine($"Source Station     : {srcName} (ID: {src})");
                Console.WriteLine($"Destination Station: {dstName} (ID: {dst})");
                Console.WriteLine($"Departure Time     : {dep:hh\\:mm}");
                Console.WriteLine($"Arrival Time       : {arr:hh\\:mm}");
                Console.WriteLine($"Running Days       : {days}  (Mon-Sun as binary)");
                Console.WriteLine($"Total Seats        : {total}");
                Console.WriteLine($"Sleeper Seats      : {sleeper} (Fare: ₹{sleeperFare})");
                Console.WriteLine($"AC3 Seats          : {ac3} (Fare: ₹{ac3Fare})");
                Console.WriteLine($"AC2 Seats          : {ac2} (Fare: ₹{ac2Fare})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error: {ex.Message}");
            }
        }

        // method to get valid station IDs
        private static List<int> GetValidStationIds()
        {
            var stationIds = new List<int>();
            var dt = DataAccess.Instance.ExecuteTable("select station_id from stations");
            foreach (DataRow row in dt.Rows)
            {
                stationIds.Add(Convert.ToInt32(row["station_id"]));
            }
            return stationIds;
        }

        // method to get station name by ID
        private static string GetStationNameById(int stationId)
        {
            var dt = DataAccess.Instance.ExecuteTable("select station_name from stations where station_id = @id",
                new SqlParameter("@id", stationId));

            if (dt.Rows.Count > 0)
                return dt.Rows[0]["station_name"].ToString();
            else
                return "Unknown";
        }

        // method to read integer input
        private static int ReadInt(string prompt)
        {
            int value;
            while (true)
            {
                Console.Write($"{prompt}: ");
                if (int.TryParse(Console.ReadLine(), out value)) break;
                Console.WriteLine($"Invalid input. Please enter a valid integer for {prompt}.");
            }
            return value;
        }

        //  method to read decimal input
        private static decimal ReadDecimal(string prompt)
        {
            decimal value;
            while (true)
            {
                Console.Write($"{prompt}: ");
                if (decimal.TryParse(Console.ReadLine(), out value)) break;
                Console.WriteLine($"Invalid input. Please enter a valid decimal for {prompt}.");
            }
            return value;
        }
    }
}
