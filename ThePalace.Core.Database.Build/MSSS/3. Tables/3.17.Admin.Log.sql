IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Admin].[Log]')) BEGIN
	CREATE TABLE [Admin].[Log](
		[LogID] [int] IDENTITY(1,1) NOT NULL,
		[MessageType] [nvarchar](50) NOT NULL,
		[MachineName] [nvarchar](200) NOT NULL,
		[ApplicationName] [nvarchar](200) NOT NULL,
		[ProcessID] [bigint] NOT NULL,
		[CreateDate] [datetime] NOT NULL,
		[Message] [nvarchar](max) NOT NULL,
		[StackTrace] [nvarchar](max) NULL,
	 CONSTRAINT [PK_Log_1] PRIMARY KEY CLUSTERED 
	(
		[LogID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Admin].[Log]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Admin].[Log]'
END
GO
