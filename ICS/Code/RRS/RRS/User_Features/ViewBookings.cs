using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS.User_Features
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
                Console.Clear();
                Console.WriteLine("====Your Bookings====");

                // Fetch bookings and total count
                var ds = DataAccess.Instance.ExecuteDataSet(
                    "sp_GetUserBookings",
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

                // Print table header (adjusted column widths)
                Console.WriteLine();
                Console.WriteLine(
                    $"{"PNR",-15} {"Train Name",-28} {"Date",-12} {"Status",-12} {"Amount",-12} {"Passenger_Count",5}"
                );
                Console.WriteLine(new string('-', 100));

                // Print each booking row
                foreach (DataRow row in dt.Rows)
                {
                    string pnr = row["pnr_number"].ToString();
                    string trainName = row["train_name"].ToString();
                    string date = ((DateTime)row["journey_date"]).ToString("yyyy-MM-dd");
                    string status = row["booking_status"].ToString();
                    decimal amount = Convert.ToDecimal(row["total_amount"]);
                    int count = Convert.ToInt32(row["passenger_count"]);

                    // format amount with rupee symbol
                    string amountStr = $"₹{amount:N2}";

                    Console.WriteLine(
                        $"{pnr,-15} {trainName,-28} {date,-12} {status,-12} {amountStr,12} {count,5}"
                    );
                }

                // Print total records
                int totalRecords = Convert.ToInt32(ds.Tables[1].Rows[0]["totalrecords"]);
                Console.WriteLine();
                Console.WriteLine($"Total Bookings: {totalRecords}");
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
