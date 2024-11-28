IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Templates].[LooseProps]')) BEGIN
	CREATE TABLE [Templates].[LooseProps](
		[TemplateID] [int] NOT NULL,
		[OrderID] [int] NOT NULL,
		[AssetID] [int] NOT NULL,
		[AssetCRC] [int] NOT NULL,
		[Flags] [int] NOT NULL,
		[RefCon] [int] NULL,
		[LocH] [smallint] NOT NULL,
		[LocV] [smallint] NOT NULL,
	 CONSTRAINT [PK_LooseProps] PRIMARY KEY CLUSTERED 
	(
		[TemplateID] ASC,
		[OrderID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Templates].[LooseProps]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Templates].[LooseProps]'
END
GO
