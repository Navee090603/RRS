--Procedures

--1.sp_cancelbooking
create or alter procedure sp_cancelbooking
    @pnrnumber nvarchar(20),
    @userid int,
    @passengername nvarchar(100),
    @cancellationreason nvarchar(200) = null
as
begin
    set nocount on;
    begin transaction;
    begin try
        declare @bookingid int, @journeydate date, @bookingstatus nvarchar(20), @currentdate date = cast(getdate() as date);
        declare @passengerid int, @fare_paid decimal(8,2), @refundamount decimal(10,2), @remaining_passengers int;

        select @bookingid = booking_id, @journeydate = journey_date, @bookingstatus = booking_status
        from bookings
        where pnr_number = @pnrnumber
          and user_id = @userid
          and booking_status in ('confirmed', 'waitlist', 'rac');

        if @bookingid is null
        begin
            select 0 as success, 'booking not found or already cancelled' as message, 0 as refundamount;
            rollback transaction;
            return;
        end;

        select top 1 @passengerid = passenger_id, @fare_paid = fare_paid
        from passengers
        where booking_id = @bookingid
          and name = @passengername
          and status in ('confirmed', 'waitlist', 'rac');

        if @passengerid is null
        begin
            select 0 as success, 'passenger not found or already cancelled' as message, 0 as refundamount;
            rollback transaction;
            return;
        end;

        declare @daystojourney int = datediff(day, @currentdate, @journeydate);
        if @bookingstatus = 'waitlist'
            set @refundamount = case when @fare_paid > 10 then @fare_paid - 10 else 0 end;
        else
            set @refundamount = case
                when @daystojourney >= 1 then @fare_paid * 0.9
                when @daystojourney >= 0 then @fare_paid * 0.5
                else 0
            end;

        update passengers
        set status = 'cancelled', cancellation_reason = @cancellationreason
        where passenger_id = @passengerid;

        update bookings
        set refund_amount = isnull(refund_amount, 0) + @refundamount
        where booking_id = @bookingid;

        update payments
        set refund_amount = isnull(refund_amount, 0) + @refundamount, refund_time = getdate()
        where booking_id = @bookingid;

        select @remaining_passengers = count(*) 
        from passengers 
        where booking_id = @bookingid and status in ('confirmed', 'waitlist', 'rac');

        if @remaining_passengers = 0
        begin
            update bookings
            set booking_status = 'cancelled', payment_status = 'refunded'
            where booking_id = @bookingid;
        end;

        commit transaction;

        select 1 as success, 
               'passenger cancelled successfully' as message, 
               @refundamount as refundamount;

    end try
    begin catch
        rollback transaction;
        select 0 as success, error_message() as message, 0 as refundamount;
    end catch;
end;
go

---------------------------------------------------
--2. sp_getallstations

create or alter procedure sp_getallstations
as
begin
    set nocount on;

    select station_id, station_name, station_code, state
    from stations
    where is_active = 1
    order by station_id;
end;
go

---------------------------------------------------
--3.sp_getrevenuereport


create or alter procedure sp_getrevenuereport
    @fromdate date,
    @todate date,
    @trainid int = null,
    @groupby nvarchar(10) = 'daily'
as
begin
    set nocount on;

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
        order by b.journey_date;
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
        order by total_revenue desc;
    end;
end;
go

---------------------------------------------------
--4.sp_getuserbookings

create or alter procedure sp_getuserbookings
    @userid int,
    @bookingstatus nvarchar(20) = null,
    @pagenumber int = 1,
    @pagesize int = 10
as
begin
    set nocount on;

    declare @offset int = (@pagenumber - 1) * @pagesize;

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
    offset @offset rows fetch next @pagesize rows only;

    select count(*) as totalrecords
    from bookings b
    where b.user_id = @userid
      and (@bookingstatus is null or b.booking_status = @bookingstatus);
end;
go

