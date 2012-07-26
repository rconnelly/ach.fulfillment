SET IDENTITY_INSERT [ach].[Role] ON
	INSERT INTO [ach].[Role] (RoleId, Name, Created) VALUES (1, 'Administrator', GetDate())
	INSERT INTO [ach].[Role] (RoleId, Name, Created) VALUES (2, 'Super Administrator', GetDate())
SET IDENTITY_INSERT [ach].[Role] OFF

SET IDENTITY_INSERT [ach].[Permission] ON
	INSERT INTO [ach].[Permission] (PermissionId, [RoleId], Name, Created) VALUES (1, 1, 'Admin', GetDate())
	INSERT INTO [ach].[Permission] (PermissionId, [RoleId], Name, Created) VALUES (2, 2, 'SuperAdmin', GetDate())
SET IDENTITY_INSERT [ach].[Permission] OFF

SET IDENTITY_INSERT [ach].[Partner] ON
	INSERT INTO [ach].[Partner] (PartnerId, Name, [Disabled], Created) values (1, 'PPS', 0, GetDate())
SET IDENTITY_INSERT [ach].[Partner] OFF

SET IDENTITY_INSERT [ach].[User] ON
	INSERT INTO [ach].[User] (UserId, RoleId, Name, Email, Deleted, Created) VALUES (1, 1, 'Mr John Smith', 'john.smith@tetsuper.com', 0, GetDate())
	INSERT INTO [ach].[User] (UserId, RoleId, Name, Email, Deleted, Created) VALUES (2, 2, 'Mrs Wo Smith', 'wo.smith@tetsuper.com', 0, GetDate())
SET IDENTITY_INSERT [ach].[User] OFF

SET IDENTITY_INSERT [ach].[UserPasswordCredential] ON
	INSERT INTO [ach].[UserPasswordCredential] (UserPasswordCredentialId, UserId, [Login], PasswordHash, PasswordSalt, Created) values (1, 1, 'ach', 'spfoYfPKWBvMv8Vk7ond80NGr/J6xP3mmmgkeQ/Cd/96poBQv05h7A==', 'lfeyTsixmrUjQgNPkXj1', GetDate())
	INSERT INTO [ach].[UserPasswordCredential] (UserPasswordCredentialId, UserId, [Login], PasswordHash, PasswordSalt, Created) values (2, 2, 'wo', 'spfoYfPKWBvMv8Vk7ond80NGr/J6xP3mmmgkeQ/Cd/96poBQv05h7A==', 'lfeyTsixmrUjQgNPkXj1', GetDate())
SET IDENTITY_INSERT [ach].[UserPasswordCredential] OFF

SET IDENTITY_INSERT [ach].[PartnerUser] ON
	INSERT INTO [ach].[PartnerUser] (PartnerUserId, PartnerId, UserId) VALUES (1, 1, 1)
	INSERT INTO [ach].[PartnerUser] (PartnerUserId, PartnerId, UserId) VALUES (2, 1, 2)
SET IDENTITY_INSERT [ach].[PartnerUser] OFF
