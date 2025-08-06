using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS.Admin_Features
{
    public class AdminViewStations
    {
        public static void adminViewStations()
        {
            try
            {
                var dt = DataAccess.Instance.ExecuteTable("sp_getallstations");

                if (dt.Rows.Count == 0)
                {
                    Console.WriteLine("No stations found.");
                    return;
                }

                Console.WriteLine("\n Available Stations:");
                Console.WriteLine(new string('-', 45));
                Console.WriteLine($"{"ID",-5} {"Station Name",-25} {"Code",-10}");
                Console.WriteLine(new string('-', 45));

                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"{row["station_id"],-5} {row["station_name"],-25} {row["station_code"],-10}");
                }

                Console.WriteLine(new string('-', 45));
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error: {ex.Message}");
            }
        }

    }
}
