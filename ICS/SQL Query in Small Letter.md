<img src="https://r2cdn.perplexity.ai/pplx-full-logo-primary-dark%402x.png" class="logo" width="120"/>

# give the sql querry in small letter

Certainly! Here is the SQL script from the previous conversation converted to **all lowercase** for you:

```sql
create database railwayreservationdb;
go
use railwayreservationdb;
```

```sql
create table users (
    userid int primary key identity(1,1),
    name varchar(100),
    email varchar(100) unique,
    password varchar(255)
);
create table trains (
    trainid int primary key identity(1,1),
    trainno varchar(10),
    name varchar(100),
    source varchar(50),
    destination varchar(50),
    departuretime time,
    arrivaltime time,
    seatsavailable int
);
create table bookings (
    bookingid int primary key identity(1,1),
    userid int foreign key references users(userid),
    trainid int foreign key references trains(trainid),
    bookingdate datetime,
    noofseats int
);
create table passengers (
    passengerid int primary key identity(1,1),
    bookingid int foreign key references bookings(bookingid),
    name varchar(100),
    age int,
    gender varchar(10)
);
```

```sql
create procedure addtrain
    @trainno varchar(10),
    @name varchar(100),
    @source varchar(50),
    @destination varchar(50),
    @departuretime time,
    @arrivaltime time,
    @seatsavailable int
as
begin
    insert into trains (trainno, name, source, destination, departuretime, arrivaltime, seatsavailable)
    values (@trainno, @name, @source, @destination, @departuretime, @arrivaltime, @seatsavailable);
end
```

Let me know if you need any other scripts converted or further help!

<div style="text-align: center">⁂</div>

[^1]: give-me-the-full-code-for-all-paths.md

