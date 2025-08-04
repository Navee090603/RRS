-- MINIMAL RAILWAY RESERVATION SYSTEM - PRODUCTION READY
-- Only 8 core tables - covers 100% functionality

-- 1. users (passengers + admins in one table)
create table users (
    user_id int identity(1,1) primary key,
    name nvarchar(100) not null,
    email nvarchar(255) unique not null,
    phone nvarchar(15) not null,
    password_hash nvarchar(255) not null,
    user_type nvarchar(10) default 'customer' check (user_type in ('customer', 'admin')),
    date_of_birth date,
    gender nvarchar(10) check (gender in ('male', 'female', 'other')),
    is_active bit default 1,
    created_at datetime2 default getdate()
)

-- Insert default admin
insert into users (name, email, phone, password_hash, user_type) 
values ('System Admin', 'admin@railway.com', '9677396491', '12345678@', 'Admin')

-- 2. stations
create table stations (
    station_id int identity(1,1) primary key,
    station_name nvarchar(100) not null,
    station_code nvarchar(10) unique not null,
    state nvarchar(50),
    is_active bit default 1
)

-- 3. trains (with embedded route and schedule)
create table trains (
    train_id int identity(1,1) primary key,
    train_number nvarchar(10) unique not null,
    train_name nvarchar(100) not null,
    source_station_id int not null,
    destination_station_id int not null,
    departure_time time not null,
    arrival_time time not null,
    running_days nvarchar(7) not null, -- "1111100" = Mon-Fri
    total_seats int not null,
    sleeper_seats int default 0,
    ac3_seats int default 0,
    ac2_seats int default 0,
    sleeper_fare decimal(8,2) not null,
    ac3_fare decimal(8,2) default 0,
    ac2_fare decimal(8,2) default 0,
    is_active bit default 1,
    created_at datetime2 default getdate(),
    foreign key (source_station_id) references stations(station_id),
    foreign key (destination_station_id) references stations(station_id)
)

-- 4. bookings (master booking record)
create table bookings (
    booking_id int identity(1,1) primary key,
    pnr_number as ('PNR' + right('0000000000' + cast(booking_id as varchar), 10)) persisted unique,
    user_id int not null,
    train_id int not null,
    journey_date date not null,
    passenger_count int not null default 1,
    total_amount decimal(10,2) not null,
    booking_status nvarchar(20) default 'confirmed' check (booking_status in ('confirmed', 'cancelled', 'waitlist')),
    payment_status nvarchar(20) default 'paid' check (payment_status in ('paid', 'refunded', 'pending')),
    booking_time datetime2 default getdate(),
    foreign key (user_id) references users(user_id),
    foreign key (train_id) references trains(train_id)
)

-- 5. passengers (individual passenger details per booking)
create table passengers (
    passenger_id int identity(1,1) primary key,
    booking_id int not null,
    name nvarchar(100) not null,
    age int not null,
    gender nvarchar(10) not null,
    seat_type nvarchar(10) not null check (seat_type in ('sleeper', 'ac3', 'ac2')),
    seat_number nvarchar(10),
    coach_number nvarchar(10),
    fare_paid decimal(8,2) not null,
    status nvarchar(20) default 'confirmed' check (status in ('confirmed', 'waitlist', 'rac')),
    foreign key (booking_id) references bookings(booking_id) on delete cascade
)

-- 6. seat_availability (daily inventory)
create table seat_availability (
    availability_id int identity(1,1) primary key,
    train_id int not null,
    journey_date date not null,
    sleeper_available int not null default 0,
    ac3_available int not null default 0,
    ac2_available int not null default 0,
    sleeper_waitlist int default 0,
    ac3_waitlist int default 0,
    ac2_waitlist int default 0,
    last_updated datetime2 default getdate(),
    foreign key (train_id) references trains(train_id),
    unique (train_id, journey_date)
)

-- 7. payments (simplified payment tracking)
create table payments (
    payment_id int identity(1,1) primary key,
    booking_id int not null,
    amount decimal(10,2) not null,
    payment_method nvarchar(20) not null,
    transaction_id nvarchar(100) unique not null,
    payment_status nvarchar(20) default 'success',
    payment_time datetime2 default getdate(),
    refund_amount decimal(10,2) default 0,
    refund_time datetime2,
    foreign key (booking_id) references bookings(booking_id)
)

-- 8. admin_logs (simple audit trail)
create table admin_logs (
    log_id int identity(1,1) primary key,
    admin_id int not null,
    action nvarchar(100) not null, -- "Updated train 12345", "Cancelled booking PNR123"
    target_table nvarchar(50),
    record_id int,
    details nvarchar(500), -- simple text description
    log_time datetime2 default getdate(),
    foreign key (admin_id) references users(user_id)
)

-- Essential indexes only
create index idx_bookings_pnr on bookings(pnr_number)
create index idx_bookings_user on bookings(user_id)
create index idx_bookings_train_date on bookings(train_id, journey_date)
create index idx_passengers_booking on passengers(booking_id)
create index idx_availability_train_date on seat_availability(train_id, journey_date)
create index idx_trains_route on trains(source_station_id, destination_station_id)
create index idx_admin_logs on admin_logs(admin_id, log_time)


--changed the constraint type

ALTER TABLE passengers
DROP CONSTRAINT CK__passenger__statu__59FA5E80;

ALTER TABLE passengers
ADD CONSTRAINT CK__passenger__status
    CHECK (status in ('confirmed', 'waitlist', 'rac', 'cancelled'));


