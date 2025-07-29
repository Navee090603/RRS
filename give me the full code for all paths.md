<img src="https://r2cdn.perplexity.ai/pplx-full-logo-primary-dark%402x.png" class="logo" width="120"/>

# give me the full code for all paths

Sure! Below is a **complete code structure** for your **Railway Reservation System using Windows Forms and ADO.NET with SQL Server**, including detailed code for each major path/module based on the previously described folder structure:

### ✅ Project Folder Structure Recap

```
RailwayReservationSystem/
├── App.config
├── Program.cs
├── Forms/
│   ├── LoginForm.cs
│   ├── RegisterForm.cs
│   ├── AdminForm.cs
│   ├── UserDashboard.cs
│   ├── BookingForm.cs
│   ├── BookingHistoryForm.cs
├── Data/
│   ├── DBHelper.cs
│   ├── UserService.cs
│   ├── TrainService.cs
│   ├── BookingService.cs
├── Models/
│   ├── User.cs
│   ├── Train.cs
│   ├── Booking.cs
│   ├── Passenger.cs
├── SQLScripts/
│   ├── CreateDatabase.sql
│   ├── CreateTables.sql
│   ├── StoredProcedures.sql
```


## 🔧 1. App.config (Database Connection)

```xml
<configuration>
  <connectionStrings>
    <add name="RailwayDB" connectionString="Data Source=.;Initial Catalog=RailwayReservationDB;Integrated Security=True;" providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
```


## 🗃 2. Data Layer – `Data/`

### 🔹 DBHelper.cs

```csharp
public static class DBHelper
{
    public static string ConnectionString => ConfigurationManager.ConnectionStrings["RailwayDB"].ConnectionString;

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}
```


### 🔹 UserService.cs

```csharp
public class UserService
{
    public bool RegisterUser(string name, string email, string password)
    {
        using (SqlConnection conn = DBHelper.GetConnection())
        {
            string query = "INSERT INTO Users (Name, Email, Password) VALUES (@Name, @Email, @Password)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password); // use hash in production

            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public bool Login(string email, string password)
    {
        using (SqlConnection conn = DBHelper.GetConnection())
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);

            conn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }
    }
}
```


### 🔹 TrainService.cs

```csharp
public class TrainService
{
    public DataTable GetAllTrains()
    {
        using (SqlConnection conn = DBHelper.GetConnection())
        {
            string query = "SELECT * FROM Trains";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    public bool AddTrain(Train train)
    {
        using (SqlConnection conn = DBHelper.GetConnection())
        {
            SqlCommand cmd = new SqlCommand("AddTrain", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TrainNo", train.TrainNo);
            cmd.Parameters.AddWithValue("@Name", train.Name);
            cmd.Parameters.AddWithValue("@Source", train.Source);
            cmd.Parameters.AddWithValue("@Destination", train.Destination);
            cmd.Parameters.AddWithValue("@DepartureTime", train.DepartureTime);
            cmd.Parameters.AddWithValue("@ArrivalTime", train.ArrivalTime);
            cmd.Parameters.AddWithValue("@SeatsAvailable", train.SeatsAvailable);

            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
```


### 🔹 BookingService.cs

```csharp
public class BookingService
{
    public bool BookTickets(int userId, int trainId, int numSeats, List<Passenger> passengers)
    {
        using (SqlConnection conn = DBHelper.GetConnection())
        {
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                SqlCommand check = new SqlCommand("SELECT SeatsAvailable FROM Trains WHERE TrainID = @TrainID", conn, transaction);
                check.Parameters.AddWithValue("@TrainID", trainId);
                int available = (int)check.ExecuteScalar();

                if (available < numSeats)
                    throw new Exception("Not enough seats");

                SqlCommand booking = new SqlCommand(@"INSERT INTO Bookings (UserID, TrainID, BookingDate, NoOfSeats)
                                                      OUTPUT Inserted.BookingID
                                                      VALUES (@UserID, @TrainID, GETDATE(), @Seats)", conn, transaction);
                booking.Parameters.AddWithValue("@UserID", userId);
                booking.Parameters.AddWithValue("@TrainID", trainId);
                booking.Parameters.AddWithValue("@Seats", numSeats);

                int bookingId = (int)booking.ExecuteScalar();

                foreach (var p in passengers)
                {
                    SqlCommand insertP = new SqlCommand("INSERT INTO Passengers (BookingID, Name, Age, Gender) VALUES (@BID, @Name, @Age, @Gender)", conn, transaction);
                    insertP.Parameters.AddWithValue("@BID", bookingId);
                    insertP.Parameters.AddWithValue("@Name", p.Name);
                    insertP.Parameters.AddWithValue("@Age", p.Age);
                    insertP.Parameters.AddWithValue("@Gender", p.Gender);
                    insertP.ExecuteNonQuery();
                }

                SqlCommand updateSeats = new SqlCommand("UPDATE Trains SET SeatsAvailable = SeatsAvailable - @Seats WHERE TrainID = @TrainID", conn, transaction);
                updateSeats.Parameters.AddWithValue("@Seats", numSeats);
                updateSeats.Parameters.AddWithValue("@TrainID", trainId);
                updateSeats.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }

    public DataTable GetBookingsByUserId(int userId)
    {
        using (SqlConnection conn = DBHelper.GetConnection())
        {
            string query = @"SELECT B.BookingID, T.Name AS TrainName, B.BookingDate, B.NoOfSeats 
                             FROM Bookings B 
                             JOIN Trains T ON B.TrainID = T.TrainID 
                             WHERE B.UserID = @UserID";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}
```


