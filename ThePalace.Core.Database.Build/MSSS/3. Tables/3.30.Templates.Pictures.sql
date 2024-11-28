IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Templates].[Pictures]')) BEGIN
	CREATE TABLE [Templates].[Pictures](
		[TemplateID] [int] NOT NULL,
		[PictureID] [smallint] NOT NULL,
		[Name] [nvarchar](1024) NOT NULL,
		[TransColor] [smallint] NULL,
	 CONSTRAINT [PK_HotspotPictures] PRIMARY KEY CLUSTERED 
	(
		[TemplateID] ASC,
		[PictureID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Templates].[Pictures]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Templates].[Pictures]'
END
GO
