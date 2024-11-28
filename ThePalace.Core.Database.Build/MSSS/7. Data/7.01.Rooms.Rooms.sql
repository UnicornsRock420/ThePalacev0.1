IF NOT EXISTS(SELECT TOP 1 1 FROM [Rooms].[Rooms] WHERE [RoomID] = 42) BEGIN
	INSERT [Rooms].[Rooms] ([RoomID], [Name], [Flags], [CreateDate], [LastModified], [OrderID]) VALUES (42, N'The Fated Hour', 0x0100, CAST(N'2018-09-27T00:00:00.000' AS DateTime), NULL, 0)
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Rooms].[Rooms] WHERE [RoomID] = 69) BEGIN
	INSERT [Rooms].[Rooms] ([RoomID], [Name], [Flags], [CreateDate], [LastModified], [OrderID]) VALUES (69, N'The Loading Program...', 0x0100, CAST(N'2018-09-27T00:00:00.000' AS DateTime), NULL, 0)
END
GO
