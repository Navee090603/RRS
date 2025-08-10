# Railway Reservation System (RRS) - Project Report

---

## 1. Overview

The Railway Reservation System (RRS) is a console-based C# application designed to automate and manage railway ticket booking, user administration, payments, and reporting. The application provides separate modules for Users and Admins, ensuring all essential functionalities for a real-world railway booking system.

---

## 2. Technologies Used

- **C# (.NET Console Application):** Main programming language and platform.
- **ADO.NET:** Used for all database operations (SQL queries, stored procedures, transactions).
- **SQL Server:** Backend database, accessed via ADO.NET.
- **Modular Design with Factory Pattern:** Each feature is implemented as a class and executed via a central factory (`All_Factory.cs`).
- **Console UI:** User-friendly text-based interface with tabular data display.

---

## 3. Main Features

### User Module
- **Registration & Login:** Secure registration with input validation; login using email and password.
- **Book Ticket:** Search for trains, choose seat types, enter journey details, add passenger info, and confirm booking.
- **View Bookings:** Display all bookings made by the user, with details like PNR, train, journey date, status, amount, and passenger count.
- **Download Ticket:** Generate and save ticket as a .txt file using PNR.
- **Cancel Booking:** Cancel a ticket for a specific passenger in a booking, supporting refunds if eligible.
- **Logout:** End user session securely.

### Admin Module
- **Admin Login:** Authentication for admin users.
- **User Management:** View all users, activate or deactivate accounts.
- **Train Management:** Add new trains, view all trains with complete details.
- **Booking Management:** View all bookings, see status, amount, refund, user info.
- **Payment Management:** Track all payments, including payment method, status, and refunds.
- **Seat Availability:** Check seat availability for any train on a specific date.
- **Station Management:** View all available stations.
- **Revenue Reports:** Generate reports by date or train, showing bookings, passengers, revenue, refunds, and average value.
- **Passenger Details:** View all passengers or filter by booking.

---

## 4. How the System Works

- The system starts with a main menu (`Program.cs`) for registration, login, or exit.
- Upon login, users or admins are routed to their respective menus.
- All operations (booking, viewing, reporting) interact with the database via a singleton data access class (`DataAccess.cs`).
- Features are modular; each menu option triggers a class via the factory pattern for separation and maintainability.
- Data is always validated before database operations.

---

## 5. Example Workflow

### **User:**
- Register → Login
- Book Ticket: Select stations, seat type, date, train, enter passenger info, make payment.
- View & Download Ticket: Access current/past bookings; download ticket as a text file.
- Cancel Booking: Select PNR and passenger, confirm cancellation, receive refund if eligible.

### **Admin:**
- Login
- Manage Users: View, activate or deactivate.
- Manage Trains: Add/view all trains.
- View Bookings/Payments: Monitor all transactions and bookings.
- Check Seat Availability: For any train and date.
- Generate Revenue Reports: By date or train, with all relevant financial statistics.
- Logout

---

## 6. Code Structure

- **Program.cs:** Main menu and workflow controller.
- **DataAccess.cs:** All SQL and database logic through ADO.NET (`SqlConnection`, `SqlCommand`, etc.).
- **Feature Classes:** Each feature (e.g., booking, registration, reporting) is its own class for clarity.
- **Factory Classes:** Each menu option is routed to a feature via the factory pattern, supporting clean modularity.

---

## 7. Database Operations

- All database interactions use ADO.NET for reliability and performance.
- Most actions use stored procedures for security and efficiency.
- Data validation is performed before any write operation.

---

## 8. Ticket Example

A downloaded ticket appears as follows:

```
=== Railway Reservation Ticket ===
PNR Number          : PNR0005
User Name           : Karunya
Train               : Coimbatore–Chennai Express (CBE-MAS-001)
Journey Date        : 2025-10-11
Booking Time        : 2025-08-09 23:57
Status              : confirmed
Total Amount        : ₹800.00

Passengers:
--------------------------------------------------------------------------------------------
Name                Age  Gender Type      Coach  Seat    Fare      Status    
---------------------------------------------------------------------------------------------
Naveen              22   male   sleeper               	₹400.00  confirmed  
Karunya             21   female sleeper               	₹400.00  confirmed  
---------------------------------------------------------------------------------------------
```

---

## 9. Summary

- User-friendly, secure, and robust railway reservation system.
- All database work is powered by ADO.NET with proper error handling.
- Features are easy to extend, thanks to modular and factory-based design.
- Admins have full control over trains, users, bookings, payments, and reporting.
- Users can easily book, view, download, and cancel tickets.

---

## 10. Conclusion

This project is a complete solution for railway reservations, suitable for learning, demonstration, or adaptation to real-world railway systems.

---

## Project Scope: Railway Reservation System

