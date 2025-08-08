using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS.User_Features
{
    public class CancelBooking
    {
        public static int? loggedInUserId { get; set; }

        public static void cancelBooking()
        {
            if (loggedInUserId == null)
            {
                Console.WriteLine("Please login first to cancel a booking.");
                return;
            }

            try
            {
                Console.Clear();
                Console.WriteLine("===Cancellation Process===");

                while (true)
                {
                    // 1) PNR prompt with exit option
                    Console.Write("\n\nEnter PNR to cancel from (or 0 to exit): ");
                    string pnr = Console.ReadLine()?.Trim();
                    if (pnr == "0")
                    {
                        Console.WriteLine("Cancellation process aborted.");
                        return;
                    }

                    // 2) Passenger name
                    Console.Write("Enter Passenger Name to cancel: ");
                    string passengerName = Console.ReadLine()?.Trim();

                    // 3) Reason for cancellation
                    Console.Write("Reason for cancellation: ");
                    string reason = Console.ReadLine()?.Trim();

                    // 4) Confirm before proceeding
                    Console.Write($"\nConfirm cancellation of passenger \"{passengerName}\" under PNR \"{pnr}\"? (Y/N): ");
                    string confirm = Console.ReadLine()?.Trim().ToUpper();
                    if (confirm != "Y")
                    {
                        Console.WriteLine("\nCancellation aborted by user.");
                        return;
                    }

                    // 5) Execute stored procedure
                    var dt = DataAccess.Instance.ExecuteTable(
                        "sp_cancelbooking",
                        new SqlParameter("@pnrnumber", pnr),
                        new SqlParameter("@userid", loggedInUserId),
                        new SqlParameter("@passengername", passengerName),
                        new SqlParameter("@cancellationreason", reason)
                    );

                    // 6) Handle result
                    if (dt.Rows.Count > 0)
                    {
                        Console.Clear();
                        Console.WriteLine($"{dt.Rows[0]["message"]}\nRefund: {dt.Rows[0]["refundamount"]}");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("\nCancellation failed: No such passenger in this booking.");
                        Console.WriteLine("Press Enter to try again.");
                        Console.ReadLine();
                        Console.Clear();
                        // loop back to PNR 
                    }
                }
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
