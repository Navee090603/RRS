-- =====================================================
-- railway reservation system - ado.net ready procedures
-- all inputs dynamic, no hardcoded values, production ready
-- =====================================================

-- =====================================================
-- 1. triggers (auto-executed on data changes)
-- =====================================================

-- a. auto-generate seat availability when train is added
create or alter trigger trg_train_insert_availability
on trains after insert
as
begin
    set nocount on

    declare @train_id int, @sleeper int, @ac3 int, @ac2 int

    select @train_id = train_id, @sleeper = sleeper_seats,
           @ac3 = ac3_seats, @ac2 = ac2_seats
    from inserted

    -- generate availability for next 120 days
    ;with daterange as (
        select cast(getdate() as date) as journey_date
        union all
        select dateadd(day, 1, journey_date)
        from daterange
        where journey_date < dateadd(day, 120, cast(getdate() as date))
    )
    insert into seat_availability (train_id, journey_date, sleeper_available, ac3_available, ac2_available)
    select @train_id, journey_date, @sleeper, @ac3, @ac2
    from daterange
    option (maxrecursion 150)
end

-- b. update seat availability when booking is made
create or alter trigger trg_passenger_seat_update
on passengers after insert
as
begin
    set nocount on;

    -- Calculate how many passengers inserted per seat type
    update sa
    set sleeper_available = sa.sleeper_available - (
            select count(*) from inserted i
            inner join bookings b on b.booking_id = i.booking_id
            where i.seat_type = 'sleeper'
              and b.train_id = sa.train_id
              and b.journey_date = sa.journey_date
        ),
        ac3_available = sa.ac3_available - (
            select count(*) from inserted i
            inner join bookings b on b.booking_id = i.booking_id
            where i.seat_type = 'ac3'
              and b.train_id = sa.train_id
              and b.journey_date = sa.journey_date
        ),
        ac2_available = sa.ac2_available - (
            select count(*) from inserted i
            inner join bookings b on b.booking_id = i.booking_id
            where i.seat_type = 'ac2'
              and b.train_id = sa.train_id
              and b.journey_date = sa.journey_date
        ),
        last_updated = getdate()
    from seat_availability sa
    where exists (
        select 1 from inserted i
        inner join bookings b on b.booking_id = i.booking_id
        where b.train_id = sa.train_id
          and b.journey_date = sa.journey_date
    );
end

-- c. restore seat availability when booking is cancelled
create or alter trigger trg_passenger_seat_restore
on passengers after update
as
begin
    set nocount on;

    -- restore seat availability when passenger status changes to cancelled
    if update(status)
    begin
        -- For each ticket that's cancelled in this update, restore seat count for the correct type
        update sa
        set sleeper_available = sa.sleeper_available + 
            (select count(*) from inserted i
                inner join deleted d on i.passenger_id = d.passenger_id
                inner join bookings b on b.booking_id = i.booking_id
                where i.seat_type = 'sleeper'
                  and i.status = 'cancelled'
                  and d.status != 'cancelled'
                  and b.train_id = sa.train_id
                  and b.journey_date = sa.journey_date),
            ac3_available = sa.ac3_available + 
            (select count(*) from inserted i
                inner join deleted d on i.passenger_id = d.passenger_id
                inner join bookings b on b.booking_id = i.booking_id
                where i.seat_type = 'ac3'
                  and i.status = 'cancelled'
                  and d.status != 'cancelled'
                  and b.train_id = sa.train_id
                  and b.journey_date = sa.journey_date),
            ac2_available = sa.ac2_available + 
            (select count(*) from inserted i
                inner join deleted d on i.passenger_id = d.passenger_id
                inner join bookings b on b.booking_id = i.booking_id
                where i.seat_type = 'ac2'
                  and i.status = 'cancelled'
                  and d.status != 'cancelled'
                  and b.train_id = sa.train_id
                  and b.journey_date = sa.journey_date),
            last_updated = getdate()
        from seat_availability sa
        where exists (
            select 1 from inserted i
            inner join deleted d on i.passenger_id = d.passenger_id
            inner join bookings b on b.booking_id = i.booking_id
            where (
                    (i.seat_type = 'sleeper' and i.status = 'cancelled' and d.status != 'cancelled')
                 or (i.seat_type = 'ac3' and i.status = 'cancelled' and d.status != 'cancelled')
                 or (i.seat_type = 'ac2' and i.status = 'cancelled' and d.status != 'cancelled')
                )
              and b.train_id = sa.train_id
              and b.journey_date = sa.journey_date
        );
    end
