using System;
using System.Data.SqlClient;
using ConsoleApp1;
using System.Data;

namespace ConsoleApp1.User_Features
{
    public class AdminAddTrain
    {
        public static void adminAddTrain()
        {
            try
            {
                Console.Write("Train Number: ");
                string number = Console.ReadLine();
                Console.Write("Train Name: ");
                string name = Console.ReadLine();
                AdminViewStations.adminViewStations();
                Console.Write("Source Station ID: ");
                int src = int.Parse(Console.ReadLine());
                Console.Write("Destination Station ID: ");
                int dst = int.Parse(Console.ReadLine());
                Console.Write("Departure Time (HH:mm): ");
                TimeSpan dep = TimeSpan.Parse(Console.ReadLine());
                Console.Write("Arrival Time (HH:mm): ");
                TimeSpan arr = TimeSpan.Parse(Console.ReadLine());
                Console.Write("Running Days (e.g. 1111100 for Mon-Fri): ");
                string days = Console.ReadLine();
                Console.Write("Total Seats: ");
                int total = int.Parse(Console.ReadLine());
                Console.Write("Sleeper Seats: ");
                int sleeper = int.Parse(Console.ReadLine());
                Console.Write("AC3 Seats: ");
                int ac3 = int.Parse(Console.ReadLine());
                Console.Write("AC2 Seats: ");
                int ac2 = int.Parse(Console.ReadLine());
                Console.Write("Sleeper Fare: ");
                decimal sleeperFare = decimal.Parse(Console.ReadLine());
                Console.Write("AC3 Fare: ");
                decimal ac3Fare = decimal.Parse(Console.ReadLine());
                Console.Write("AC2 Fare: ");
                decimal ac2Fare = decimal.Parse(Console.ReadLine());

                DataAccess.Instance.ExecuteNonQuery(
                    "insert into trains (train_number, train_name, source_station_id, destination_station_id, departure_time, arrival_time, running_days, total_seats, sleeper_seats, ac3_seats, ac2_seats, sleeper_fare, ac3_fare, ac2_fare) values (@tnum, @tname, @src, @dst, @dep, @arr, @days, @total, @sleeper, @ac3, @ac2, @sfare, @ac3fare, @ac2fare)",
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
                Console.WriteLine("Train added successfully.");
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input format. Please enter correct values.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}