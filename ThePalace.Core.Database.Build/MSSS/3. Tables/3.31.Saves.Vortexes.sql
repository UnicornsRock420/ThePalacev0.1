IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Saves].[Vortexes]')) BEGIN
	CREATE TABLE [Saves].[Vortexes](
		[UserID] [int] NOT NULL,
		[RoomID] [smallint] NOT NULL,
		[HotspotID] [smallint] NOT NULL,
		[VortexID] [smallint] NOT NULL,
		[LocH] [smallint] NOT NULL,
		[LocV] [smallint] NOT NULL,
	 CONSTRAINT [PK_Vortexes] PRIMARY KEY CLUSTERED 
	(
		[UserID] ASC,
		[RoomID] ASC,
		[HotspotID] ASC,
		[VortexID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Saves].[Vortexes]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Saves].[Vortexes]'
END
GO