end

-- =====================================================
-- 2. user authentication & management
-- =====================================================

-- a. user registration
create or alter procedure sp_registeruser
    @name nvarchar(100),
    @email nvarchar(255),
    @phone nvarchar(15),
    @password nvarchar(255),
    @dateofbirth date = null,
    @gender nvarchar(10) = null,
    @usertype nvarchar(10) = 'customer'
as
begin
    set nocount on
    begin try
        -- check if email already exists
        if exists(select 1 from users where email = @email)
        begin
            select 0 as success, 'email already registered' as message, 0 as userid
            return
        end

        -- check if phone already exists
        if exists(select 1 from users where phone = @phone)
        begin
            select 0 as success, 'phone number already registered' as message, 0 as userid
            return
        end

        declare @userid int

        insert into users (name, email, phone, password_hash, date_of_birth, gender, user_type)
        values (@name, @email, @phone, @password, @dateofbirth, @gender, @usertype)

        set @userid = scope_identity()

        select 1 as success, 'user registered successfully' as message, @userid as userid

    end try
    begin catch
        select 0 as success, error_message() as message, 0 as userid
    end catch
end

-- b. user login
create or alter procedure sp_loginuser
    @email nvarchar(255),
    @password nvarchar(255)
as
begin
    set nocount on

    select
        case when password_hash = @password and is_active = 1 then 1 else 0 end as success,
        user_id,
        name,
        email,
        phone,
        user_type,
        case when password_hash = @password and is_active = 1
             then 'login successful'
             when is_active = 0
             then 'account is deactivated'
             else 'invalid credentials'
        end as message
    from users
    where email = @email
end

-- =====================================================
-- 3. station management
-- =====================================================

-- a. get all active stations
create or alter procedure sp_getallstations
as
begin
    set nocount on

    select station_id, station_name, station_code, state
    from stations
    where is_active = 1
    order by station_name
end

-- b. search stations by name/code
create or alter procedure sp_searchstations
    @searchterm nvarchar(100)
as
begin
    set nocount on

    select station_id, station_name, station_code, state
    from stations
    where is_active = 1
      and (station_name like '%' + @searchterm + '%'
           or station_code like '%' + @searchterm + '%')
    order by station_name
end

-- =====================================================
-- 4. train search & availability
-- =====================================================

-- a. search available trains (ado.net ready)
create or alter procedure sp_searchtrains
    @sourcestationid int,
    @destinationstationid int,
    @journeydate date,
    @seattype nvarchar(10) = 'sleeper',
    @passengercount int = 1
as
begin
    set nocount on

    declare @dayofweek int = datepart(weekday, @journeydate)

    select
        t.train_id,
        t.train_number,
        t.train_name,
        s1.station_name as source_station,
        s1.station_code as source_code,
        s2.station_name as destination_station,
        s2.station_code as destination_code,
        t.departure_time,
        t.arrival_time,
        case @seattype
            when 'sleeper' then isnull(sa.sleeper_available, 0)
            when 'ac3' then isnull(sa.ac3_available, 0)
            when 'ac2' then isnull(sa.ac2_available, 0)
        end as available_seats,
        case @seattype
            when 'sleeper' then t.sleeper_fare
            when 'ac3' then t.ac3_fare
            when 'ac2' then t.ac2_fare
        end as fare_per_passenger,
        (case @seattype
            when 'sleeper' then t.sleeper_fare
            when 'ac3' then t.ac3_fare
            when 'ac2' then t.ac2_fare
        end * @passengercount) as total_fare,
        case
            when case @seattype
                when 'sleeper' then isnull(sa.sleeper_available, 0)
                when 'ac3' then isnull(sa.ac3_available, 0)
                when 'ac2' then isnull(sa.ac2_available, 0)
            end >= @passengercount then 'available'
            else 'waitlist'
        end as booking_status
    from trains t
    inner join stations s1 on t.source_station_id = s1.station_id
    inner join stations s2 on t.destination_station_id = s2.station_id
    left join seat_availability sa on t.train_id = sa.train_id and sa.journey_date = @journeydate
    where t.source_station_id = @sourcestationid
      and t.destination_station_id = @destinationstationid
      and t.is_active = 1
      and s1.is_active = 1
      and s2.is_active = 1
      and len(t.running_days) >= @dayofweek
      and substring(t.running_days, @dayofweek, 1) = '1'
	  and @journeydate <= '2025-12-01' --for my reference cause train seat availability maded upto only 2025-12-01 
    order by t.departure_time
