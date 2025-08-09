using System;
using System.Data;

namespace RRS.Admin_Features
{
    public class AdminViewAllBookings
    {
        public static void adminViewAllBookings()
        {
            try
            {
                var dt = DataAccess.Instance.ExecuteTable(
                    "SELECT b.booking_id, b.pnr_number, b.booking_time, b.journey_date, b.booking_status, " +
                    "u.name AS user_name, t.train_number, t.train_name, b.total_amount, b.refund_amount " +
                    "FROM bookings b " +
                    "INNER JOIN users u ON b.user_id = u.user_id " +
                    "INNER JOIN trains t ON b.train_id = t.train_id " +
                    "ORDER BY b.booking_time DESC"
                );

                int totalWidth = 130;
                string separator = new string('-', totalWidth);

                Console.WriteLine(separator);
                Console.WriteLine(
                    $"{ "PNR",-12} " +
                    $"{ "User",-15} " +
                    $"{ "Train No",-12} " +
                    $"{ "Train Name",-35} " +
                    $"{ "Date",-12} " +
                    $"{ "Status",-12} " +
                    $"{ "Total Amount",-10} " +
                    $"{ "Refund Amount",-10}"
                );
                Console.WriteLine(separator);

                foreach (DataRow row in dt.Rows)
                {
                    string pnr = row["pnr_number"]?.ToString() ?? "";
                    string user = row["user_name"]?.ToString() ?? "";
                    string trainNo = row["train_number"]?.ToString() ?? "";
                    string trainName = row["train_name"]?.ToString() ?? "";
                    string journeyDate = row["journey_date"] != DBNull.Value
                        ? Convert.ToDateTime(row["journey_date"]).ToString("yyyy-MM-dd")
                        : "N/A";
                    string status = row["booking_status"]?.ToString() ?? "";

                    decimal totalAmount = row["total_amount"] != DBNull.Value
                        ? Convert.ToDecimal(row["total_amount"])
                        : 0;

                    decimal refundAmount = row["refund_amount"] != DBNull.Value
                        ? Convert.ToDecimal(row["refund_amount"])
                        : 0;

                    Console.WriteLine(
                        $"{pnr,-12} " +
                        $"{user,-15} " +
                        $"{trainNo,-12} " +
                        $"{trainName,-35} " +
                        $"{journeyDate,-12} " +
                        $"{status,-12} " +
                        $"₹{totalAmount,-10:N2} " +
                        $"₹{refundAmount,-10:N2}"
                    );
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
