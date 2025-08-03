using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace RailwayReservationConsole
{
    class Program
    {
        static DataAccess db = new DataAccess();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n=== Railway Reservation System ===");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Book Ticket (requires login)");
                Console.WriteLine("4. View My Bookings (requires login)");
                Console.WriteLine("5. Cancel Booking (requires login)");
                Console.WriteLine("6. Exit");
                Console.Write("Select option: ");
                string opt = Console.ReadLine();

                try
                {
                    switch (opt)
                    {
                        case "1":
                            RegisterUser();
                            break;
                        case "2":
                            LoginUser();
                            break;
                        case "3":
                            BookTicket();
                            break;
                        case "4":
                            ViewBookings();
                            break;
                        case "5":
                            CancelBooking();
                            break;
                        case "6":
                            return;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
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
                    Console.WriteLine($"General Error: {ex.Message}");
                }
            }
        }

        static int? loggedInUserId = null;
        static string loggedInUserName = null;

        static void RegisterUser()
        {
            try
            {
                Console.Write("Name: "); string name = Console.ReadLine();
                Console.Write("Email: "); string email = Console.ReadLine();
                Console.Write("Phone: "); string phone = Console.ReadLine();
                Console.Write("Password: "); string password = Console.ReadLine();
                Console.Write("Date of Birth (YYYY-MM-DD): "); string dobStr = Console.ReadLine();
                Console.Write("Gender (male/female/other): "); string gender = Console.ReadLine();

                DateTime dob;
                if (!DateTime.TryParse(dobStr, out dob))
                    dob = DateTime.Now;

                var dt = db.ExecuteTable("sp_registeruser",
                    new SqlParameter("@name", name),
                    new SqlParameter("@email", email),
                    new SqlParameter("@phone", phone),
                    new SqlParameter("@password", password),
                    new SqlParameter("@dateofbirth", dob),
                    new SqlParameter("@gender", gender)
                );

                if (dt.Rows.Count > 0)
                {
                    var success = Convert.ToInt32(dt.Rows[0]["success"]);
                    var message = dt.Rows[0]["message"].ToString();
                    Console.WriteLine(message);
                }
                else
                {
                    Console.WriteLine("Registration failed. Please try again.");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                    Console.WriteLine("Email already exists.");
                else
                    Console.WriteLine($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void LoginUser()
        {
            try
            {
                Console.Write("Email: "); string email = Console.ReadLine();
                Console.Write("Password: "); string password = Console.ReadLine();

                var dt = db.ExecuteTable("sp_loginuser",
                    new SqlParameter("@email", email),
                    new SqlParameter("@password", password)
                );

                if (dt.Rows.Count > 0)
                {
                    var success = Convert.ToInt32(dt.Rows[0]["success"]);
                    var message = dt.Rows[0]["message"].ToString();
                    if (success == 1)
                    {
                        loggedInUserId = Convert.ToInt32(dt.Rows[0]["user_id"]);
                        loggedInUserName = dt.Rows[0]["name"].ToString();
                        Console.WriteLine($"Login successful. Welcome, {loggedInUserName}!");
                    }
                    else
                    {
                        Console.WriteLine($"Login failed: {message}");
                        loggedInUserId = null;
                        loggedInUserName = null;
                    }
                }
                else
                {
                    Console.WriteLine("Login failed. Please check your credentials.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void BookTicket()
        {
            if (loggedInUserId == null)
            {
                Console.WriteLine("Please login first to book tickets.");
                return;
            }

            try
            {
                // Show stations
                var stations = db.ExecuteTable("sp_getallstations");
                Console.WriteLine("\nStations:");
                foreach (DataRow row in stations.Rows)
                {
                    Console.WriteLine($"{row["station_id"]} - {row["station_name"]} ({row["station_code"]})");
                }
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
                var trains = db.ExecuteTable("sp_searchtrains",
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
                {
                    Console.WriteLine($"TrainID: {row["train_id"]}, Name: {row["train_name"]}, Dep:{row["departure_time"]}, Arr:{row["arrival_time"]}, Seats:{row["available_seats"]}, Fare/Person:{row["fare_per_passenger"]}, Status:{row["booking_status"]}");
                }

                Console.Write("Enter Train ID to book: ");
                int trainId = int.Parse(Console.ReadLine());

                // Passenger details
                string[] names = new string[passengerCount];
                string[] ages = new string[passengerCount];
                string[] genders = new string[passengerCount];
                for(int i = 0; i < passengerCount; i++)
                {
                    Console.WriteLine($"\nPassenger {i+1}:");
                    Console.Write("  Name: "); names[i] = Console.ReadLine();
                    Console.Write("  Age: "); ages[i] = Console.ReadLine();
                    Console.Write("  Gender (male/female/other): "); genders[i] = Console.ReadLine();
                }

                Console.Write("Payment Method (UPI/NetBanking/CreditCard/DebitCard): ");
                string paymentMethod = Console.ReadLine();
                string transactionId = Guid.NewGuid().ToString();

                // Prepare output parameters
                var bookingIdParam = new SqlParameter("@BookingId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var pnrParam = new SqlParameter("@PNRNumber", SqlDbType.NVarChar, 20) { Direction = ParameterDirection.Output };
                var amountParam = new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Direction = ParameterDirection.Output };
                var statusParam = new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuery("sp_MakeBooking",
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

        static void ViewBookings()
        {
            if (loggedInUserId == null)
            {
                Console.WriteLine("Please login first to view bookings.");
                return;
            }

            try
            {
                var ds = db.ExecuteDataSet("sp_GetUserBookings",
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

                Console.WriteLine("\nYour Bookings:");
                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"PNR: {row["pnr_number"]}, Train: {row["train_name"]}, Date: {row["journey_date"]}, Status: {row["booking_status"]}, Amount: {row["total_amount"]}");
                }
                Console.WriteLine($"Total: {ds.Tables[1].Rows[0]["totalrecords"]}");
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

        static void CancelBooking()
        {
            if (loggedInUserId == null)
            {
                Console.WriteLine("Please login first to cancel a booking.");
                return;
            }

            try
            {
                Console.Write("Enter PNR to cancel: ");
                string pnr = Console.ReadLine();
                Console.Write("Reason for cancellation: ");
                string reason = Console.ReadLine();

                var dt = db.ExecuteTable("sp_CancelBooking",
                    new SqlParameter("@PNRNumber", pnr),
                    new SqlParameter("@UserId", loggedInUserId),
                    new SqlParameter("@CancellationReason", reason)
                );

                if (dt.Rows.Count > 0)
                {
                    Console.WriteLine($"{dt.Rows[0]["Message"]}\nRefund: {dt.Rows[0]["RefundAmount"]}");
                }
                else
                {
                    Console.WriteLine("Cancellation failed: No such booking.");
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

    public class DataAccess
    {
        private readonly string _connStr;

        public DataAccess()
        {
            _connStr = ConfigurationManager.ConnectionStrings["RailwayDb"].ConnectionString;
        }

        public DataTable ExecuteTable(string procName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        public DataSet ExecuteDataSet(string procName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                return ds;
            }
        }

        public int ExecuteNonQuery(string procName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string procName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}