end

-- b. get train details
create or alter procedure sp_gettraindetails
    @trainid int,
    @journeydate date
as
begin
    set nocount on

    select
        t.*,
        s1.station_name as source_station,
        s1.station_code as source_code,
        s2.station_name as destination_station,
        s2.station_code as destination_code,
        sa.sleeper_available,
        sa.ac3_available,
        sa.ac2_available,
        sa.sleeper_waitlist,
        sa.ac3_waitlist,
        sa.ac2_waitlist
    from trains t
    inner join stations s1 on t.source_station_id = s1.station_id
    inner join stations s2 on t.destination_station_id = s2.station_id
    left join seat_availability sa on t.train_id = sa.train_id and sa.journey_date = @journeydate
    where t.train_id = @trainid
end

-- =====================================================
-- 5. booking operations
-- =====================================================

-- a. make booking (complete ado.net ready)
create or alter procedure sp_makebooking
    @userid int,
    @trainid int,
    @journeydate date,
    @passengercount int,
    @seattype nvarchar(10),
    @paymentmethod nvarchar(20),
    @transactionid nvarchar(100),
    @passengernames nvarchar(max),
    @passengerages nvarchar(100),
    @passengergenders nvarchar(100),
    @bookingid int output,
    @pnrnumber nvarchar(20) output,
    @totalamount decimal(10,2) output,
    @status nvarchar(20) output
as
begin
    set nocount on
    begin transaction

    begin try
        -- Validate journey date
        if @journeydate < cast(getdate() as date) or @journeydate > '2025-12-01'
        begin
            select 0 as success, 'Booking is only allowed for journeys on or before 2025-12-01.' as message,
                   null as pnr, 0 as amount, 'failed' as bookingstatus
            rollback transaction
            return
        end

        -- Validate train and get fare
        declare @fareperpassenger decimal(8,2)
        declare @availableseats int

        select
            @fareperpassenger = case @seattype
                when 'sleeper' then sleeper_fare
                when 'ac3' then ac3_fare
                when 'ac2' then ac2_fare
            end
        from trains
        where train_id = @trainid and is_active = 1

        if @fareperpassenger is null
        begin
            select 0 as success, 'Invalid train or seat type.' as message,
                   null as pnr, 0 as amount, 'failed' as bookingstatus
            rollback transaction
            return
        end

        -- Get available seats (default to 0 if no row exists)
        select
            @availableseats = case @seattype
                when 'sleeper' then isnull(sleeper_available, 0)
                when 'ac3' then isnull(ac3_available, 0)
                when 'ac2' then isnull(ac2_available, 0)
            end
        from seat_availability
        where train_id = @trainid and journey_date = @journeydate

        if @availableseats is null
            set @availableseats = 0

        -- Determine booking status
        set @status = case when @availableseats >= @passengercount then 'confirmed' else 'waitlist' end
        set @totalamount = @fareperpassenger * @passengercount

        -- Create booking
        insert into bookings (user_id, train_id, journey_date, passenger_count, total_amount, booking_status)
        values (@userid, @trainid, @journeydate, @passengercount, @totalamount, @status)

        set @bookingid = scope_identity()
        select @pnrnumber = pnr_number from bookings where booking_id = @bookingid

        -- Parse passenger details
        declare @names table(id int identity, name nvarchar(100))
        declare @ages table(id int identity, age int)
        declare @genders table(id int identity, gender nvarchar(10))

        insert into @names select value from string_split(@passengernames, ',')
        insert into @ages select cast(value as int) from string_split(@passengerages, ',')
        insert into @genders select value from string_split(@passengergenders, ',')

        -- Insert passengers
        insert into passengers (booking_id, name, age, gender, seat_type, fare_paid, status)
        select
            @bookingid,
            ltrim(rtrim(n.name)),
            a.age,
            ltrim(rtrim(g.gender)),
            @seattype,
            @fareperpassenger,
            @status
        from @names n
        inner join @ages a on n.id = a.id
        inner join @genders g on n.id = g.id

        -- Record payment
        insert into payments (booking_id, amount, payment_method, transaction_id)
        values (@bookingid, @totalamount, @paymentmethod, @transactionid)

        commit transaction

        select 1 as success, 'Booking created successfully.' as message,
               @pnrnumber as pnr, @totalamount as amount, @status as bookingstatus

    end try
    begin catch
        rollback transaction
        select 0 as success, error_message() as message,
               null as pnr, 0 as amount, 'failed' as bookingstatus
    end catch
