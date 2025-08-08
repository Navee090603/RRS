using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ConsoleApp1.User_Feature
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
                // Show stations
                var stations = DataAccess.Instance.ExecuteTable("sp_getallstations");
                Console.WriteLine("\nStations:");
                foreach (DataRow row in stations.Rows)
                    Console.WriteLine($"{row["station_id"]} - {row["station_name"]} ({row["station_code"]})");

                Console.Write("Source Station ID: ");
                int sourceId = int.Parse(Console.ReadLine());
                Console.Write("Destination Station ID: ");
                int destId = int.Parse(Console.ReadLine());
                Console.Write("Journey Date (YYYY-MM-DD): ");
                DateTime journeyDate = DateTime.Parse(Console.ReadLine());
                Console.Write("Seat Type (sleeper/ac3/ac2): ");
                string seatType = Console.ReadLine();
                Console.Write("Passenger Count: ");
                int passengerCount = int.Parse(Console.ReadLine());

                // Search trains
                var trains = DataAccess.Instance.ExecuteTable("sp_searchtrains",
                    new SqlParameter("@sourcestationid", sourceId),
                    new SqlParameter("@destinationstationid", destId),
                    new SqlParameter("@journeydate", journeyDate),
                    new SqlParameter("@seattype", seatType),
                    new SqlParameter("@passengercount", passengerCount)
                );

                if (trains.Rows.Count == 0)
                {
                    Console.WriteLine("No trains found for the given criteria.");
                    return;
                }

                Console.WriteLine("\nAvailable Trains:");
                foreach (DataRow row in trains.Rows)
                    Console.WriteLine($"TrainID: {row["train_id"]}, Name: {row["train_name"]}, Dep:{row["departure_time"]}, Arr:{row["arrival_time"]}, Seats:{row["available_seats"]}, Fare/Person:{row["fare_per_passenger"]}, Status:{row["booking_status"]}");

                Console.Write("Enter Train ID to book: ");
                int trainId = int.Parse(Console.ReadLine());

                // Passenger details
                string[] names = new string[passengerCount];
                string[] ages = new string[passengerCount];
                string[] genders = new string[passengerCount];

                for (int i = 0; i < passengerCount; i++)
                {
                    Console.WriteLine($"\nPassenger {i + 1}:");
                    Console.Write(" Name: "); names[i] = Console.ReadLine();
                    Console.Write(" Age: "); ages[i] = Console.ReadLine();
                    Console.Write(" Gender (male/female/other): "); genders[i] = Console.ReadLine();
                }

                Console.Write("Payment Method (UPI/NetBanking/CreditCard/DebitCard): ");
                string paymentMethod = Console.ReadLine();
                string transactionId = Guid.NewGuid().ToString();

                // Prepare output parameters
                var bookingIdParam = new SqlParameter("@BookingId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var pnrParam = new SqlParameter("@PNRNumber", SqlDbType.NVarChar, 20) { Direction = ParameterDirection.Output };
                var amountParam = new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Direction = ParameterDirection.Output };
                var statusParam = new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Direction = ParameterDirection.Output };

                DataAccess.Instance.ExecuteNonQuery("sp_MakeBooking",
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

                Console.WriteLine($"\nBooking Status: {statusParam.Value}\nBooking PNR: {pnrParam.Value}\nAmount: {amountParam.Value}");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input format. Please try again.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}