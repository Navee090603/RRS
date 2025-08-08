using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS.Admin_Features
{
    public class AdminViewTrains
    {
        public static void adminViewTrains()
        {
            try
            {
                var dt = DataAccess.Instance.ExecuteTable(
                    "select t.train_id, t.train_number, t.train_name, s1.station_name as source, s2.station_name as destination, " +
                    "t.departure_time, t.arrival_time, t.running_days, t.is_active " +
                    "from trains t " +
                    "inner join stations s1 on t.source_station_id = s1.station_id " +
                    "inner join stations s2 on t.destination_station_id = s2.station_id");

                Console.WriteLine("==Trains==");
                Console.WriteLine();

                int totalWidth = 120;
                string separator = new string('-', totalWidth);
                Console.WriteLine($"{"ID",-4} {"Number",-15} {"Name",-30} {"Source",-15} {"Destination",-15} {"Dep",-10} {"Arr",-10} {"Active",-6}");
                Console.WriteLine(separator);

                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine(
                        $"{row["train_id"],-4} " +
                        $"{row["train_number"],-15} " +
                        $"{row["train_name"],-30} " +
                        $"{row["source"],-15} " +
                        $"{row["destination"],-15} " +
                        $"{row["departure_time"],-10} " +
                        $"{row["arrival_time"],-10} " +
                        $"{row["is_active"],-6}");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
