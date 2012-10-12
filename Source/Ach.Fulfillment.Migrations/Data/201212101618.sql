SET IDENTITY_INSERT [ach].[PartnerDetail] ON
	INSERT INTO [ach].[PartnerDetail] ( PartnerDetailId, PartnerId, ImmediateDestination, CompanyIdentification, Destination, OriginOrCompanyName, CompanyName, DiscretionaryData,DFIIdentification, Created)
	       VALUES (1, 1, 'b121108250', '1234567890', 'Bank of America RIC', 'PriorityPaymentSystem', 'PPS', '', '12345678', getdate() )	
SET IDENTITY_INSERT [ach].[PartnerDetail] OFF