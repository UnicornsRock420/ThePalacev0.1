IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Admin].[Config]')) BEGIN
	CREATE TABLE [Admin].[Config](
		[Key] [nvarchar](50) NOT NULL,
		[Value] [nvarchar](100) NULL,
	 CONSTRAINT [PK_Config] PRIMARY KEY CLUSTERED 
	(
		[Key] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Admin].[Config]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Admin].[Config]'
END
GO
