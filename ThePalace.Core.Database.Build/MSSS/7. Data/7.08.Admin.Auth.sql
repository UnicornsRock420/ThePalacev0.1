IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Auth] WHERE [UserID] = 1) BEGIN
	INSERT [Admin].[Auth] ([UserID], [AuthType], [Value], [Ctr], [Crc]) VALUES (1, 1, N'test', NULL, NULL)
	--INSERT [Admin].[Auth] ([UserID], [AuthType], [Value], [Ctr], [Crc]) VALUES (1, 2, N'98.167.220.154', NULL, NULL)
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Auth] WHERE [UserID] = 2) BEGIN
	INSERT [Admin].[Auth] ([UserID], [AuthType], [Value], [Ctr], [Crc]) VALUES (2, 2, N'127.0.0.1', NULL, NULL)
	--INSERT [Admin].[Auth] ([UserID], [AuthType], [Value], [Ctr], [Crc]) VALUES (2, 2, N'174.55.248.89', NULL, NULL)
END
GO