This project outlines the development of a console-based Railway Reservation System with distinct user and administrative functionalities, supported by specific technical considerations.

### 1. User Module:

#### Account Management:
- **User Registration:** Create new user accounts with personal details.
- **User Login:** Secure access for registered users.
- **Logout:** End user sessions.

#### Booking Operations:
- **Book Ticket:** Select source/destination, train, seat type, and date; input passenger details.
- **View Bookings:** Access current and historical booking information.
- **Download Ticket:** Obtain tickets as a text file.
- **Cancel Ticket/Booking:** Initiate cancellations for specific passengers within a booking, including refund processing (if applicable).

### 2. Admin Module:

#### Administrative Access:
- **Admin Login:** Secure access for administrators.
- **Logout:** End admin sessions.

#### System Management:
- **User Management:** Oversee user accounts (view, activate, deactivate).
- **Train Management:** Add new trains and view existing train details.
- **Station Management:** View all registered stations.
- **Seat Availability:** Check seat status for trains on specified dates.

#### Reporting & Oversight:
- **Booking Management:** View all system-wide bookings.
- **Payment Management:** Monitor all payments and refunds.
- **Passenger Details:** Access passenger information for individual bookings or the entire system.
- **Revenue Reports:** Generate reports based on date or train, detailing bookings, refunds, and net revenue.

### 3. Technical Scope:

- **Application Type:** Console-based (no Graphical User Interface).
- **Design Principles:** Modular, feature-based design utilizing the Factory Pattern.
- **Database Interaction:** All database operations will be performed using ADO.NET with SQL Server.
- **Data Integrity:** Implementation of comprehensive data validation for all inputs.
- **Error Management:** Robust error handling and feedback mechanisms for both users and administrators.

### 4. Out of Scope:

- **Interface:** No web or mobile interfaces are planned.
- **Advanced Features:** Does not include advanced analytics, predictive functionalities, or machine learning.
- **Localization:** No support for multiple languages or internationalization.
- **Payment Integration:** Real payment integration is not included; transactions will be simulated within the database.

---

## Attached Content

### Database Structure

**DataBase Diagram**

<img width="900" height="1050" alt="image" src="https://github.com/user-attachments/assets/0cd221d6-20a7-49e3-aca6-da93d100dbd6" />

**Tables in RRS_DB:**

<img width="182" height="214" alt="Tables" src="https://github.com/user-attachments/assets/e92ee125-5a17-475e-aa23-268d281ff0b4" />

- `dbo.admin_logs`
- `dbo.bookings`
- `dbo.passengers`
- `dbo.payments`
- `dbo.seat_availability`
- `dbo.stations`
- `dbo.trains`
- `dbo.users`

The above tables represent the core entities of the Railway Reservation System. They support user management, booking and passenger details, payment and refund tracking, seat inventory, station details, and an audit trail for admin actions.

---

### Stored Procedures

<img width="169" height="155" alt="Procedure_Images" src="https://github.com/user-attachments/assets/8f9fb191-d37f-435f-9e06-0856c3afa4a8" />

Core stored procedures implemented in SQL Server for RRS_DB:
- `dbo.sp_cancelbooking`: Handles cancellation and refund logic for bookings.
- `dbo.sp_getallstations`: Retrieves all active stations.
- `dbo.sp_getrevenuereport`: Generates revenue reports by date or train.
- `dbo.sp_getuserbookings`: Fetches bookings for a specific user.
- `dbo.sp_loginuser`: Authenticates users.
- `dbo.sp_makebooking`: Handles ticket booking, seat allocation, and payment recording.
- `dbo.sp_registeruser`: Registers new users.
- `dbo.sp_searchtrains`: Searches available trains based on journey criteria.
- `dbo.sp_setuseractive`: Admin activation/deactivation of user accounts.

These stored procedures ensure security, transactional integrity, and efficient database operations.

---

### Functions

<img width="208" height="32" alt="Function_Image" src="https://github.com/user-attachments/assets/2127db80-8acc-4544-8938-e8360bbcf658" />

**Scalar-valued Function:**

- `dbo.fn_isvalidjourneydate`: Checks if a journey date is valid for a given train, verifying against its schedule and current date.

This function is essential for ensuring bookings can only be made for valid journey dates.

---

## Complete SQL Implementation References

For detailed SQL table definitions, sample data insertion, stored procedures, triggers, and functions, please refer to the following attached scripts:

- `Create_Table.sql`
- `Insert_Table.sql`
- `Procedure_Function_Trigger.sql`

These scripts provide the full backend schema and logic for the Railway Reservation System.

---

## Final Notes

This comprehensive report, along with the attached SQL scripts and images, documents the design, features, workflow, database structure, and technical scope of the Railway Reservation System (RRS) project. It is ready for evaluation, demonstration, or further development as a real-world railway booking solution.
