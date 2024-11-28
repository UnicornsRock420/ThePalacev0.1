IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Rooms].[DrawCmds]')) BEGIN
	CREATE TABLE [Rooms].[DrawCmds](
		[RoomID] [smallint] NOT NULL,
		[DrawCmdID] [int] NOT NULL,
		[DrawCmdType] [smallint] NOT NULL,
		[Data] [varbinary](max) NULL,
	 CONSTRAINT [PK_DrawCmds] PRIMARY KEY CLUSTERED
	(
		[RoomID] ASC,
		[DrawCmdID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Rooms].[DrawCmds]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Rooms].[DrawCmds]'
END
GO
