IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.TABLES WHERE OBJECT_ID = OBJECT_ID('[Assets].[Metadata]')) BEGIN
	CREATE TABLE [Assets].[Metadata](
		[AssetID] [int] NOT NULL,
		[Flags] [int] NOT NULL,
		[Format] [NVARCHAR](50) NOT NULL,
		[Width] [smallint] NOT NULL,
		[Height] [smallint] NOT NULL,
		[OffsetX] [smallint] NOT NULL,
		[OffsetY] [smallint] NOT NULL,
	 CONSTRAINT [PK_Assets_Metadata] PRIMARY KEY CLUSTERED 
	(
		[AssetID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	PRINT 'CREATED [Assets].[Metadata]'
END ELSE BEGIN
	PRINT 'ALREADY EXISTS [Assets].[Metadata]'
END
GO
