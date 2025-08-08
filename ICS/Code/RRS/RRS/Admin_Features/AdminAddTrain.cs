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
                Console.WriteLine("=== Add New Train ===");
                Console.WriteLine("Enter 0 at any prompt to cancel.\n");

                // Train Number
                Console.Write("Train Number: ");
                string number = Console.ReadLine();
                if (number == "0")
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }

                // Train Name
                Console.Write("\nTrain Name: ");
                string name = Console.ReadLine();
                if (name == "0")
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }

                Console.WriteLine();
                AdminViewStations.adminViewStations();

                List<int> validStationIds = GetValidStationIds();

                // Source Station
                Console.WriteLine();
                Console.Write("Source Station ID: ");
                string srcInput = Console.ReadLine();
                if (srcInput == "0")
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }
                if (!int.TryParse(srcInput, out int src) || !validStationIds.Contains(src))
                {
                    Console.WriteLine("Invalid Source Station ID. Please enter a valid ID from the list.");
                    return;
                }

                // Destination Station
                Console.WriteLine();
                Console.Write("Destination Station ID: ");
                string dstInput = Console.ReadLine();
                if (dstInput == "0")
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }
                if (!int.TryParse(dstInput, out int dst) || !validStationIds.Contains(dst))
                {
                    Console.WriteLine("Invalid Destination Station ID. Please enter a valid ID from the list.");
                    return;
                }

                // Departure Time
                Console.WriteLine();
                Console.Write("Departure Time (HH:mm): ");
                string depInput = Console.ReadLine();
                if (depInput == "0")
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }
                if (!TimeSpan.TryParse(depInput, out TimeSpan dep))
                {
                    Console.WriteLine("Invalid time format. Please enter time as HH:mm.");
                    return;
                }

                // Arrival Time
                Console.WriteLine();
                Console.Write("Arrival Time (HH:mm): ");
                string arrInput = Console.ReadLine();
                if (arrInput == "0")
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }
                if (!TimeSpan.TryParse(arrInput, out TimeSpan arr))
                {
                    Console.WriteLine("Invalid time format. Please enter time as HH:mm.");
                    return;
                }

                // Running Days
                Console.WriteLine();
                Console.Write("Running Days (e.g. 1111100 for Mon-Fri): ");
                string days = Console.ReadLine();
                if (days == "0")
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }
                if (days.Length != 7 || days.Any(c => c != '0' && c != '1'))
                {
                    Console.WriteLine("Invalid format. Please enter exactly 7 digits using only 0 or 1.");
                    return;
                }

                // Sleeper Seats
                Console.WriteLine();
                int sleeper = ReadInt("Sleeper Seats");
                if (sleeper == 0)
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }

                // AC3 Seats
                Console.WriteLine();
                int ac3 = ReadInt("AC3 Seats");
                if (ac3 == 0)
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }

                // AC2 Seats
                Console.WriteLine();
                int ac2 = ReadInt("AC2 Seats");
                if (ac2 == 0)
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }

                // Total Seats
                int total = sleeper + ac3 + ac2;
                Console.WriteLine($"\nTotal Seats: {total}");

                // Sleeper Fare
                Console.WriteLine();
                decimal sleeperFare = ReadDecimal("Sleeper Fare");
                if (sleeperFare == 0m)
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }

                // AC3 Fare
                Console.WriteLine();
                decimal ac3Fare = ReadDecimal("AC3 Fare");
                if (ac3Fare == 0m)
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }

                // AC2 Fare
                Console.WriteLine();
                decimal ac2Fare = ReadDecimal("AC2 Fare");
                if (ac2Fare == 0m)
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }

                // Insert into database
                DataAccess.Instance.ExecuteNonQuery(
                    "INSERT INTO trains (train_number, train_name, source_station_id, destination_station_id, " +
                    "departure_time, arrival_time, running_days, total_seats, sleeper_seats, ac3_seats, ac2_seats, " +
                    "sleeper_fare, ac3_fare, ac2_fare) " +
                    "VALUES (@tnum, @tname, @src, @dst, @dep, @arr, @days, @total, @sleeper, @ac3, @ac2, @sfare, @ac3fare, @ac2fare)",
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

                // Display confirmation
                Console.WriteLine("\nTrain added successfully.\n");
                Console.WriteLine($"Train Number       : {number}");
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

        private static List<int> GetValidStationIds()
        {
            var stationIds = new List<int>();
            var dt = DataAccess.Instance.ExecuteTable("SELECT station_id FROM stations");
            foreach (DataRow row in dt.Rows)
            {
                stationIds.Add(Convert.ToInt32(row["station_id"]));
            }
            return stationIds;
        }

        private static string GetStationNameById(int stationId)
        {
            var dt = DataAccess.Instance.ExecuteTable(
                "SELECT station_name FROM stations WHERE station_id = @id",
                new SqlParameter("@id", stationId));

            if (dt.Rows.Count > 0)
                return dt.Rows[0]["station_name"].ToString();
            else
                return "Unknown";
        }

        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                string input = Console.ReadLine();
                if (input == "0")
                    return 0;
                if (int.TryParse(input, out int value))
                    return value;
                Console.WriteLine($"Invalid input. Please enter a valid integer for {prompt}.");
            }
        }

        private static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                string input = Console.ReadLine();
                if (input == "0")
                    return 0m;
                if (decimal.TryParse(input, out decimal value))
                    return value;
                Console.WriteLine($"Invalid input. Please enter a valid decimal for {prompt}.");
            }
        }
    }
}