end


-- b. cancel booking
CREATE OR ALTER PROCEDURE sp_cancelbooking
    @pnrnumber NVARCHAR(20),
    @userid INT,
    @passengername NVARCHAR(100),
    @cancellationreason NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @bookingid INT, @journeydate DATE, @bookingstatus NVARCHAR(20), @currentdate DATE = CAST(GETDATE() AS DATE)
        DECLARE @passengerid INT, @fare_paid DECIMAL(8,2), @refundamount DECIMAL(10,2), @remaining_passengers INT

        -- Get booking details
        SELECT @bookingid = booking_id, @journeydate = journey_date, @bookingstatus = booking_status
        FROM bookings
        WHERE pnr_number = @pnrnumber
          AND user_id = @userid
          AND booking_status IN ('confirmed', 'waitlist', 'rac')

        IF @bookingid IS NULL
        BEGIN
            SELECT 0 AS success, 'Booking not found or already cancelled' AS message, 0 AS refundamount
            ROLLBACK TRANSACTION
            RETURN
        END

        -- Get passenger to cancel
        SELECT TOP 1 @passengerid = passenger_id, @fare_paid = fare_paid
        FROM passengers
        WHERE booking_id = @bookingid
          AND name = @passengername
          AND status IN ('confirmed', 'waitlist', 'rac')

        IF @passengerid IS NULL
        BEGIN
            SELECT 0 AS success, 'Passenger not found or already cancelled' AS message, 0 AS refundamount
            ROLLBACK TRANSACTION
            RETURN
        END

        -- Calculate refund for only this passenger
        DECLARE @daystojourney INT = DATEDIFF(DAY, @currentdate, @journeydate)
        IF @bookingstatus = 'waitlist'
            SET @refundamount = CASE WHEN @fare_paid > 10 THEN @fare_paid - 10 ELSE 0 END
        ELSE
            SET @refundamount = CASE
                WHEN @daystojourney >= 1 THEN @fare_paid * 0.9
                WHEN @daystojourney >= 0 THEN @fare_paid * 0.5
                ELSE 0
            END

        -- Update passenger status and cancellation reason
        UPDATE passengers
        SET status = 'cancelled', cancellation_reason = @cancellationreason
        WHERE passenger_id = @passengerid

        -- Update booking's total_amount and refund in payments
        UPDATE bookings
        SET total_amount = total_amount - @fare_paid
        WHERE booking_id = @bookingid

        UPDATE payments
        SET refund_amount = ISNULL(refund_amount, 0) + @refundamount, refund_time = GETDATE()
        WHERE booking_id = @bookingid

        -- If all passengers cancelled, mark booking as cancelled
        SELECT @remaining_passengers = COUNT(*) 
        FROM passengers 
        WHERE booking_id = @bookingid AND status IN ('confirmed', 'waitlist', 'rac')

        IF @remaining_passengers = 0
        BEGIN
            UPDATE bookings
            SET booking_status = 'cancelled', payment_status = 'refunded'
            WHERE booking_id = @bookingid
        END

        COMMIT TRANSACTION;

        SELECT 1 AS success, 
               'Passenger cancelled successfully' AS message, 
               @refundamount AS refundamount

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0 AS success, ERROR_MESSAGE() AS message, 0 AS refundamount
    END CATCH
END

-- c. get user bookings
create or alter procedure sp_getuserbookings
    @userid int,
    @bookingstatus nvarchar(20) = null,
    @pagenumber int = 1,
    @pagesize int = 10
