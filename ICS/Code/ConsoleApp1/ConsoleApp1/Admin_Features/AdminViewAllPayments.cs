using System;
using System.Data;

namespace ConsoleApp1.User_Features
{
    public class AdminViewAllPayments
    {
        public static void adminViewAllPayments()
        {
            try
            {
                var dt = DataAccess.Instance.ExecuteTable(@"select p.payment_id, p.booking_id, p.amount, p.payment_method, p.transaction_id, p.payment_status, 
                p.payment_time, p.refund_amount, p.refund_time 
                from payments p order by p.payment_time desc");
                Console.WriteLine("Payments:");
                foreach (DataRow row in dt.Rows)
                    Console.WriteLine($"PayID:{row["payment_id"]}, BookID:{row["booking_id"]}, Amount:{row["amount"]}, Method:{row["payment_method"]}, Status:{row["payment_status"]}, Refund:{row["refund_amount"]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
