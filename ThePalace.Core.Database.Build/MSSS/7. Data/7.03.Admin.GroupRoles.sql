IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[GroupRoles] WHERE [GroupID] = 1 AND [RoleID] = 1) BEGIN
	INSERT [Admin].[GroupRoles] ([GroupID], [RoleID]) VALUES (1, 1)
END
GO

IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin].[GroupRoles] WHERE [GroupID] = 2 AND [RoleID] = 2) BEGIN
	INSERT [Admin].[GroupRoles] ([GroupID], [RoleID]) VALUES (2, 2)
END
GO
