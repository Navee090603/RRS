using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS.Admin_Features
{
    public class AdminViewAllBookings
    {
        public static void adminViewAllBookings()
        {
            try
            {
                var dt = DataAccess.Instance.ExecuteTable(
                    "select b.booking_id, b.pnr_number, b.booking_time, b.journey_date, b.booking_status, " +
                    "u.name as user_name, t.train_number, t.train_name, b.total_amount " +
                    "from bookings b " +
                    "inner join users u on b.user_id = u.user_id " +
                    "inner join trains t on b.train_id = t.train_id " +
                    "order by b.booking_time desc"
                );

                int totalWidth = 120;
                string separator = new string('-', totalWidth);

                Console.WriteLine(separator);
                Console.WriteLine(
                    $"{ "PNR",-15} { "User",-15} { "Train No",-15} { "Train Name",-35} { "Date",-12} { "Status",-12} { "Amount",10}");
                Console.WriteLine(separator);

                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine(
                        $"{row["pnr_number"],-15} " +
                        $"{row["user_name"],-15} " +
                        $"{row["train_number"],-15} " +
                        $"{row["train_name"],-35} " +
                        $"{Convert.ToDateTime(row["journey_date"]).ToString("yyyy-MM-dd"),-12} " +
                        $"{row["booking_status"],-12} " +
                        $"₹{Convert.ToDecimal(row["total_amount"]):N2}");
                }

                Console.WriteLine(separator);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
