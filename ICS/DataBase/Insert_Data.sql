-- ADMIN USER
--insert into users (name, email, phone, password_hash, user_type) 
--values ('System Admin', 'admin@railway.com', '9677396491', '12345678@', 'admin');

-- STATIONS
insert into stations (station_name, station_code, state) values
('Coimbatore', 'CBE', 'Tamil Nadu'),   -- 1
('Chennai', 'MAS', 'Tamil Nadu'),      -- 2
('Bangalore', 'SBC', 'Karnataka'),     -- 3
('Noida', 'NDLS', 'Uttar Pradesh'),    -- 4
('Vizag', 'VSKP', 'Andhra Pradesh')   -- 5

-- TRAINS - 20, bi-directional for each unique city pair (A->B and B->A)
-- Pair order: CBE-MAS, CBE-SBC, CBE-NDLS, CBE-VSKP, MAS-SBC, MAS-NDLS, MAS-VSKP, SBC-NDLS, SBC-VSKP, NDLS-VSKP
-- Coimbatore <-> Chennai
-- Insert 20 trains with correct column specification for your schema

ALTER TABLE trains ALTER COLUMN train_number NVARCHAR(20) NOT NULL;

insert into trains (
    train_number, train_name, source_station_id, destination_station_id,
    departure_time, arrival_time, running_days, total_seats,
    sleeper_seats, ac3_seats, ac2_seats,
    sleeper_fare, ac3_fare, ac2_fare
) values
-- Coimbatore <-> Chennai
('CBE-MAS-001', 'Coimbatore-Chennai Express', 1, 2, '07:00', '13:00', '1111111', 800, 600, 150, 50, 400.00, 700.00, 1100.00),
('MAS-CBE-001', 'Chennai-Coimbatore Express', 2, 1, '14:00', '20:00', '1111111', 800, 600, 150, 50, 400.00, 700.00, 1100.00),

-- Coimbatore <-> Bangalore
('CBE-SBC-001', 'Coimbatore-Bangalore Express', 1, 3, '08:00', '14:00', '1111100', 850, 650, 150, 50, 420.00, 720.00, 1150.00),
('SBC-CBE-001', 'Bangalore-Coimbatore Express', 3, 1, '15:00', '21:00', '1111100', 850, 650, 150, 50, 420.00, 720.00, 1150.00),

-- Coimbatore <-> Noida
('CBE-NDLS-001', 'Coimbatore-Noida SF', 1, 4, '09:00', '23:00', '1111111', 900, 700, 150, 50, 800.00, 1500.00, 2500.00),
('NDLS-CBE-001', 'Noida-Coimbatore SF', 4, 1, '10:00', '00:00', '1111111', 900, 700, 150, 50, 800.00, 1500.00, 2500.00),

-- Coimbatore <-> Vizag
('CBE-VSKP-001', 'Coimbatore-Vizag Express', 1, 5, '06:30', '19:00', '1111100', 800, 600, 150, 50, 700.00, 1300.00, 1800.00),
('VSKP-CBE-001', 'Vizag-Coimbatore Express', 5, 1, '20:00', '08:30', '1111100', 800, 600, 150, 50, 700.00, 1300.00, 1800.00),

-- Chennai <-> Bangalore
('MAS-SBC-001', 'Chennai-Bangalore SF', 2, 3, '07:00', '13:00', '1111111', 900, 650, 200, 50, 350.00, 750.00, 1200.00),
('SBC-MAS-001', 'Bangalore-Chennai SF', 3, 2, '14:00', '20:00', '1111111', 900, 650, 200, 50, 350.00, 750.00, 1200.00),

-- Chennai <-> Noida
('MAS-NDLS-001', 'Chennai-Noida Rajdhani', 2, 4, '17:00', '09:00', '1111111', 1200, 700, 300, 200, 800.00, 1500.00, 2500.00),
('NDLS-MAS-001', 'Noida-Chennai Rajdhani', 4, 2, '18:00', '10:00', '1111111', 1200, 700, 300, 200, 800.00, 1500.00, 2500.00),

-- Chennai <-> Vizag
('MAS-VSKP-001', 'Chennai-Vizag SF', 2, 5, '08:00', '20:00', '1111111', 1000, 700, 200, 100, 500.00, 900.00, 1400.00),
('VSKP-MAS-001', 'Vizag-Chennai SF', 5, 2, '21:00', '09:00', '1111111', 1000, 700, 200, 100, 500.00, 900.00, 1400.00),

-- Bangalore <-> Noida
('SBC-NDLS-001', 'Bangalore-Noida SF', 3, 4, '06:00', '22:00', '1111111', 950, 700, 200, 50, 800.00, 1500.00, 2500.00),
('NDLS-SBC-001', 'Noida-Bangalore SF', 4, 3, '07:00', '23:00', '1111111', 950, 700, 200, 50, 800.00, 1500.00, 2500.00),

-- Bangalore <-> Vizag
('SBC-VSKP-001', 'Bangalore-Vizag Express', 3, 5, '09:00', '21:00', '1111100', 850, 650, 150, 50, 650.00, 1250.00, 1750.00),
('VSKP-SBC-001', 'Vizag-Bangalore Express', 5, 3, '22:00', '10:00', '1111100', 850, 650, 150, 50, 650.00, 1250.00, 1750.00),

-- Noida <-> Vizag
('NDLS-VSKP-001', 'Noida-Vizag SF', 4, 5, '10:00', '00:00', '1111111', 950, 700, 200, 50, 1000.00, 1700.00, 2200.00),
('VSKP-NDLS-001', 'Vizag-Noida SF', 5, 4, '11:00', '01:00', '1111111', 950, 700, 200, 50, 1000.00, 1700.00, 2200.00);


--INSERT OF SEAT_AVAILABILITY
-- variables for date range
declare @fromdate date = cast(getdate() as date);
declare @todate date = dateadd(day, 119, @fromdate); -- 120 days

-- generate date series
with daterange as (
    select @fromdate as journey_date
    union all
    select dateadd(day, 1, journey_date)
    from daterange
    where journey_date < @todate
)
insert into seat_availability (train_id, journey_date, sleeper_available, ac3_available, ac2_available)
select t.train_id, d.journey_date, t.sleeper_seats, t.ac3_seats, t.ac2_seats
from trains t
cross join daterange d
where t.is_active = 1
    and not exists (
        select 1
        from seat_availability sa
        where sa.train_id = t.train_id
          and sa.journey_date = d.journey_date
    )
option (maxrecursion 130);


---- OPTIONAL ADMIN LOG FOR AUDIT
--insert into admin_logs (admin_id, action, target_table, details)
--values (1, 'Initial system setup: stations, trains, seat inventory', 'stations,trains,seat_availability', 'Seed data inserted for 5 stations, 20 trains, and seat inventory for next 3 days.');
