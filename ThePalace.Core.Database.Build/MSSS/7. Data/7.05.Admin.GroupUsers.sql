IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[GroupUsers] WHERE [GroupID] = 1 AND [UserID] = 1) BEGIN
	INSERT [Admin].[GroupUsers] ([GroupID], [UserID]) VALUES (1, 1)
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[GroupUsers] WHERE [GroupID] = 1 AND [UserID] = 2) BEGIN
	INSERT [Admin].[GroupUsers] ([GroupID], [UserID]) VALUES (1, 2)
END
GO