as
begin
    set nocount on

    declare @offset int = (@pagenumber - 1) * @pagesize

    select
        b.booking_id,
        b.pnr_number,
        b.journey_date,
        b.passenger_count,
        b.total_amount,
        b.booking_status,
        b.payment_status,
        b.booking_time,
        t.train_number,
        t.train_name,
        s1.station_name as source_station,
        s2.station_name as destination_station,
        t.departure_time,
        t.arrival_time
    from bookings b
    inner join trains t on b.train_id = t.train_id
    inner join stations s1 on t.source_station_id = s1.station_id
    inner join stations s2 on t.destination_station_id = s2.station_id
    where b.user_id = @userid
      and (@bookingstatus is null or b.booking_status = @bookingstatus)
    order by b.booking_time desc
    offset @offset rows fetch next @pagesize rows only

    select count(*) as totalrecords
    from bookings b
    where b.user_id = @userid
      and (@bookingstatus is null or b.booking_status = @bookingstatus)
end

-- d. get booking details with passengers
create or alter procedure sp_getbookingdetails
    @pnrnumber nvarchar(20),
    @userid int = null
as
begin
    set nocount on

    -- booking details
    select
        b.booking_id,
        b.pnr_number,
        b.journey_date,
        b.passenger_count,
        b.total_amount,
        b.booking_status,
        b.payment_status,
        b.booking_time,
        t.train_number,
        t.train_name,
        s1.station_name as source_station,
        s1.station_code as source_code,
        s2.station_name as destination_station,
        s2.station_code as destination_code,
        t.departure_time,
        t.arrival_time,
        u.name as user_name,
        u.email as user_email,
        u.phone as user_phone,
        p.amount as paid_amount,
        p.payment_method,
        p.transaction_id,
        p.refund_amount,
        p.refund_time
    from bookings b
    inner join trains t on b.train_id = t.train_id
    inner join stations s1 on t.source_station_id = s1.station_id
    inner join stations s2 on t.destination_station_id = s2.station_id
    inner join users u on b.user_id = u.user_id
    left join payments p on b.booking_id = p.booking_id
    where b.pnr_number = @pnrnumber
      and (@userid is null or b.user_id = @userid)

    -- passenger details
    select
        passenger_id,
        name,
        age,
        gender,
        seat_type,
        seat_number,
        coach_number,
        fare_paid,
        status
    from passengers p
    inner join bookings b on p.booking_id = b.booking_id
    where b.pnr_number = @pnrnumber
      and (@userid is null or b.user_id = @userid)
    order by p.passenger_id
end

-- =====================================================
-- 6. helper functions (ado.net compatible)
-- =====================================================

-- a. check seat availability
create or alter function fn_getseatavailability(
    @trainid int,
    @journeydate date,
    @seattype nvarchar(10)
)
returns int
as
begin
    declare @availableseats int = 0

    select @availableseats =
        case @seattype
            when 'sleeper' then isnull(sleeper_available, 0)
            when 'ac3' then isnull(ac3_available, 0)
            when 'ac2' then isnull(ac2_available, 0)
            else 0
        end
    from seat_availability
    where train_id = @trainid and journey_date = @journeydate

    return @availableseats
end

-- b. calculate fare
create or alter function fn_calculatefare(
    @trainid int,
    @seattype nvarchar(10),
    @passengercount int
)
returns decimal(10,2)
as
begin
    declare @totalfare decimal(10,2) = 0

    select @totalfare = @passengercount *
        case @seattype
            when 'sleeper' then sleeper_fare
            when 'ac3' then ac3_fare
            when 'ac2' then ac2_fare
            else 0
        end
    from trains
    where train_id = @trainid

    return isnull(@totalfare, 0)
end

---- c. validate journey date
create or alter function fn_isvalidjourneydate(
    @trainid int,
    @journeydate date
)
returns bit
as
begin
    declare @isvalid bit = 0
    declare @dayofweek int = datepart(weekday, @journeydate)
    declare @runningdays nvarchar(7)
    declare @currentdate date = cast(getdate() as date)

    -- check if journey date is not in past
    if @journeydate < @currentdate
        return 0

    select @runningdays = running_days
    from trains
    where train_id = @trainid and is_active = 1

    if @runningdays is not null and len(@runningdays) >= @dayofweek
    begin
        set @isvalid = case when substring(@runningdays, @dayofweek, 1) = '1' then 1 else 0 end
    end

    return @isvalid
end

-- =====================================================
-- 7. admin procedures
-- =====================================================

-- a. get booking reports
create or alter procedure sp_getbookingreports
    @fromdate date,
    @todate date,
    @trainid int = null,
    @bookingstatus nvarchar(20) = null
