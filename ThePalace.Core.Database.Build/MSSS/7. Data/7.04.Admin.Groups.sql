IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Groups] WHERE [Name] = 'God') BEGIN
	INSERT [Admin].[Groups] ([Name]) VALUES (N'God')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Groups] WHERE [Name] = 'Wizard') BEGIN
	INSERT [Admin].[Groups] ([Name]) VALUES (N'Wizard')
END
GO
