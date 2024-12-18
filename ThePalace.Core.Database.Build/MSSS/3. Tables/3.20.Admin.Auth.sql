IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Admin].[Auth]')) BEGIN
	CREATE TABLE [Admin].[Auth](
		[UserID] [int] NOT NULL,
		[AuthType] [tinyint] NOT NULL,
		[Value] [nvarchar](255) NULL,
		[Ctr] [int] NULL,
		[Crc] [int] NULL,
	 CONSTRAINT [PK_Auths] PRIMARY KEY CLUSTERED 
	(
		[UserID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Admin].[Auth]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Admin].[Auth]'
END
GO
