using System;
using System.Data;
using ConsoleApp1;

namespace ConsoleApp1.User_Features
{
    public class AdminViewAllBookings
    {
        public static void adminViewAllBookings()
        {
            try
            {
                var dt = DataAccess.Instance.ExecuteTable(@"select b.booking_id, b.pnr_number, b.booking_time, b.journey_date, b.booking_status, 
                    u.name as user_name, t.train_number, t.train_name, b.total_amount
                    from bookings b
                    inner join users u on b.user_id = u.user_id
                    inner join trains t on b.train_id = t.train_id
                    order by b.booking_time desc");
                Console.WriteLine("Bookings:");
                foreach (DataRow row in dt.Rows)
                    Console.WriteLine($"PNR:{row["pnr_number"]}, User:{row["user_name"]}, Train:{row["train_number"]} ({row["train_name"]}), Date:{row["journey_date"]}, Status:{row["booking_status"]}, Amount:{row["total_amount"]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
