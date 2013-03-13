set IDENTITY_INSERT [ach].[Role] on
    insert into [ach].[Role] (RoleId, Name, Created) values (1, 'Administrator', GetDate())
    insert into [ach].[Role] (RoleId, Name, Created) values (2, 'Super Administrator', GetDate())
    insert into [ach].[Role] (RoleId, Name, Created) VALUES (3, 'DefaultUser', GetDate())
set IDENTITY_INSERT [ach].[Role] off

set IDENTITY_INSERT [ach].[Permission] on
    insert into [ach].[Permission] (PermissionId, [RoleId], Name, Created) values (1, 1, 'Admin', GetDate())
    insert into [ach].[Permission] (PermissionId, [RoleId], Name, Created) values (2, 2, 'SuperAdmin', GetDate())
set IDENTITY_INSERT [ach].[Permission] off

set IDENTITY_INSERT [ach].[Partner] on
    insert into [ach].[Partner] (PartnerId, Name, [Disabled], Created) values (1, 'Test', 0, GetDate())
    insert into [ach].[Partner] (PartnerId, Name, [Disabled], Created) values (2, 'PPS', 0, GetDate())
set IDENTITY_INSERT [ach].[Partner] off

set IDENTITY_INSERT [ach].[PartnerDetail] on
    insert into [ach].[PartnerDetail] ( PartnerDetailId, PartnerId, ImmediateDestination, CompanyIdentification, Destination, OriginOrCompanyName, CompanyName, DFIIdentification, Created) values (1, 2, 'b111000012', '1234567890', 'Bank of America RIC', 'PriorityPaymentSystem', 'PPS', '11100001', getdate())
set IDENTITY_INSERT [ach].[PartnerDetail] off

set IDENTITY_INSERT [ach].[User] on
    insert into [ach].[User] (UserId, RoleId, Name, Email, Deleted, Created) values (1, 1, 'Mr John Smith', 'john.smith@tetsuper.com', 0, GetDate())
    insert into [ach].[User] (UserId, RoleId, Name, Email, Deleted, Created) values (2, 2, 'Mrs Wo Smith', 'wo.smith@tetsuper.com', 0, GetDate())
    insert into [ach].[User] (UserId, RoleId, Name, Email, Deleted, Created) values (3, 3, 'PPSUser', 'pps.user@tetsuper.com', 0, GetDate())    
set IDENTITY_INSERT [ach].[User] off

set IDENTITY_INSERT [ach].[UserPasswordCredential] on
    insert into [ach].[UserPasswordCredential] (UserPasswordCredentialId, UserId, [Login], PasswordHash, PasswordSalt, Created) values (1, 1, 'admin', 'tzz372n5WtIAgY5SvSclpfUyaRuXoMpEgMiRnMptbf29iqGm1RVznw==', 'fMjtdJTx2XBLdpmDXWJV', GetDate())
    insert into [ach].[UserPasswordCredential] (UserPasswordCredentialId, UserId, [Login], PasswordHash, PasswordSalt, Created) values (2, 2, 'super', 'kLtziaXFt7C7lVZN6LNRXj9lb+Ybhzc1/ymyNvfJm/ZjUltvXMxyyg==', '7CC5dg2d/sYexAMJ/k7+', GetDate())
set IDENTITY_INSERT [ach].[UserPasswordCredential] off

set IDENTITY_INSERT [ach].[PartnerUser] on
    insert into [ach].[PartnerUser] (PartnerUserId, PartnerId, UserId) values (1, 1, 1)
    insert into [ach].[PartnerUser] (PartnerUserId, PartnerId, UserId) values (2, 1, 2)
    insert into [ach].[PartnerUser] (PartnerUserId, PartnerId, UserId) values (3, 2, 3)    
set IDENTITY_INSERT [ach].[PartnerUser] off