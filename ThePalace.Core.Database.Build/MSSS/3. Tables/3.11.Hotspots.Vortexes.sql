IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Hotspots].[Vortexes]')) BEGIN
	CREATE TABLE [Hotspots].[Vortexes](
		[RoomID] [smallint] NOT NULL,
		[HotspotID] [smallint] NOT NULL,
		[VortexID] [smallint] NOT NULL,
		[LocH] [smallint] NOT NULL,
		[LocV] [smallint] NOT NULL,
	 CONSTRAINT [PK_Vortexes] PRIMARY KEY CLUSTERED 
	(
		[RoomID] ASC,
		[HotspotID] ASC,
		[VortexID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Hotspots].[Vortexes]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [UsHotspotsers].[Vortexes]'
END
GO
