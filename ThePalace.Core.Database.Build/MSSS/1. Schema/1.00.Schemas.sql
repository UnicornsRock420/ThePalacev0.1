﻿IF NOT EXISTS (SELECT TOP 1 1 FROM SYS.SCHEMAS WHERE name = 'Admin')
BEGIN
	EXEC('CREATE SCHEMA [Admin]')
END
GO

IF NOT EXISTS (SELECT TOP 1 1 FROM SYS.SCHEMAS WHERE name = 'Assets')
BEGIN
	EXEC('CREATE SCHEMA [Assets]')
END
GO

IF NOT EXISTS (SELECT TOP 1 1 FROM SYS.SCHEMAS WHERE name = 'Crons')
BEGIN
	EXEC('CREATE SCHEMA [Crons]')
END
GO

IF NOT EXISTS (SELECT TOP 1 1 FROM SYS.SCHEMAS WHERE name = 'Hotspots')
BEGIN
	EXEC('CREATE SCHEMA [Hotspots]')
END
GO

IF NOT EXISTS (SELECT TOP 1 1 FROM SYS.SCHEMAS WHERE name = 'Rooms')
BEGIN
	EXEC('CREATE SCHEMA [Rooms]')
END
GO

IF NOT EXISTS (SELECT TOP 1 1 FROM SYS.SCHEMAS WHERE name = 'Saves')
BEGIN
	EXEC('CREATE SCHEMA [Saves]')
END
GO

IF NOT EXISTS (SELECT TOP 1 1 FROM SYS.SCHEMAS WHERE name = 'Templates')
BEGIN
	EXEC('CREATE SCHEMA [Templates]')
END
GO

IF NOT EXISTS (SELECT TOP 1 1 FROM SYS.SCHEMAS WHERE name = 'Users')
BEGIN
	EXEC('CREATE SCHEMA [Users]')
END
GO
