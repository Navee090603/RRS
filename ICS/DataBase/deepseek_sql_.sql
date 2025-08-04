CREATE OR ALTER PROCEDURE sp_setuseractive
@userid INT,
@active BIT,
@admin_id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE users
    SET is_active = @active
    WHERE user_id = @userid;

    -- Optionally log to admin_logs table if needed
    INSERT INTO admin_logs (admin_id, action, target_table, record_id, details)
    VALUES (@admin_id, 'Set User Active/InActive', 'users', @userid, 
            'Set is_active to ' + CAST(@active AS VARCHAR) + ' for user_id ' + CAST(@userid AS VARCHAR));

    SELECT user_id, name, email, is_active FROM users WHERE user_id = @userid;
END
