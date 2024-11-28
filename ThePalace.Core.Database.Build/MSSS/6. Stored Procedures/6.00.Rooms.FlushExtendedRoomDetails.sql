IF EXISTS (SELECT TOP 1 1 FROM SYS.OBJECTS WHERE TYPE = N'P' AND OBJECT_ID = OBJECT_ID(N'[Rooms].[FlushExtendedRoomDetails]')) BEGIN
	DROP PROCEDURE [Rooms].[FlushExtendedRoomDetails]
END
GO

CREATE PROCEDURE [Rooms].[FlushExtendedRoomDetails]
	@RoomID SMALLINT
	, @AuthorChanges BIT = 0
AS
-- =============================================
-- Author:		Paul Taylor
-- Create date: 9/29/2018
-- Description:	???
-- =============================================
BEGIN
	IF @AuthorChanges = 1 BEGIN
		DELETE FROM Hotspots.Hotspots WHERE RoomID = @RoomID
		DELETE FROM Hotspots.Pictures WHERE RoomID = @RoomID
		DELETE FROM Hotspots.States WHERE RoomID = @RoomID
		DELETE FROM Hotspots.Vortexes WHERE RoomID = @RoomID
	END

	DELETE FROM Rooms.LooseProps WHERE RoomID = @RoomID
	DELETE FROM Rooms.DrawCmds WHERE RoomID = @RoomID
END