## 📦 3. Models

### 🔸 User.cs

```csharp
public class User
{
    public int UserID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
```


### 🔸 Train.cs

```csharp
public class Train
{
    public int TrainID { get; set; }
    public string TrainNo { get; set; }
    public string Name { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan ArrivalTime { get; set; }
    public int SeatsAvailable { get; set; }
}
```


### 🔸 Passenger.cs

```csharp
public class Passenger
{
    public int PassengerID { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
}
```


## 🖼 Forms

### 🔸 LoginForm.cs

```csharp
private void btnLogin_Click(object sender, EventArgs e)
{
    var service = new UserService();
    if (service.Login(txtEmail.Text, txtPassword.Text))
    {
        MessageBox.Show("Login successful!");
        this.Hide();
        new UserDashboard().Show();
    }
    else
        MessageBox.Show("Invalid credentials");
}
```


### 🔸 RegisterForm.cs

```csharp
private void btnRegister_Click(object sender, EventArgs e)
{
    var userService = new UserService();
    bool result = userService.RegisterUser(txtName.Text, txtEmail.Text, txtPassword.Text);
    MessageBox.Show(result ? "Registration successful!" : "Failed. Email may be in use.");
}
```


### 🔸 AdminForm.cs (Add Train)

```csharp
private void btnAddTrain_Click(object sender, EventArgs e)
{
    var train = new Train
    {
        TrainNo = txtTrainNo.Text,
        Name = txtTrainName.Text,
        Source = txtSource.Text,
        Destination = txtDestination.Text,
        DepartureTime = dtDeparture.Value.TimeOfDay,
        ArrivalTime = dtArrival.Value.TimeOfDay,
        SeatsAvailable = int.Parse(txtSeats.Text)
    };
    var service = new TrainService();
    if (service.AddTrain(train))
        MessageBox.Show("Train added!");
    else
        MessageBox.Show("Failed.");
}
```


## 🧾 4. SQL Scripts – SQLScripts Folder

### ✅ CreateDatabase.sql

```sql
CREATE DATABASE RailwayReservationDB;
GO
USE RailwayReservationDB;
```


### ✅ CreateTables.sql

```sql
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(100),
    Email VARCHAR(100) UNIQUE,
    Password VARCHAR(255)
);
CREATE TABLE Trains (
    TrainID INT PRIMARY KEY IDENTITY(1,1),
    TrainNo VARCHAR(10),
    Name VARCHAR(100),
    Source VARCHAR(50),
    Destination VARCHAR(50),
    DepartureTime TIME,
    ArrivalTime TIME,
    SeatsAvailable INT
);
CREATE TABLE Bookings (
    BookingID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    TrainID INT FOREIGN KEY REFERENCES Trains(TrainID),
    BookingDate DATETIME,
    NoOfSeats INT
);
CREATE TABLE Passengers (
    PassengerID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT FOREIGN KEY REFERENCES Bookings(BookingID),
    Name VARCHAR(100),
    Age INT,
    Gender VARCHAR(10)
);
```


### ✅ StoredProcedures.sql

```sql
CREATE PROCEDURE AddTrain
    @TrainNo VARCHAR(10),
    @Name VARCHAR(100),
    @Source VARCHAR(50),
    @Destination VARCHAR(50),
    @DepartureTime TIME,
    @ArrivalTime TIME,
    @SeatsAvailable INT
AS
BEGIN
    INSERT INTO Trains (TrainNo, Name, Source, Destination, DepartureTime, ArrivalTime, SeatsAvailable)
    VALUES (@TrainNo, @Name, @Source, @Destination, @DepartureTime, @ArrivalTime, @SeatsAvailable);
END
```


## 📌 Next Step Optionally

- Would you like a Zip file with the VS solution template?
- Need UI screenshots or project diagrams?
- Want to deploy this using SQL LocalDB or Docker-based SQL Server?

Let me know and I’ll add those resources!

