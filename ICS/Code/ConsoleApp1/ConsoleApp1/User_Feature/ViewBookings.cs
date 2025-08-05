using System;
using System.Data;
using System.Data.SqlClient;

namespace ConsoleApp1.User_Feature
{
    public class ViewBookings
    {
        public static int? loggedInUserId { get; set; }
        public static void viewBookings()
        {
            if (loggedInUserId == null)
            {
                Console.WriteLine("Please login first to view bookings.");
                return;
            }

            try
            {
                var ds = DataAccess.Instance.ExecuteDataSet("sp_GetUserBookings",
                    new SqlParameter("@UserId", loggedInUserId),
                    new SqlParameter("@BookingStatus", DBNull.Value),
                    new SqlParameter("@PageNumber", 1),
                    new SqlParameter("@PageSize", 10)
                );
                var dt = ds.Tables[0];
                if (dt.Rows.Count == 0)
                {
                    Console.WriteLine("No bookings found.");
                    return;
                }
                Console.WriteLine("\nYour Bookings:");
                foreach (DataRow row in dt.Rows)
                    Console.WriteLine($"PNR: {row["pnr_number"]}, Train: {row["train_name"]}, Date: {row["journey_date"]}, Status: {row["booking_status"]}, Amount: {row["total_amount"]}");
                Console.WriteLine($"Total: {ds.Tables[1].Rows[0]["totalrecords"]}");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}