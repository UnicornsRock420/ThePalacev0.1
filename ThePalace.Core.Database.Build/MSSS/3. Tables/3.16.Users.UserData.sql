IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Users].[UserData]')) BEGIN
	CREATE TABLE [Users].[UserData](
		[UserID] [int] NOT NULL,
		[RoomPosV] [smallint] NOT NULL,
		[RoomPosH] [smallint] NOT NULL,
		[FaceNbr] [smallint] NOT NULL,
		[ColorNbr] [smallint] NOT NULL,
		[AwayFlag] [smallint] NOT NULL,
		[OpenToMsgs] [smallint] NOT NULL,
	 CONSTRAINT [PK_Rooms_1] PRIMARY KEY CLUSTERED 
	(
		[UserID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Users].[UserData]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Users].[UserData]'
END
GO
