-- Add this to sp_setuseractive procedure
CREATE OR ALTER PROCEDURE sp_setuseractive
    @admin_id INT,  -- The admin performing the action
    @userid INT,
    @active BIT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Get user details before update for logging
        DECLARE @username NVARCHAR(100), @oldstatus BIT, @newstatus NVARCHAR(20)
        SELECT @username = name, @oldstatus = is_active FROM users WHERE user_id = @userid;
        
        -- Perform the update
        UPDATE users
        SET is_active = @active
        WHERE user_id = @userid;

        -- Log the action
        SET @newstatus = CASE WHEN @active = 1 THEN 'activated' ELSE 'deactivated' END;
        
        INSERT INTO admin_logs (admin_id, action, target_table, record_id, details)
        VALUES (@admin_id, 'user status change', 'users', @userid, 
                CONCAT('User "', @username, '" ', @newstatus, ' by admin'));

        COMMIT TRANSACTION;
        
        SELECT user_id, name, email, is_active FROM users WHERE user_id = @userid;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Add this to sp_generateavailability procedure
CREATE OR ALTER PROCEDURE sp_generateavailability
    @admin_id INT,  -- The admin performing the action
    @trainid INT,
    @fromdate DATE,
    @todate DATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @sleeper INT, @ac3 INT, @ac2 INT, @trainname NVARCHAR(100), @recordscreated INT = 0;

        SELECT @sleeper = sleeper_seats, @ac3 = ac3_seats, @ac2 = ac2_seats, 
               @trainname = train_name
        FROM trains WHERE train_id = @trainid;

        WITH daterange AS (
            SELECT @fromdate AS journey_date
            UNION ALL
            SELECT DATEADD(day, 1, journey_date)
            FROM daterange
            WHERE journey_date < @todate
        )
        INSERT INTO seat_availability (train_id, journey_date, sleeper_available, ac3_available, ac2_available)
        SELECT @trainid, journey_date, @sleeper, @ac3, @ac2
        FROM daterange
        WHERE NOT EXISTS (
            SELECT 1 FROM seat_availability sa
            WHERE sa.train_id = @trainid AND sa.journey_date = daterange.journey_date
        )
        OPTION (maxrecursion 366);

        SET @recordscreated = @@ROWCOUNT;
        
        -- Log the action
        INSERT INTO admin_logs (admin_id, action, target_table, record_id, details)
        VALUES (@admin_id, 'generate availability', 'seat_availability', @trainid, 
                CONCAT('Generated availability for train ', @trainname, ' (ID:', @trainid, 
                       ') from ', @fromdate, ' to ', @todate, '. Created ', @recordscreated, ' records'));
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS success, @recordscreated AS recordscreated;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 0 AS success, ERROR_MESSAGE() AS message;
    END CATCH
END
GO

-- Create new procedure for adding trains with logging
CREATE OR ALTER PROCEDURE sp_addtrain
    @admin_id INT,
    @train_number NVARCHAR(20),
    @train_name NVARCHAR(100),
    @source_station_id INT,
    @destination_station_id INT,
    @departure_time TIME,
    @arrival_time TIME,
    @running_days NVARCHAR(7),
    @total_seats INT,
    @sleeper_seats INT,
    @ac3_seats INT,
    @ac2_seats INT,
    @sleeper_fare DECIMAL(8,2),
    @ac3_fare DECIMAL(8,2),
    @ac2_fare DECIMAL(8,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        INSERT INTO trains (
            train_number, train_name, source_station_id, destination_station_id,
            departure_time, arrival_time, running_days, total_seats,
            sleeper_seats, ac3_seats, ac2_seats,
            sleeper_fare, ac3_fare, ac2_fare
        )
        VALUES (
            @train_number, @train_name, @source_station_id, @destination_station_id,
            @departure_time, @arrival_time, @running_days, @total_seats,
            @sleeper_seats, @ac3_seats, @ac2_seats,
            @sleeper_fare, @ac3_fare, @ac2_fare
        );
        
        DECLARE @train_id INT = SCOPE_IDENTITY();
        
        -- Log the action
        INSERT INTO admin_logs (admin_id, action, target_table, record_id, details)
        VALUES (@admin_id, 'add train', 'trains', @train_id, 
                CONCAT('Added new train ', @train_name, ' (', @train_number, 
                       ') from station ID:', @source_station_id, ' to ', @destination_station_id));
        
        COMMIT TRANSACTION;
        
        SELECT @train_id AS train_id;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO