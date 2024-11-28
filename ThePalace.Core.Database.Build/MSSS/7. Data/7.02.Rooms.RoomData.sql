IF NOT EXISTS(SELECT TOP 1 1 FROM [Rooms].[RoomData] WHERE [RoomID] = 42) BEGIN
	INSERT [Rooms].[RoomData] ([RoomID], [FacesID], [PictureName], [ArtistName], [Password]) VALUES (42, 0, N'endoftime.jpg', N'Fate', NULL)
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Rooms].[RoomData] WHERE [RoomID] = 69) BEGIN
	INSERT [Rooms].[RoomData] ([RoomID], [FacesID], [PictureName], [ArtistName], [Password]) VALUES (69, 0, N'clouds.jpg', N'Sinistral Janus', N'broken')
END
GO