---------------------------------------------------
--5.sp_loginuser

create or alter procedure sp_loginuser
    @email nvarchar(255),
    @password nvarchar(255)
as
begin
    set nocount on;

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
    where email = @email;
end;
go

---------------------------------------------------
--6.sp_makebooking

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
    set nocount on;
    begin transaction;

    begin try
        if @journeydate < cast(getdate() as date) or @journeydate > '2025-12-01'
        begin
            select 0 as success, 'booking is only allowed for journeys on or before 2025-12-01.' as message,
                   null as pnr, 0 as amount, 'failed' as bookingstatus;
            rollback transaction;
            return;
        end;

        declare @fareperpassenger decimal(8,2);
        declare @availableseats int;

        select
            @fareperpassenger = case @seattype
                when 'sleeper' then sleeper_fare
                when 'ac3' then ac3_fare
                when 'ac2' then ac2_fare
            end
        from trains
        where train_id = @trainid and is_active = 1;

        if @fareperpassenger is null
        begin
            select 0 as success, 'invalid train or seat type.' as message,
                   null as pnr, 0 as amount, 'failed' as bookingstatus;
            rollback transaction;
            return;
        end;

        select
            @availableseats = case @seattype
                when 'sleeper' then isnull(sleeper_available, 0)
                when 'ac3' then isnull(ac3_available, 0)
                when 'ac2' then isnull(ac2_available, 0)
            end
        from seat_availability
        where train_id = @trainid and journey_date = @journeydate;

        if @availableseats is null
            set @availableseats = 0;

        set @status = case when @availableseats >= @passengercount then 'confirmed' else 'waitlist' end;
        set @totalamount = @fareperpassenger * @passengercount;

        insert into bookings (user_id, train_id, journey_date, passenger_count, total_amount, booking_status)
        values (@userid, @trainid, @journeydate, @passengercount, @totalamount, @status);

        set @bookingid = scope_identity();
        select @pnrnumber = pnr_number from bookings where booking_id = @bookingid;

        declare @names table(id int identity, name nvarchar(100));
        declare @ages table(id int identity, age int);
        declare @genders table(id int identity, gender nvarchar(10));

        insert into @names select value from string_split(@passengernames, ',');
        insert into @ages select cast(value as int) from string_split(@passengerages, ',');
        insert into @genders select value from string_split(@passengergenders, ',');

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
        inner join @genders g on n.id = g.id;

        insert into payments (booking_id, amount, payment_method, transaction_id)
        values (@bookingid, @totalamount, @paymentmethod, @transactionid);

        commit transaction;

        select 1 as success, 'booking created successfully.' as message,
               @pnrnumber as pnr, @totalamount as amount, @status as bookingstatus;

    end try
    begin catch
        rollback transaction;
        select 0 as success, error_message() as message,
               null as pnr, 0 as amount, 'failed' as bookingstatus;
    end catch;
end;
go

---------------------------------------------------
--7.sp_registeruser

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
    set nocount on;
    begin try
        if exists(select 1 from users where email = @email)
        begin
            select 0 as success, 'email already registered' as message, 0 as userid;
            return;
        end;

        if exists(select 1 from users where phone = @phone)
        begin
            select 0 as success, 'phone number already registered' as message, 0 as userid;
            return;
        end;

        declare @userid int;

        insert into users (name, email, phone, password_hash, date_of_birth, gender, user_type)
        values (@name, @email, @phone, @password, @dateofbirth, @gender, @usertype);

        set @userid = scope_identity();

        select 1 as success, 'user registered successfully' as message, @userid as userid;

    end try
    begin catch
        select 0 as success, error_message() as message, 0 as userid;
    end catch;
end;
go

---------------------------------------------------
--8.sp_searchtrains

create or alter procedure sp_searchtrains
    @sourcestationid int,
    @destinationstationid int,
    @journeydate date,
    @seattype nvarchar(10) = 'sleeper',
    @passengercount int = 1
