using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text;

namespace RRS.User_Features
{
    public class DownloadTicket
    {
        public static void downloadTicket()
        {
            Console.Clear();
            Console.Write("Enter PNR Number: ");
            string pnrNumber = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(pnrNumber))
            {
                Console.WriteLine("PNR number cannot be empty.");
                return;
            }

            try
            {
                // Get booking ID from PNR
                var pnrParam = new SqlParameter[]
                {
                    new SqlParameter("@pnr_number", pnrNumber)
                };
                var bookingIdDt = DataAccess.Instance.ExecuteTable(
                    "SELECT booking_id FROM bookings WHERE pnr_number = @pnr_number",
                    pnrParam
                );

                if (bookingIdDt.Rows.Count == 0)
                {
                    Console.WriteLine("No booking found for the given PNR.");
                    return;
                }

                int bookingId = Convert.ToInt32(bookingIdDt.Rows[0]["booking_id"]);

                // Get full booking details
                var bookingParams = new SqlParameter[]
                {
                    new SqlParameter("@booking_id", bookingId)
                };
                var bookingDt = DataAccess.Instance.ExecuteTable(
                    "SELECT b.pnr_number, b.booking_time, b.journey_date, b.total_amount, b.booking_status, " +
                    "u.name AS user_name, t.train_number, t.train_name " +
                    "FROM bookings b " +
                    "INNER JOIN users u ON b.user_id = u.user_id " +
                    "INNER JOIN trains t ON b.train_id = t.train_id " +
                    "WHERE b.booking_id = @booking_id",
                    bookingParams
                );

                // Get passenger details
                var passengerParams = new SqlParameter[]
                {
                    new SqlParameter("@booking_id", bookingId)
                };
                var passengerDt = DataAccess.Instance.ExecuteTable(
                    "SELECT name, age, gender, seat_type, seat_number, coach_number, fare_paid, status " +
                    "FROM passengers WHERE booking_id = @booking_id",
                    passengerParams
                );

                var booking = bookingDt.Rows[0];

                // Build ticket content
                var sb = new StringBuilder();
                sb.AppendLine("=== Railway Reservation Ticket ===");
                sb.AppendLine($"PNR Number : {booking["pnr_number"]}");
                sb.AppendLine($"User Name  : {booking["user_name"]}");
                sb.AppendLine($"Train      : {booking["train_name"]} ({booking["train_number"]})");
                sb.AppendLine($"Journey Date: {Convert.ToDateTime(booking["journey_date"]):yyyy-MM-dd}");
                sb.AppendLine($"Booking Time: {Convert.ToDateTime(booking["booking_time"]):yyyy-MM-dd HH:mm}");
                sb.AppendLine($"Status     : {booking["booking_status"]}");
                sb.AppendLine($"Total Amount: ₹{Convert.ToDecimal(booking["total_amount"]):N2}");
                sb.AppendLine();
                sb.AppendLine("Passengers:");
                sb.AppendLine("-------------------------------------------------------------");
                sb.AppendLine($"{"Name",-15} {"Age",-4} {"Gender",-6} {"Type",-8} {"Coach",-6} {"Seat",-6} {"Fare",-8} {"Status",-10}");
                sb.AppendLine("-------------------------------------------------------------");

                foreach (DataRow row in passengerDt.Rows)
                {
                    sb.AppendLine(
                        $"{row["name"],-15} {row["age"],-4} {row["gender"],-6} {row["seat_type"],-8} " +
                        $"{row["coach_number"],-6} {row["seat_number"],-6} ₹{Convert.ToDecimal(row["fare_paid"]),-7:N2} {row["status"],-10}"
                    );
                }

                sb.AppendLine("-------------------------------------------------------------");

                // Ask user for folder to save, with retry or cancel
                string folderPath;
                while (true)
                {
                    Console.Write("Enter folder path to save ticket (leave blank for current directory, or 'c' to cancel): ");
                    folderPath = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(folderPath))
                    {
                        folderPath = Directory.GetCurrentDirectory();
                    }
                    else if (string.Equals(folderPath, "c", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Operation cancelled.");
                        return;
                    }

                    if (Directory.Exists(folderPath))
                    {
                        break;
                    }

                    Console.WriteLine("Could not find the specified path. Please try again or press 'c' to cancel.");
                }

                string fileName = $"Ticket_{booking["pnr_number"]}.txt";
                string fullPath = Path.Combine(folderPath, fileName);

                // Attempt to write, handle IO errors gracefully with one more retry
                try
                {
                    File.WriteAllText(fullPath, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                }
                catch (IOException ioEx)
                {
                    Console.WriteLine($"I/O error while saving: {ioEx.Message}");
                    Console.WriteLine("Please enter another folder path or 'c' to cancel.");
                    // one more path prompt
                    while (true)
                    {
                        Console.Write("Folder path: ");
                        var retryPath = Console.ReadLine()?.Trim();

                        if (string.Equals(retryPath, "c", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Operation cancelled.");
                            return;
                        }

                        if (!string.IsNullOrEmpty(retryPath) && Directory.Exists(retryPath))
                        {
                            fullPath = Path.Combine(retryPath, fileName);
                            File.WriteAllText(fullPath, sb.ToString(), new UTF8Encoding(false));
                            break;
                        }

                        Console.WriteLine("Path not found. Try again or 'c' to cancel.");
                    }
                }

                Console.Clear();
                Console.WriteLine($"Ticket saved to: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
