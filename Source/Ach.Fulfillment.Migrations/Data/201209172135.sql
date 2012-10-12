
/*==============================================================*/
/* Table: "AchTransaction"                                                */
/*==============================================================*/
create table ach."AchTransaction" (
   AchTransactionId     int                  identity,
   PartnerId            int                  not null,
   IndividualIdNumber	nvarchar(15)         not null,
   ReceiverName         nvarchar(22)         not null,
   EntryDescription		nvarchar(10)         not null,  
   EntryDate			datetime			 null,   
   TransactionCode		nvarchar(2)			 not null,
   TransitRoutingNumber	nvarchar(9)          not null,
   DFIAccountId			nvarchar(17)         not null,
   Amount				decimal(10,2)		 not null,
   ServiceClassCode		nvarchar(3)			 not null,
   EntryClassCode		nvarchar(3)			 not null,
   PaymentRelatedInfo   nvarchar(80)		 null,
   CallbackUrl			nvarchar(255)        not null,
   TransactionStatus	int					 null,   
   Locked				bit					 not null default 0,
   Created              datetime             not null,
   Modified             datetime             null,
   constraint PK_ACHTRANSACTION primary key (AchTransactionId)
)
GO

/*==============================================================*/
/* Index: UX_ACHTRANSACTION                                     */
/*==============================================================*/
create unique index UX_ACHTRANSACTION on ach.AchTransaction (
   AchTransactionId ASC
)
GO

alter table ach.AchTransaction
   add constraint FK_ACHTRANSACTION_PARTNER foreign key (PartnerId)
      references ach."Partner" (PartnerId)
GO


/*==============================================================*/
/* Table: "File"                                                */
/*==============================================================*/
create table ach."AchFile" (
   AchFileId			int                  identity,
   PartnerId			int                  not null,
   Name					nvarchar(16)         not null,
   FileIdModifier		nvarchar(1)			 not null,
   FileStatus			int					 null,
   Locked				bit					 not null default 0,
   Created              datetime             not null,
   Modified             datetime             null,
   constraint PK_ACHFILE primary key (AchFileId))
GO
alter table ach."AchFile"
   add constraint FK_ACHFILE_PARTNER foreign key (PartnerId)
      references ach."Partner" (PartnerId)

/*==============================================================*/
/* Table: "FileTransaction"                                     */
/*==============================================================*/
create table ach."AchFileTransaction" (
   AchFileTransactionId int				 identity,
   AchFileId			 int             not null,
   AchTransactionId	 int				 not null,
   constraint PK_ACHFILETRANSACTION primary key (AchFileTransactionId)
)

alter table ach.AchFileTransaction
   add constraint FK_ACHFILETRANSACTION_TRANSACTION foreign key (AchTransactionId)
      references ach."AchTransaction" (AchTransactionId)

alter table ach.AchFileTransaction
   add constraint FK_ACHFILETRANSACTION_FILE foreign key (AchFileId)
      references ach."AchFile" (AchFileId)
GO

/*==============================================================*/
/* Table: "PartnerDetail"                                       */
/*==============================================================*/
create table ach."PartnerDetail" (
   PartnerDetailId		 int                  identity,
   PartnerId			 int				  not null,
   ImmediateDestination	 nvarchar(10)         null,
   CompanyIdentification nvarchar(10)         null,
   Destination			 nvarchar(23)         null,
   OriginOrCompanyName	 nvarchar(23)		  null,
   CompanyName			 nvarchar(16)		  null,
   DiscretionaryData	 nvarchar(20)		  null,
   DFIIdentification	 nvarchar(8)		  null,
   Created               datetime             not null,
   Modified              datetime             null,
   constraint PK_PARTNERDETAIL primary key (PartnerDetailId)
)
GO
alter table ach.PartnerDetail
   add constraint FK_PARTNERDETAIL_PARTNER foreign key (PartnerId)
      references ach."Partner" (PartnerId)