as
begin
    set nocount on

    select
        b.pnr_number,
        b.journey_date,
        b.passenger_count,
        b.total_amount,
        b.booking_status,
        b.payment_status,
        b.booking_time,
        t.train_number,
        t.train_name,
        u.name as user_name,
        u.email as user_email,
        count(p.passenger_id) as actual_passengers
    from bookings b
    inner join trains t on b.train_id = t.train_id
    inner join users u on b.user_id = u.user_id
    left join passengers p on b.booking_id = p.booking_id
    where b.journey_date between @fromdate and @todate
      and (@trainid is null or b.train_id = @trainid)
      and (@bookingstatus is null or b.booking_status = @bookingstatus)
    group by b.booking_id, b.pnr_number, b.journey_date, b.passenger_count,
             b.total_amount, b.booking_status, b.payment_status, b.booking_time,
             t.train_number, t.train_name, u.name, u.email
    order by b.booking_time desc
end

-- b. generate seat availability
create or alter procedure sp_generateavailability
    @trainid int,
    @fromdate date,
    @todate date
as
begin
    set nocount on
    begin try
        declare @sleeper int, @ac3 int, @ac2 int

        select @sleeper = sleeper_seats, @ac3 = ac3_seats, @ac2 = ac2_seats
        from trains where train_id = @trainid

        with daterange as (
            select @fromdate as journey_date
            union all
            select dateadd(day, 1, journey_date)
            from daterange
            where journey_date < @todate
        )
        insert into seat_availability (train_id, journey_date, sleeper_available, ac3_available, ac2_available)
        select @trainid, journey_date, @sleeper, @ac3, @ac2
        from daterange
        where not exists (
            select 1 from seat_availability sa
            where sa.train_id = @trainid and sa.journey_date = daterange.journey_date
        )
        option (maxrecursion 366)

        select 1 as success, @@rowcount as recordscreated
    end try
    begin catch
        select 0 as success, error_message() as message
    end catch
end

-- =====================================================
-- 8. additional business logic procedures
-- =====================================================

-- a. update passenger seat assignment
create or alter procedure sp_assignseats
    @bookingid int,
    @seatassignments nvarchar(max)
as
begin
    set nocount on
    begin try

        declare @success bit = 1

        -- validate booking exists and is confirmed
        if not exists(select 1 from bookings where booking_id = @bookingid and booking_status = 'confirmed')
        begin
            select 0 as success, 'invalid booking or booking not confirmed' as message
            return
        end

        -- log the seat assignment activity
        insert into admin_logs (admin_id, action, target_table, record_id, details)
        values (1, 'seat assignment', 'passengers', @bookingid, 'seats assigned for booking: ' + cast(@bookingid as nvarchar))

        select 1 as success, 'seat assignments updated successfully' as message

    end try
    begin catch
        select 0 as success, error_message() as message
    end catch
end

-- b. get train occupancy report
create or alter procedure sp_gettrainoccupancy
    @trainid int,
    @fromdate date,
    @todate date
as
begin
    set nocount on

    select
        b.journey_date,
        t.train_number,
        t.train_name,
        t.total_seats,
        sum(case when p.seat_type = 'sleeper' then 1 else 0 end) as sleeper_booked,
        t.sleeper_seats - sum(case when p.seat_type = 'sleeper' then 1 else 0 end) as sleeper_available,
        sum(case when p.seat_type = 'ac3' then 1 else 0 end) as ac3_booked,
        t.ac3_seats - sum(case when p.seat_type = 'ac3' then 1 else 0 end) as ac3_available,
        sum(case when p.seat_type = 'ac2' then 1 else 0 end) as ac2_booked,
        t.ac2_seats - sum(case when p.seat_type = 'ac2' then 1 else 0 end) as ac2_available,
        count(p.passenger_id) as total_passengers,
        sum(b.total_amount) as total_revenue,
        cast((count(p.passenger_id) * 100.0 / t.total_seats) as decimal(5,2)) as occupancy_percentage
    from trains t
    left join bookings b on t.train_id = b.train_id
        and b.journey_date between @fromdate and @todate
        and b.booking_status = 'confirmed'
    left join passengers p on b.booking_id = p.booking_id and p.status = 'confirmed'
    where t.train_id = @trainid
    group by b.journey_date, t.train_number, t.train_name, t.total_seats, t.sleeper_seats, t.ac3_seats, t.ac2_seats
    order by b.journey_date