as
begin
    set nocount on;

    declare @dayofweek int = datepart(weekday, @journeydate);

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
        (
            case @seattype
                when 'sleeper' then t.sleeper_fare
                when 'ac3' then t.ac3_fare
                when 'ac2' then t.ac2_fare
            end * @passengercount
        ) as total_fare,
        case
            when
                case @seattype
                    when 'sleeper' then isnull(sa.sleeper_available, 0)
                    when 'ac3' then isnull(sa.ac3_available, 0)
                    when 'ac2' then isnull(sa.ac2_available, 0)
                end >= @passengercount
            then 'available'
            else 'Not_Running'
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
      and @journeydate <= '2025-12-05'
    order by t.departure_time;
end;
go




---------------------------------------------------
--9.sp_setuseractive

create or alter procedure sp_setuseractive 
    @userid int, 
    @active bit, 
    @admin_id int 
as 
begin 
    set nocount on;

    -- prevent admin from deactivating themselves
    if @userid = @admin_id and @active = 0
    begin
        throw 50000, 'error: admins cannot deactivate themselves.', 1;
    end

    update users 
    set is_active = @active 
    where user_id = @userid;

    insert into admin_logs 
        (admin_id, action, target_table, record_id, details) 
    values 
        (@admin_id, 'set user active/inactive', 'users', @userid, 
         'set is_active to ' + cast(@active as varchar) + ' for user_id ' + cast(@userid as varchar));

    select user_id, name, email, is_active 
    from users 
    where user_id = @userid;
end;
go

---------------------------------------------------
--Functions

--1. is valid journey date
create or alter function fn_isvalidjourneydate(
    @trainid int,
    @journeydate date
)
returns bit
as
begin
    declare @isvalid bit = 0;
    declare @runningdays nvarchar(7);
    declare @currentdate date = cast(getdate() as date);

    if @journeydate < @currentdate
        return 0;

    select @runningdays = running_days
    from trains
    where train_id = @trainid and is_active = 1;

    declare @dayindex int = ((datepart(weekday, @journeydate) + @@datefirst - 2) % 7) + 1;

    if @runningdays is not null and len(@runningdays) >= @dayindex
    begin
        set @isvalid = case when substring(@runningdays, @dayindex, 1) = '1' then 1 else 0 end;
    end;

    return @isvalid;
end;
go

---------------------------------------------------

--Triggers

-- 1. auto-generate seat availability when train is added
create or alter trigger trg_train_insert_availability
on trains
after insert
as
begin
    set nocount on;

    declare @fromdate date   = cast(getdate() as date);
    declare @todate   date   = dateadd(day, 119, @fromdate);

    ;with daterange as (
        select @fromdate as journey_date
        union all
        select dateadd(day, 1, journey_date)
        from daterange
        where journey_date < @todate
    ),
    new_availability as (
        select i.train_id, d.journey_date,
               i.sleeper_seats, i.ac3_seats, i.ac2_seats
        from inserted i
        cross join daterange d
        where i.is_active = 1
    )
    insert into seat_availability
      (train_id, journey_date, sleeper_available, ac3_available, ac2_available)
    select na.train_id, na.journey_date,
           na.sleeper_seats, na.ac3_seats, na.ac2_seats
    from new_availability na
    where not exists (
        select 1
        from seat_availability sa
        where sa.train_id    = na.train_id
          and sa.journey_date = na.journey_date
    )
    option (maxrecursion 130);

    -- now delete any rows we just inserted that aren’t valid journeys
    delete sa
    from seat_availability sa
    inner join inserted i
        on sa.train_id = i.train_id
    where dbo.fn_isvalidjourneydate(sa.train_id, sa.journey_date) = 0;
end;



-- 2. update seat availability when booking is made

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
    )
end

-- 3. restore seat availability when booking is cancelled

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
        )
    end
end
