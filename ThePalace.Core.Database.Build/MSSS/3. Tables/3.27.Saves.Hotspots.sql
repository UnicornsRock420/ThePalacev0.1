IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Saves].[Hotspots]')) BEGIN
	CREATE TABLE [Saves].[Hotspots](
		[UserID] [int] NOT NULL,
		[RoomID] [smallint] NOT NULL,
		[HotspotID] [smallint] NOT NULL,
		[Flags] [int] NOT NULL,
		[Type] [smallint] NOT NULL,
		[Name] [nvarchar](1024) NOT NULL,
		[State] [smallint] NOT NULL,
		[ScriptEventMask] [int] NULL,
		[SecureInfo] [int] NULL,
		[RefCon] [int] NULL,
		[LocH] [smallint] NULL,
		[LocV] [smallint] NULL,
		[Dest] [smallint] NULL,
		[Script] [nvarchar](max) NULL,
	 CONSTRAINT [PK_Hotspots] PRIMARY KEY CLUSTERED 
	(
		[UserID] ASC,
		[RoomID] ASC,
		[HotspotID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	PRINT 'CREATED [Saves].[Hotspots]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Saves].[Hotspots]'
END
GO