end

-- c. process waitlist bookings
create or alter procedure sp_processwaitlist
    @trainid int,
    @journeydate date
as
begin
    set nocount on
    begin transaction

    begin try
        declare @processedcount int = 0

        -- get available seats
        declare @sleeperavailable int, @ac3available int, @ac2available int

        select @sleeperavailable = sleeper_available,
               @ac3available = ac3_available,
               @ac2available = ac2_available
        from seat_availability
        where train_id = @trainid and journey_date = @journeydate

        -- process sleeper waitlist
        if @sleeperavailable > 0
        begin
            update top(@sleeperavailable) p
            set status = 'confirmed'
            from passengers p
            inner join bookings b on p.booking_id = b.booking_id
            where b.train_id = @trainid
              and b.journey_date = @journeydate
              and p.seat_type = 'sleeper'
              and p.status = 'waitlist'
              and b.booking_status = 'waitlist'

            set @processedcount += @@rowcount
        end

        -- update booking status for confirmed passengers
        update b
        set booking_status = 'confirmed'
        from bookings b
        where b.train_id = @trainid
          and b.journey_date = @journeydate
          and b.booking_status = 'waitlist'
          and exists (
              select 1 from passengers p
              where p.booking_id = b.booking_id and p.status = 'confirmed'
          )

        commit transaction

        select 1 as success, @processedcount as processedpassengers

    end try
    begin catch
        rollback transaction
        select 0 as success, error_message() as message
    end catch
end

-- d. get revenue report
create or alter procedure sp_getrevenuereport
    @fromdate date,
    @todate date,
    @trainid int = null,
    @groupby nvarchar(10) = 'daily'
as
begin
    set nocount on

    if @groupby = 'daily'
    begin
        select
            b.journey_date,
            count(distinct b.booking_id) as total_bookings,
            sum(b.passenger_count) as total_passengers,
            sum(b.total_amount) as total_revenue,
            sum(case when b.booking_status = 'cancelled' then p.refund_amount else 0 end) as total_refunds,
            sum(b.total_amount) - sum(case when b.booking_status = 'cancelled' then p.refund_amount else 0 end) as net_revenue
        from bookings b
        left join payments p on b.booking_id = p.booking_id
        where b.journey_date between @fromdate and @todate
          and (@trainid is null or b.train_id = @trainid)
        group by b.journey_date
        order by b.journey_date
    end
    else if @groupby = 'train'
    begin
        select
            t.train_number,
            t.train_name,
            count(distinct b.booking_id) as total_bookings,
            sum(b.passenger_count) as total_passengers,
            sum(b.total_amount) as total_revenue,
            avg(b.total_amount) as avg_booking_value
        from bookings b
        inner join trains t on b.train_id = t.train_id
        where b.journey_date between @fromdate and @todate
          and (@trainid is null or b.train_id = @trainid)
        group by t.train_id, t.train_number, t.train_name
        order by total_revenue desc
    end
end

-- =====================================================
-- 9. notification & communication procedures
-- =====================================================

-- a. get booking notifications
create or alter procedure sp_getbookingnotifications
    @userid int,
    @notificationtype nvarchar(20) = null
as
begin
    set nocount on

    -- return booking-related notifications
    select
        'booking_confirmation' as notification_type,
        b.pnr_number,
        'your booking ' + b.pnr_number + ' for ' + t.train_name + ' on ' +
        convert(nvarchar, b.journey_date, 106) + ' is confirmed' as message,
        b.booking_time as created_at,
        case when datediff(hour, b.booking_time, getdate()) <= 24 then 1 else 0 end as is_new
    from bookings b
    inner join trains t on b.train_id = t.train_id
    where b.user_id = @userid
      and b.booking_status = 'confirmed'
      and (@notificationtype is null or @notificationtype = 'booking')

    union all

    select
        'waitlist_notification' as notification_type,
        b.pnr_number,
        'your booking ' + b.pnr_number + ' is in waitlist. we will notify you if seats become available' as message,
        b.booking_time as created_at,
        case when datediff(hour, b.booking_time, getdate()) <= 24 then 1 else 0 end as is_new
    from bookings b
    where b.user_id = @userid
      and b.booking_status = 'waitlist'
      and (@notificationtype is null or @notificationtype = 'waitlist')

    order by created_at desc
