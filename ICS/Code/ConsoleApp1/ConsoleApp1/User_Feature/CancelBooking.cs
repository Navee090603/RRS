using System;
using System.Data.SqlClient;

namespace ConsoleApp1.User_Feature
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
                Console.Write("Enter PNR to cancel from: ");
                string pnr = Console.ReadLine();
                Console.Write("Enter Passenger Name to cancel: ");
                string passengerName = Console.ReadLine();
                Console.Write("Reason for cancellation: ");
                string reason = Console.ReadLine();

                var dt = DataAccess.Instance.ExecuteTable("sp_cancelbooking",
                    new SqlParameter("@pnrnumber", pnr),
                    new SqlParameter("@userid", loggedInUserId),
                    new SqlParameter("@passengername", passengerName),
                    new SqlParameter("@cancellationreason", reason)
                );
                if (dt.Rows.Count > 0)
                    Console.WriteLine($"{dt.Rows[0]["message"]}\nRefund: {dt.Rows[0]["refundamount"]}");
                else
                    Console.WriteLine("Cancellation failed: No such passenger in this booking.");
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