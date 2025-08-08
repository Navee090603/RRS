using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS.Admin_Features
{
    public class AdminViewAllPayments
    {
        public static void adminViewAllPayments()
        {
            try
            {
                var dt = DataAccess.Instance.ExecuteTable(
                    "SELECT p.payment_id, p.booking_id, p.amount, p.payment_method, p.transaction_id, " +
                    "p.payment_status, p.payment_time, p.refund_amount, p.refund_time " +
                    "FROM payments p ORDER BY p.payment_time DESC");

                int totalWidth = 120;
                string separator = new string('-', totalWidth);

                Console.WriteLine(separator);
                Console.WriteLine(
                    $"{ "PayID",-6} { "BookID",-7} { "Amount",-10} { "Method",-10} { "Status",-10} { "Refund",-10} { "Time",-12} { "TxnID",-10} { "RTime",-12}");
                Console.WriteLine(separator);

                foreach (DataRow row in dt.Rows)
                {
                    string amount = $"₹{Convert.ToDecimal(row["amount"]):N2}";
                    string refund = $"₹{Convert.ToDecimal(row["refund_amount"]):N2}";
                    string payTime = Convert.ToDateTime(row["payment_time"]).ToString("dd-MM HH:mm");
                    string refundTime = row["refund_time"] == DBNull.Value ? "N/A" : Convert.ToDateTime(row["refund_time"]).ToString("dd-MM HH:mm");
                    string txnId = row["transaction_id"].ToString();
                    txnId = txnId.Length > 8 ? txnId.Substring(txnId.Length - 8) : txnId;

                    Console.WriteLine(
                        $"{row["payment_id"],-6} " +
                        $"{row["booking_id"],-7} " +
                        $"{amount,-10} " +
                        $"{row["payment_method"],-10} " +
                        $"{row["payment_status"],-10} " +
                        $"{refund,-10} " +
                        $"{payTime,-12} " +
                        $"{txnId,-10} " +
                        $"{refundTime,-12}");
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
