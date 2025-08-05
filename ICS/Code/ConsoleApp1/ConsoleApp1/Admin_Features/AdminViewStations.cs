using System;
using System.Data;
using ConsoleApp1;

namespace ConsoleApp1.User_Features
{
    public class AdminViewStations
    {
        public static void adminViewStations()
        {
            try
            {
                var dt = DataAccess.Instance.ExecuteTable("sp_getallstations");
                Console.WriteLine("Stations:");
                foreach (DataRow row in dt.Rows)
                    Console.WriteLine($"{row["station_id"]} - {row["station_name"]} ({row["station_code"]})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
