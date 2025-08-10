-- STATIONS

insert into stations (station_name, station_code, state) values
('Coimbatore', 'CBE', 'Tamil Nadu'),   -- 1
('Chennai', 'MAS', 'Tamil Nadu'),      -- 2
('Bangalore', 'SBC', 'Karnataka')     -- 3

--Trains

insert into trains (
  train_number,
  train_name,
  source_station_id,
  destination_station_id,
  departure_time,
  arrival_time,
  running_days,
  total_seats,
  sleeper_seats,
  ac3_seats,
  ac2_seats,
  sleeper_fare,
  ac3_fare,
  ac2_fare
) values

-- 3 trains with total_seats = 10 (5 sleeper / 3 AC3 / 2 AC2)
('CBE-MAS-001', 'Coimbatore–Chennai Express',      1, 2, '07:00', '13:00', '1111111', 10, 5, 3, 2, 400.00, 700.00, 1100.00),
('CBE-SBC-001', 'Coimbatore–Bangalore Express',     1, 3, '08:00', '14:00', '1111011', 10, 5, 3, 2, 450.00, 750.00, 1150.00),
('MAS-SBC-001', 'Chennai–Bangalore Express',        2, 3, '06:00', '12:00', '1111111', 10, 5, 3, 2, 420.00, 720.00, 1120.00),

-- 3 trains with total_seats = 50 (30 sleeper / 12 AC3 / 8 AC2)
('MAS-CBE-001', 'Chennai–Coimbatore Return',        2, 1, '14:00', '20:00', '1110111', 50, 30, 12, 8, 400.00, 700.00, 1100.00),
('SBC-CBE-001', 'Bangalore–Coimbatore Return',      3, 1, '15:00', '21:00', '1011111', 50, 30, 12, 8, 450.00, 750.00, 1150.00),
('SBC-MAS-001', 'Bangalore–Chennai Return',         3, 2, '13:00', '19:00', '1111101', 50, 30, 12, 8, 420.00, 720.00, 1120.00);


-- Insert default admin
insert into users (name, email, phone, password_hash, user_type) 
values ('System Admin', 'naveen@railway.com', '9677396491', '10', 'Admin')