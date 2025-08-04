use railwayreservationdb
select * from users --1
select * from trains --2
select * from admin_logs --3
select * from bookings --4
select * from passengers --5
select * from payments --6
select * from seat_availability --7
select * from stations --8

--> Total Number of Tables is "8"
--> 2,7,8 are hardcoded tables 

--For Active and InActive

--EXEC sp_setuseractive @userid = 2, @active = 0;  -- Deactivate user_id 2
EXEC sp_setuseractive @userid = 2, @active = 1;  -- Reactivate user_id 2
