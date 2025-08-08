use railwayreservationdb
select * from users --1
select * from trains--2
select * from admin_logs --3
select * from bookings --4
select * from passengers --5
select * from payments --6
select * from seat_availability --7
select * from stations --8 




--> Total Number of Tables is "8"
--> 2,7,8 are hardcoded tables 


exec sp_helptext 'sp_makebooking'


DELETE FROM trains
WHERE train_id = 23;

SELECT * FROM seat_availability WHERE train_id = 21;

DELETE FROM seat_availability WHERE train_id = 22;
delete from bookings where booking_id=5
delete from payments where booking_id=5
DELETE FROM trains WHERE train_id = 22;


create database RRS_DB

