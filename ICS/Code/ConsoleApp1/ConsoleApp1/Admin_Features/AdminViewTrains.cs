using System;
using System.Data;
using ConsoleApp1;

namespace ConsoleApp1.User_Features
{
    public class AdminViewTrains
    {
        public static void adminViewTrains()
        {
            try
            {
                var dt = DataAccess.Instance.ExecuteTable(@"
                    select t.train_id, t.train_number, t.train_name, s1.station_name as source, s2.station_name as destination, 
                    t.departure_time, t.arrival_time, t.running_days, t.is_active 
                    from trains t 
                    inner join stations s1 on t.source_station_id = s1.station_id
                    inner join stations s2 on t.destination_station_id = s2.station_id");

                Console.WriteLine("Trains:");
                foreach (DataRow row in dt.Rows)
                    Console.WriteLine($"ID:{row["train_id"]}, {row["train_number"]}, {row["train_name"]}, {row["source"]}->{row["destination"]}, Dep:{row["departure_time"]}, Arr:{row["arrival_time"]}, Active:{row["is_active"]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
