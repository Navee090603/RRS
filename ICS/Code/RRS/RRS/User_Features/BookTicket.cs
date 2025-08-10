using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRS.User_Features
{
    public class BookTicket
    {
        public static int? loggedInUserId { get; set; }
        public static void bookTicket()
        {
            Console.Clear();

            if (loggedInUserId == null)
            {
                Console.WriteLine("Please login first to book tickets.");
                return;
            }

            try
            {
                // 1. Fetch and display stations
                var stations = DataAccess.Instance.ExecuteTable("sp_getallstations");
                HashSet<int> stationIds = new HashSet<int>();

                Console.WriteLine("\nAvailable Stations:");
                Console.WriteLine(new string('-', 45));
                Console.WriteLine($"{"ID",-5} {"Station Name",-25} {"Code",-5}");
                Console.WriteLine(new string('-', 45));

                foreach (DataRow row in stations.Rows)
                {
                    Console.WriteLine($"{row["station_id"],-5} {row["station_name"],-25} {row["station_code"],-5}");
                    stationIds.Add(Convert.ToInt32(row["station_id"]));
                }

                Console.WriteLine(new string('-', 45));

                // 2. Source & Destination input loop
                int sourceId, destId;
                while (true)
                {
                    sourceId = GetValidatedInt("Source Station ID \n(or 0 to cancel): ");
                    if (sourceId == 0)
                    {
                        Console.WriteLine("Booking cancelled by user.");
                        return;
                    }
                    Console.WriteLine();
                    destId = GetValidatedInt("Destination Station ID : ");

                    if (sourceId == destId)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Source and destination cannot be the same. Please re-enter.\n");
                        continue;
                    }
                    if (!stationIds.Contains(sourceId) || !stationIds.Contains(destId))
                    {
                        Console.WriteLine();
                        Console.WriteLine("Invalid station(s). Please enter valid station IDs from the above list.\n");
                        continue;
                    }
                    break;
                }

                // 3. Seat Type
                string seatType;
                while (true)
                {
                    Console.WriteLine();
                    Console.Write("Seat Type (sleeper/ac3/ac2 or type 'exit' to cancel): ");
                    seatType = (Console.ReadLine() ?? "").Trim().ToLower();
                    if (seatType == "exit")
                    {
                        Console.WriteLine("Booking cancelled by user.");
                        return;
                    }
                    if (seatType == "sleeper" || seatType == "ac2" || seatType == "ac3")
                        break;

                    Console.WriteLine("Enter valid seat type (sleeper/ac3/ac2) or 'exit' to cancel.\n");
                }

                // 4. Passenger count
                int passengerCount;
                while (true)
                {
                    passengerCount = GetValidatedInt("\nPassenger Count : ");
                    if (passengerCount > 0 && passengerCount <= 6)
                        break;

                    Console.WriteLine("Passengers must be between 1 and 6. \n");
                }

                // 5. Journey Date & Train Search Loop
                DateTime journeyDate;
                DataTable trains;
                int trainId;
                while (true)
                {
                    // 5a. Input date or exit
                    while (true)
                    {
                        Console.WriteLine();
                        Console.Write("Journey Date (YYYY-MM-DD) (type exit to cancel) : ");
                        string dateInput = (Console.ReadLine() ?? "").Trim().ToLower();
                        if (dateInput == "exit")
                        {
                            Console.WriteLine("Booking cancelled by user.");
                            return;
                        }
                        if (DateTime.TryParse(dateInput, out journeyDate))
                            break;

                        Console.WriteLine("Invalid date format. Please enter date as YYYY-MM-DD.\n");
                    }

                    // 5b. Search trains
                    trains = DataAccess.Instance.ExecuteTable(
                        "sp_searchtrains",
                        new SqlParameter("@sourcestationid", sourceId),
                        new SqlParameter("@destinationstationid", destId),
                        new SqlParameter("@journeydate", journeyDate),
                        new SqlParameter("@seattype", seatType),
                        new SqlParameter("@passengercount", passengerCount)
                    );

                    if (trains.Rows.Count == 0)
                    {
                        Console.WriteLine("\nNo trains found. Booking is allowed only for dates up to '2025-12-05'.");
                        Console.WriteLine("Please re-enter a valid journey date or type 'exit' to cancel.\n");
                        continue;
                    }

                    // 6. Display available trains
                    Console.WriteLine("\nAvailable Trains:");
                    Console.WriteLine(new string('-', 100));
                    Console.WriteLine($"{"Train ID",-10} {"Train Name",-30} {"Dep",-10} {"Arr",-10} {"Seats",-8} {"Fare/Person",-15} {"Available",-20}");
                    Console.WriteLine(new string('-', 100));

                    foreach (DataRow row in trains.Rows)
                    {
                        Console.WriteLine(
                            $"{row["train_id"],-10} " +
                            $"{row["train_name"],-30} " +
                            $"{row["departure_time"],-10} " +
                            $"{row["arrival_time"],-10} " +
                            $"{row["available_seats"],-8} " +
                            $"₹{Convert.ToDecimal(row["fare_per_passenger"]),-15:N2} " +
                            $"{row["booking_status"],-15}" 
                            //$"{row["running_days"],17}"
                        );
                    }

                    Console.WriteLine(new string('-', 100));

                    // 7. Train selection or exit
                    var validTrainIds = new HashSet<int>();
                    foreach (DataRow row in trains.Rows)
                        validTrainIds.Add(Convert.ToInt32(row["train_id"]));

                    while (true)
                    {
                        Console.Write("\nEnter Train ID to book (or 0 to cancel): ");
                        string trainInput = (Console.ReadLine() ?? "").Trim();
                        if (trainInput == "0")
                        {
                            Console.WriteLine("Booking cancelled by user.");
                            return;
                        }
                        if (int.TryParse(trainInput, out trainId) && validTrainIds.Contains(trainId))
                            break;

                        Console.WriteLine("Invalid Train ID. Please select from the listed trains or enter 0 to cancel.\n");
                    }

                    //  Journey date validation for selected train
                    var dtValid = DataAccess.Instance.ExecuteTable(
                        "SELECT dbo.fn_isvalidjourneydate(@trainid, @journeydate) AS isvalid",
                        new SqlParameter("@trainid", trainId),
                        new SqlParameter("@journeydate", journeyDate)
                    );

                    bool isValid = dtValid.Rows.Count > 0 && Convert.ToInt32(dtValid.Rows[0]["isvalid"]) == 1;
                    if (!isValid)
                    {
                        Console.WriteLine("\nSelected journey date is invalid for the chosen train.");
                        Console.WriteLine("Please check the running days and re-enter a valid journey date.\n");

                        continue; // Restart full loop
                    }

                    // If all is valid, break out of loop to continue booking
                    break;
                }
                // --- End journey date validation ---

                // 8. Passenger details
                string[] names = new string[passengerCount];
                string[] ages = new string[passengerCount];
                string[] genders = new string[passengerCount];

                for (int i = 0; i < passengerCount; i++)
                {
                    Console.WriteLine($"\nPassenger {i + 1} (or type 'exit' to cancel):");

                    // Name
                    while (true)
                    {
                        Console.Write(" Name: ");
                        string nameInput = (Console.ReadLine() ?? "").Trim();
                        if (nameInput.ToLower() == "exit")
                        {
                            Console.WriteLine("Booking cancelled by user.");
                            return;
                        }
                        if (nameInput.Length > 0)
                        {
                            names[i] = nameInput;
                            break;
                        }
                        Console.WriteLine("Name is required. Or type 'exit' to cancel.\n");
                    }

                    // Age
                    while (true)
                    {
                        Console.Write(" Age: ");
                        string ageInput = (Console.ReadLine() ?? "").Trim().ToLower();
                        if (ageInput == "exit")
                        {
                            Console.WriteLine("Booking cancelled by user.");
                            return;
                        }
                        if (int.TryParse(ageInput, out int ageVal) && ageVal >= 0 && ageVal < 120)
                        {
                            ages[i] = ageInput;
                            break;
                        }
                        Console.WriteLine("Invalid age. Or type 'exit' to cancel.\n");
                    }

                    // Gender
                    while (true)
                    {
                        Console.Write(" Gender (male/female/other): ");
                        string genderInput = (Console.ReadLine() ?? "").Trim().ToLower();
                        if (genderInput == "exit")
                        {
                            Console.WriteLine("Booking cancelled by user.");
                            return;
                        }
                        if (genderInput == "male" || genderInput == "female" || genderInput == "other")
                        {
                            genders[i] = genderInput;
                            break;
                        }
                        Console.WriteLine("Enter valid gender (male, female, other) or 'exit' to cancel.\n");
                    }
                }

                // 9. Payment method or exit
                string[] validMethods = { "upi", "netbanking", "creditcard", "debitcard" };
                string paymentMethod;
                while (true)
                {
                    Console.Write("\nPayment Method (UPI/NetBanking/CreditCard/DebitCard or type 'exit' to cancel): ");
                    paymentMethod = (Console.ReadLine() ?? "").Trim().ToLower();
                    if (paymentMethod == "exit")
                    {
                        Console.WriteLine("Booking cancelled by user.");
                        return;
                    }
                    if (Array.Exists(validMethods, m => m == paymentMethod))
                        break;

                    Console.WriteLine("Invalid payment method. Please try again or type 'exit' to cancel.\n");
                }

                // 10. Final confirmation
                Console.Write("\nConfirm booking? (yes/no): ");
                string confirm = (Console.ReadLine() ?? "").Trim().ToLower();
                if (confirm != "yes")
                {
                    Console.WriteLine("Booking cancelled.");
                    return;
                }

                // 11. Execute booking
                string transactionId = Guid.NewGuid().ToString();
                var bookingIdParam = new SqlParameter("@BookingId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var pnrParam = new SqlParameter("@PNRNumber", SqlDbType.NVarChar, 20) { Direction = ParameterDirection.Output };
                var amountParam = new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Direction = ParameterDirection.Output };
                var statusParam = new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Direction = ParameterDirection.Output };

                DataAccess.Instance.ExecuteNonQuery(
                    "sp_MakeBooking",
                    new SqlParameter("@UserId", loggedInUserId),
                    new SqlParameter("@TrainId", trainId),
                    new SqlParameter("@JourneyDate", journeyDate),
                    new SqlParameter("@PassengerCount", passengerCount),
                    new SqlParameter("@SeatType", seatType),
                    new SqlParameter("@PaymentMethod", paymentMethod),
                    new SqlParameter("@TransactionId", transactionId),
                    new SqlParameter("@PassengerNames", string.Join(",", names)),
                    new SqlParameter("@PassengerAges", string.Join(",", ages)),
                    new SqlParameter("@PassengerGenders", string.Join(",", genders)),
                    bookingIdParam,
                    pnrParam,
                    amountParam,
                    statusParam
                );

                Console.WriteLine("\nBooking Confirmed!");
                Console.WriteLine($"Status       : {statusParam.Value}");
                Console.WriteLine($"PNR Number   : {pnrParam.Value}");
                Console.WriteLine($"Total Amount : ₹{Convert.ToDecimal(amountParam.Value):N2}");
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

        private static int GetValidatedInt(string prompt)
        {
            int val;
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim() ?? "";
                if (int.TryParse(input, out val))
                    return val;
                Console.WriteLine("Invalid number. Please try again.\n");
            }
        }

        public static bool CanBookTicket()
        {
            return loggedInUserId != null;
        }

        public static bool ValidateSourceAndDestination(int sourceId, int destId, HashSet<int> stationIds)
        {
            if (sourceId == destId) return false;
            if (!stationIds.Contains(sourceId) || !stationIds.Contains(destId)) return false;
            return true;
        }

    }
}
