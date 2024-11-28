IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Users] WHERE [Name] = 'Janus') BEGIN
	INSERT [Admin].[Users] ([Name]) VALUES (N'Janus')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Users] WHERE [Name] = 'Fate') BEGIN
	INSERT [Admin].[Users] ([Name]) VALUES (N'Fate')
END
GO
