IF EXISTS (SELECT TOP 1 1 FROM SYS.OBJECTS WHERE TYPE = N'P' AND OBJECT_ID = OBJECT_ID(N'[Users].[FlushUserDetails]')) BEGIN
	DROP PROCEDURE [Users].[FlushUserDetails]
END
GO

CREATE PROCEDURE [Users].[FlushUserDetails]
	@UserID INT
AS
-- =============================================
-- Author:		Paul Taylor
-- Create date: 9/29/2018
-- Description:	???
-- =============================================
BEGIN
	DELETE FROM Users.Users WHERE UserID = @UserID
	DELETE FROM Users.UserData WHERE UserID = @UserID
	DELETE FROM Users.Assets WHERE UserID = @UserID
END
