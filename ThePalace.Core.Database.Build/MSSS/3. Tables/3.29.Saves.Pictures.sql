IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Saves].[Pictures]')) BEGIN
	CREATE TABLE [Saves].[Pictures](
		[UserID] [int] NOT NULL,
		[RoomID] [smallint] NOT NULL,
		[PictureID] [smallint] NOT NULL,
		[Name] [nvarchar](1024) NOT NULL,
		[TransColor] [smallint] NULL,
	 CONSTRAINT [PK_HotspotPictures] PRIMARY KEY CLUSTERED 
	(
		[UserID] ASC,
		[RoomID] ASC,
		[PictureID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Saves].[Pictures]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Saves].[Pictures]'
END
GO
