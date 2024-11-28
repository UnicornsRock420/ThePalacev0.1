IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Roles] WHERE [Name] = 'Owner') BEGIN
	INSERT [Admin].[Roles] ([Name]) VALUES (N'Owner')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Roles] WHERE [Name] = 'Moderator') BEGIN
	INSERT [Admin].[Roles] ([Name]) VALUES (N'Moderator')
END
GO
