IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Admin].[GroupRoles]')) BEGIN
	CREATE TABLE [Admin].[GroupRoles](
		[GroupID] [int] NOT NULL,
		[RoleID] [int] NOT NULL,
	 CONSTRAINT [PK_GroupRoles] PRIMARY KEY CLUSTERED 
	(
		[GroupID] ASC,
		[RoleID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Admin].[GroupRoles]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Admin].[GroupRoles]'
END
GO
