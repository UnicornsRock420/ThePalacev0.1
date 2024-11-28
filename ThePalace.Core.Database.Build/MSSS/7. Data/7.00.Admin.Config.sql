IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'AppCacheTTL') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'AppCacheTTL', N'3')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'ServerRequiresAuthentication') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'ServerRequiresAuthentication', N'FALSE')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'BindAddress') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'BindAddress', N'0.0.0.0')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'BindPalacePort') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'BindPalacePort', N'9998')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'BindProxyPort') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'BindProxyPort', N'9999')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'BindAephixPort') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'BindAephixPort', N'10000')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'ListenBacklog') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'ListenBacklog', N'100')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'MediaUrl') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'MediaUrl', N'https://localhost:5000/media')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'MaxUserID') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'MaxUserID', N'9999')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'MaxRoomOccupancy') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'MaxRoomOccupancy', N'45')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'FloodControlThreadshold_RawCount') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'FloodControlThreadshold_RawCount', N'100')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'PassiveIdleTimeout_InSeconds') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'PassiveIdleTimeout_InSeconds', N'15')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'IdleTimeout_InMinutes') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'IdleTimeout_InMinutes', N'10')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'DOSTimeout_InSeconds') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'DOSTimeout_InSeconds', N'25')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'LatencyMaxCounter') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'LatencyMaxCounter', N'25')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'ServerName') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'ServerName', N'The Construct of the Matrix')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'FloodControlThreadshold_InMilliseconds') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'FloodControlThreadshold_InMilliseconds', N'1000')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'ThreadAbortWait_InMilliseconds') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'ThreadAbortWait_InMilliseconds', N'1000')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'ThreadPause_InMilliseconds') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'ThreadPause_InMilliseconds', N'500')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'ThreadWait_InMilliseconds') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'ThreadWait_InMilliseconds', N'60000')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'ThreadShutdownWait_InMilliseconds') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'ThreadShutdownWait_InMilliseconds', N'2500')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'ThreadManageConnections_InMilliseconds') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'ThreadManageConnections_InMilliseconds', N'750')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'ThreadRefreshSettings_InMilliseconds') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'ThreadRefreshSettings_InMilliseconds', N'15000')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'ThreadManageAssetsMax') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'ThreadManageAssetsMax', N'4')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'ThreadManageFilesMax') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'ThreadManageFilesMax', N'4')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'ThreadManageQueueMax') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'ThreadManageQueueMax', N'4')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'KeepAliveInterval_InMilliseconds') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'KeepAliveInterval_InMilliseconds', N'15000')
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[Config] WHERE [Key] = 'KeepAliveTime_InMilliseconds') BEGIN
	INSERT [Admin].[Config] ([Key], [Value]) VALUES (N'KeepAliveTime_InMilliseconds', N'15000')
END
GO
