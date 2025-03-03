-- User Table
CREATE TABLE LOC_User (
    UserID INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    Username VARCHAR(255) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PhoneNumber VARCHAR(20) NOT NULL,
    Address VARCHAR(255) NOT NULL,
    Role VARCHAR(50) NOT NULL DEFAULT 'User'
);


-- select all
CREATE PROCEDURE [dbo].[PR_User_SelectAll]
AS
BEGIN
	SELECT 
		[dbo].[LOC_User].[UserID], 
		[dbo].[LOC_User].[UserName], 
		[dbo].[LOC_User].[Password], 
		[dbo].[LOC_User].[Email], 
		[dbo].[LOC_User].[PhoneNumber], 
		[dbo].[LOC_User].[Address],   
		[dbo].[LOC_User].[Role]
	FROM [dbo].[LOC_User]
	ORDER BY [dbo].[LOC_User].[UserID], 
			 [dbo].[LOC_User].[UserName], 
		     [dbo].[LOC_User].[Password], 
		     [dbo].[LOC_User].[Email], 
		     [dbo].[LOC_User].[PhoneNumber], 
		     [dbo].[LOC_User].[Address],   
		     [dbo].[LOC_User].[Role];
END


-- sign up
CREATE PROCEDURE [dbo].[PR_User_SignUp]
    @UserName VARCHAR(255),
    @Password VARCHAR(255),
    @Email VARCHAR(255),
    @PhoneNumber VARCHAR(20),
    @Address VARCHAR(255)
AS
BEGIN
    -- Check if username or email already exists
    IF EXISTS (SELECT 1 FROM [dbo].[LOC_User] WHERE [UserName] = @UserName)
    BEGIN
        RAISERROR ('Username already exists.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM [dbo].[LOC_User] WHERE [Email] = @Email)
    BEGIN
        RAISERROR ('Email already exists.', 16, 1);
        RETURN;
    END

    -- Insert user if no duplicate is found
    INSERT INTO [dbo].[LOC_User] ([UserName], [Password], [Email], [PhoneNumber], [Address])
    VALUES (@UserName, @Password, @Email, @PhoneNumber, @Address);
END;


-- login
CREATE PROCEDURE [dbo].[PR_User_Login]
    @UserName VARCHAR(255),
    @Password VARCHAR(255),
    @Role VARCHAR(50)  -- Ensure Role is included
AS
BEGIN
    -- Check if the user exists
    IF NOT EXISTS (SELECT 1 FROM [dbo].[LOC_User] WHERE [UserName] = @UserName)
    BEGIN
        RAISERROR ('User does not exist.', 16, 1);
        RETURN;
    END

    -- Check if the password match
    IF NOT EXISTS (SELECT 1 FROM [dbo].[LOC_User] WHERE [Password] = @Password)
    BEGIN
        RAISERROR ('Incorrect password.', 16, 1);
        RETURN;
    END

    -- Check if the user has the correct role
    IF NOT EXISTS (SELECT 1 FROM [dbo].[LOC_User] WHERE [UserName] = @UserName AND [Password] = @Password AND [Role] = @Role)
    BEGIN
        RAISERROR ('Invalid Username or Password', 16, 1);
        RETURN;
    END

    -- Return user data if all checks pass
    SELECT UserID, UserName, Role FROM [dbo].[LOC_User]
    WHERE UserName = @UserName AND Password = @Password;
END;