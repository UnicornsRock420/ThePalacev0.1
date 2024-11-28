IF EXISTS(SELECT TOP 1 1 FROM SYS.ALL_OBJECTS WHERE OBJECT_ID = OBJECT_ID('[Admin].[ComputeCrc]')) BEGIN
	DROP FUNCTION [Admin].[ComputeCrc]

	PRINT 'Dropping [Admin].[ComputeCrc]'
END
GO

IF EXISTS(SELECT TOP 1 1 FROM SYS.ASSEMBLY_FILES WHERE NAME = 'ThePalace.SqlFunctions') BEGIN
	EXEC (N'CREATE FUNCTION [Admin].[ComputeCrc](
		@Data [varbinary](max),
		@Offset INT,
		@isAsset [bit]
	) RETURNS INT
	AS EXTERNAL NAME [ThePalace.SqlFunctions].[Cipher].[ComputeCrc]')

	PRINT N'Created [Admin].[ComputeCrc]'
END
GO
