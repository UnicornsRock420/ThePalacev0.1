IF EXISTS (SELECT TOP 1 1 FROM SYS.OBJECTS WHERE TYPE = N'P' AND OBJECT_ID = OBJECT_ID(N'[Users].[FlushUserAssets]')) BEGIN
	DROP PROCEDURE [Users].[FlushUserAssets]
END
GO

CREATE PROCEDURE [Users].[FlushUserAssets]
	@UserID INT
AS
-- =============================================
-- Author:		Paul Taylor
-- Create date: 9/29/2018
-- Description:	???
-- =============================================
BEGIN
	DELETE FROM Users.Assets WHERE UserID = @UserID
END
