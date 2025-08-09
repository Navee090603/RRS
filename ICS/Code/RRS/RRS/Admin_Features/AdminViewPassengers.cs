using System;
using System.Data;
using System.Data.SqlClient;

namespace RRS.Admin_Features
{
    public class AdminViewPassengers
    {
        public static void ViewPassengers()
        {
            try
            {
                Console.Write("Enter Booking ID to view passengers (or press Enter to view all): ");
                string input = Console.ReadLine();
                string query;
                SqlParameter[] parameters;

                if (string.IsNullOrWhiteSpace(input))
                {
                    query = "SELECT * FROM passengers ORDER BY booking_id, passenger_id";
                    parameters = null;
                }
                else if (int.TryParse(input, out int bookingId))
                {
                    query = "SELECT * FROM passengers WHERE booking_id = @booking_id ORDER BY passenger_id";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@booking_id", bookingId)
                    };
                }
                else
                {
                    Console.WriteLine("Invalid Booking ID.");
                    return;
                }

                DataTable dt = parameters == null
                    ? DataAccess.Instance.ExecuteTable(query)
                    : DataAccess.Instance.ExecuteTable(query, parameters);

                string separator = new string('-', 120);
                Console.WriteLine(separator);
                Console.WriteLine(
                    $"{ "ID",-5} { "Booking",-8} { "Name",-20} { "Age",-5} { "Gender",-8} { "Seat Type",-10} { "Seat No",-10} { "Coach",-10} { "Fare",-10} { "Status",-12}"
                );
                Console.WriteLine(separator);

                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine(
                        $"{row["passenger_id"],-5} " +
                        $"{row["booking_id"],-8} " +
                        $"{row["name"],-20} " +
                        $"{row["age"],-5} " +
                        $"{row["gender"],-8} " +
                        $"{row["seat_type"],-10} " +
                        $"{row["seat_number"],-10} " +
                        $"{row["coach_number"],-10} " +
                        $"₹{Convert.ToDecimal(row["fare_paid"]),-9:N2} " +
                        $"{row["status"],-12}"
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
