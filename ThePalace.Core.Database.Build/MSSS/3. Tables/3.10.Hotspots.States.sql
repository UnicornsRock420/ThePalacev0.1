IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Hotspots].[States]')) BEGIN
	CREATE TABLE [Hotspots].[States](
		[RoomID] [smallint] NOT NULL,
		[HotspotID] [smallint] NOT NULL,
		[StateID] [smallint] NOT NULL,
		[PictureID] [smallint] NULL,
		[LocH] [smallint] NULL,
		[LocV] [smallint] NULL,
	 CONSTRAINT [PK_HotspotStates_1] PRIMARY KEY CLUSTERED 
	(
		[RoomID] ASC,
		[HotspotID] ASC,
		[StateID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Hotspots].[States]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Hotspots].[States]'
END
GO
