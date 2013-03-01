SET IDENTITY_INSERT [ach].[Role] ON
	INSERT INTO [ach].[Role] (RoleId, Name, Created) VALUES (3, 'DefaultUser', GetDate())
SET IDENTITY_INSERT [ach].[Role] OFF

SET IDENTITY_INSERT [ach].[Partner] ON
	INSERT INTO [ach].[Partner] (PartnerId, Name, [Disabled], Created) values (2, 'PPS', 0, GetDate())
SET IDENTITY_INSERT [ach].[Partner] OFF

SET IDENTITY_INSERT [ach].[PartnerDetail] ON
	INSERT INTO [ach].[PartnerDetail] ( PartnerDetailId, PartnerId, ImmediateDestination, CompanyIdentification, Destination, OriginOrCompanyName, CompanyName, DiscretionaryData,DFIIdentification, Created)
	       VALUES (2, 2, 'b111000012', '1234567890', 'Bank of America RIC', 'PriorityPaymentSystem', 'PPS', '', '11100001', getdate() )	
SET IDENTITY_INSERT [ach].[PartnerDetail] OFF

SET IDENTITY_INSERT [ach].[User] ON
	INSERT INTO [ach].[User] (UserId, RoleId, Name, Email, Deleted, Created) VALUES (3, 3, 'PPSUser', 'pps.user@tetsuper.com', 0, GetDate())	
SET IDENTITY_INSERT [ach].[User] OFF

SET IDENTITY_INSERT [ach].[PartnerUser] ON
	INSERT INTO [ach].[PartnerUser] (PartnerUserId, PartnerId, UserId) VALUES (3, 2, 3)	
SET IDENTITY_INSERT [ach].[PartnerUser] OFF

