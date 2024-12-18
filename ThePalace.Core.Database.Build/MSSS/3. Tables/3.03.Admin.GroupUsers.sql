IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Admin].[GroupUsers]')) BEGIN
	CREATE TABLE [Admin].[GroupUsers](
		[GroupID] [int] NOT NULL,
		[UserID] [int] NOT NULL,
	 CONSTRAINT [PK_GroupUsers] PRIMARY KEY CLUSTERED 
	(
		[GroupID] ASC,
		[UserID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Admin].[GroupUsers]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Admin].[GroupUsers]'
END
GO
