using System;
using System.Data;
using System.Data.SqlClient;

namespace RRS.Admin_Features
{
    public class AdminRevenueReport
    {
        public static void ViewRevenueReport()
        {
            while (true)
            {
                try
                {
                    // Step 1: Input collection
                    DateTime fromDate;
                    while (true)
                    {
                        Console.Write("Enter From Date (yyyy-MM-dd) or 0 to exit: ");
                        string input = Console.ReadLine()?.Trim();

                        if (input == "0")
                        {
                            Console.WriteLine("Exiting revenue report...");
                            return;
                        }

                        if (DateTime.TryParse(input, out fromDate))
                            break;

                        Console.WriteLine("Invalid date format. Please try again.");
                    }

                    DateTime toDate;
                    while (true)
                    {
                        Console.Write("Enter To Date (yyyy-MM-dd): ");
                        if (DateTime.TryParse(Console.ReadLine(), out toDate))
                            break;

                        Console.WriteLine("Invalid date format. Please try again.");
                    }

                    int? trainId = null;
                    while (true)
                    {
                        Console.Write("Enter Train ID (or press Enter for all): ");
                        string trainInput = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(trainInput))
                            break;

                        if (int.TryParse(trainInput, out int parsedId))
                        {
                            trainId = parsedId;
                            break;
                        }

                        Console.WriteLine("Invalid Train ID. Please enter a number or leave blank.");
                    }

                    string groupBy;
                    while (true)
                    {
                        Console.Write("Group By (daily/train): ");
                        groupBy = Console.ReadLine()?.ToLower();
                        if (groupBy == "daily" || groupBy == "train")
                            break;

                        Console.WriteLine("Invalid option. Please enter 'daily' or 'train'.");
                    }

                    // Step 2: Confirm inputs
                    Console.WriteLine();
                    Console.WriteLine("Confirm Inputs:");
                    Console.WriteLine($"From Date   : {fromDate:yyyy-MM-dd}");
                    Console.WriteLine($"To Date     : {toDate:yyyy-MM-dd}");
                    Console.WriteLine($"Train ID    : {(trainId.HasValue ? trainId.Value.ToString() : "All")}");
                    Console.WriteLine($"Group By    : {groupBy}");
                    Console.Write("Proceed with report? (Y/N): ");
                    string confirm = Console.ReadLine()?.Trim().ToUpper();

                    if (confirm != "Y")
                    {
                        Console.WriteLine("Restarting input...");
                        Console.WriteLine();
                        continue;
                    }

                    // Step 3: Execute report
                    var parameters = new SqlParameter[]
                    {
                        new SqlParameter("@fromdate", fromDate),
                        new SqlParameter("@todate", toDate),
                        new SqlParameter("@trainid", trainId.HasValue ? (object)trainId.Value : DBNull.Value),
                        new SqlParameter("@groupby", groupBy)
                    };

                    var dt = DataAccess.Instance.ExecuteTable("sp_getrevenuereport", parameters);

                    // Step 4: Display results
                    if (groupBy == "daily")
                    {
                        string separator = new string('-', 100);
                        Console.WriteLine(separator);
                        Console.WriteLine($"{ "Date",-12} { "Bookings",-10} { "Passengers",-12} { "Revenue",-12} { "Refunds",-12} { "Net Revenue",-12}");
                        Console.WriteLine(separator);

                        foreach (DataRow row in dt.Rows)
                        {
                            Console.WriteLine(
                                $"{Convert.ToDateTime(row["journey_date"]):yyyy-MM-dd,-12} " +
                                $"{Convert.ToInt32(row["total_bookings"]),-10} " +
                                $"{Convert.ToInt32(row["total_passengers"]),-12} " +
                                $"₹{Convert.ToDecimal(row["total_revenue"]),-11:N2} " +
                                $"₹{Convert.ToDecimal(row["total_refunds"]),-11:N2} " +
                                $"₹{Convert.ToDecimal(row["net_revenue"]),-11:N2}"
                            );
                        }

                        Console.WriteLine(separator);
                    }
                    else if (groupBy == "train")
                    {
                        string separator = new string('-', 110);
                        Console.WriteLine(separator);
                        Console.WriteLine($"{ "Train No",-12} { "Train Name",-35} { "Bookings",-10} { "Passengers",-12} { "Revenue",-12} { "Avg Value",-12}");
                        Console.WriteLine(separator);

                        foreach (DataRow row in dt.Rows)
                        {
                            Console.WriteLine(
                                $"{row["train_number"],-12} " +
                                $"{row["train_name"],-35} " +
                                $"{Convert.ToInt32(row["total_bookings"]),-10} " +
                                $"{Convert.ToInt32(row["total_passengers"]),-12} " +
                                $"₹{Convert.ToDecimal(row["total_revenue"]),-11:N2} " +
                                $"₹{Convert.ToDecimal(row["avg_booking_value"]),-11:N2}"
                            );
                        }

                        Console.WriteLine(separator);
                    }

                    break; // exit loop after successful report
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("Restarting input...");
                    Console.WriteLine();
                }
            }
        }
    }
}