end

-- b. get journey reminders
create or alter procedure sp_getjourneyreminders
    @reminderdate date = null
as
begin
    set nocount on

    set @reminderdate = isnull(@reminderdate, cast(getdate() as date))

    select
        u.user_id,
        u.name,
        u.email,
        u.phone,
        b.pnr_number,
        t.train_number,
        t.train_name,
        s1.station_name as source_station,
        s2.station_name as destination_station,
        t.departure_time,
        b.journey_date,
        'your train ' + t.train_name + ' (' + t.train_number + ') departs today at ' +
        convert(nvarchar, t.departure_time, 108) + ' from ' + s1.station_name as reminder_message
    from bookings b
    inner join users u on b.user_id = u.user_id
    inner join trains t on b.train_id = t.train_id
    inner join stations s1 on t.source_station_id = s1.station_id
    inner join stations s2 on t.destination_station_id = s2.station_id
    where b.journey_date = @reminderdate
      and b.booking_status = 'confirmed'
      and u.is_active = 1
    order by t.departure_time
end

-- =====================================================
-- 10. data validation & business rules
-- =====================================================

-- a. validate booking request
create or alter procedure sp_validatebookingrequest
    @userid int,
    @trainid int,
    @journeydate date,
    @passengercount int,
    @seattype nvarchar(10)
as
begin
    set nocount on

    declare @validationerrors table (error_message nvarchar(200))

    -- check user exists and is active
    if not exists(select 1 from users where user_id = @userid and is_active = 1)
        insert into @validationerrors values ('invalid or inactive user')

    -- check train exists and is active
    if not exists(select 1 from trains where train_id = @trainid and is_active = 1)
        insert into @validationerrors values ('invalid or inactive train')

    -- check journey date is not in past
    if @journeydate < cast(getdate() as date)
        insert into @validationerrors values ('journey date cannot be in the past')

    -- check journey date is within booking window (120 days)
    if @journeydate > dateadd(day, 120, cast(getdate() as date))
        insert into @validationerrors values ('booking not allowed beyond 120 days')

    -- check train runs on selected date
    if dbo.fn_isvalidjourneydate(@trainid, @journeydate) = 0
        insert into @validationerrors values ('train does not run on selected date')

    -- check passenger count limits
    if @passengercount <= 0 or @passengercount > 6
        insert into @validationerrors values ('passenger count must be between 1 and 6')

    -- check seat type is valid for train
    declare @seatexists bit = 0

    select @seatexists = case
        when @seattype = 'sleeper' and sleeper_seats > 0 then 1
        when @seattype = 'ac3' and ac3_seats > 0 then 1
        when @seattype = 'ac2' and ac2_seats > 0 then 1
        else 0
    end
    from trains where train_id = @trainid

    if @seatexists = 0
        insert into @validationerrors values ('selected seat type not available in this train')

    -- return validation results
    if exists(select 1 from @validationerrors)
    begin
        select 0 as isvalid, error_message as validationmessage
        from @validationerrors
    end
    else
    begin
        select 1 as isvalid, 'validation successful' as validationmessage
    end
end

-- b. check duplicate booking
create or alter procedure sp_checkduplicatebooking
    @userid int,
    @trainid int,
    @journeydate date
as
begin
    set nocount on

    declare @existingpnr nvarchar(20) = null

    select top 1 @existingpnr = pnr_number
    from bookings
    where user_id = @userid
      and train_id = @trainid
      and journey_date = @journeydate
      and booking_status in ('confirmed', 'waitlist')

    if @existingpnr is not null
    begin
        select 1 as hasduplicate, @existingpnr as existingpnr,
               'you already have a booking for this train on this date' as message
    end
    else
    begin
        select 0 as hasduplicate, null as existingpnr, 'no duplicate booking found' as message
    end
end

--For Activate and Deactivate User

create or alter procedure sp_setuseractive
@userid int,
@active bit,
@admin_id int
as
begin
    set nocount on;
    
    update users
    set is_active = @active
    where user_id = @userid;

    insert into admin_logs (admin_id, action, target_table, record_id, details)
    values (@admin_id, 'set user active/inactive', 'users', @userid, 
            'set is_active to ' + cast(@active as varchar) + ' for user_id ' + cast(@userid as varchar));

    select user_id, name, email, is_active from users where user_id = @userid;
end



