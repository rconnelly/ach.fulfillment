/*==============================================================*/
/* User: ach                                                    */
/*==============================================================*/
create schema ach authorization dbo
GO


/*==============================================================*/
/* Table: Partner                                               */
/*==============================================================*/
create table ach.Partner (
   PartnerId            int                  identity,
   Name                 nvarchar(255)        not null,
   Disabled             bit                  not null constraint DF_DISABLED_PARTNER default 0,
   Created              datetime             not null,
   Modified             datetime             null,
   constraint PK_PARTNER primary key (PartnerId)
)
GO

/*==============================================================*/
/* Index: UX_PARTNER                                            */
/*==============================================================*/
create unique index UX_PARTNER on ach.Partner (
   Name ASC
)
GO


/*==============================================================*/
/* Table: PartnerUser                                           */
/*==============================================================*/
create table ach.PartnerUser (
   PartnerUserId        int                  identity,
   PartnerId            int                  not null,
   UserId               int                  not null,
   constraint PK_PARTNERUSER primary key (PartnerUserId)
)
GO

/*==============================================================*/
/* Index: UX_USER                                               */
/*==============================================================*/
create unique index UX_USER on ach.PartnerUser (
   UserId ASC
)
GO


/*==============================================================*/
/* Table: Permission                                            */
/*==============================================================*/
create table ach.Permission (
   PermissionId         int                  identity,
   RoleId               int                  not null,
   Name                 nvarchar(255)        not null,
   Created              datetime             not null,
   Modified             datetime             null,
   constraint PK_PERMISSION primary key (PermissionId)
)
GO

/*==============================================================*/
/* Index: UX_PERMISSION                                         */
/*==============================================================*/
create unique index UX_PERMISSION on ach.Permission (
   Name ASC,
   RoleId ASC
)
GO


/*==============================================================*/
/* Table: Role                                                  */
/*==============================================================*/
create table ach.Role (
   RoleId               int                  identity,
   Name                 nvarchar(255)        not null,
   Created              datetime             not null,
   Modified             datetime             null,
   constraint PK_ROLE primary key (RoleId)
)
GO


/*==============================================================*/
/* Table: User                                                  */
/*==============================================================*/
create table ach."User" (
   UserId               int                  identity,
   RoleId               int                  not null,
   Name                 nvarchar(255)        not null,
   Email                nvarchar(255)        null,
   Deleted              bit                  not null constraint DF_DELETED_USER default 0,
   Created              datetime             not null,
   Modified             datetime             null,
   constraint PK_USER primary key (UserId)
)
GO


/*==============================================================*/
/* Table: UserPasswordCredential                                */
/*==============================================================*/
create table ach.UserPasswordCredential (
   UserPasswordCredentialId int              identity,
   UserId               int                  not null,
   Login                nvarchar(255)        not null,
   PasswordHash         nvarchar(100)        not null,
   PasswordSalt         nvarchar(100)        not null,
   Created              datetime             not null,
   Modified             datetime             null,
   constraint PK_USERPASSWORDCREDENTIAL primary key (UserPasswordCredentialId)
)
GO

/*==============================================================*/
/* Index: UX_LOGIN                                              */
/*==============================================================*/
create unique index UX_LOGIN on ach.UserPasswordCredential (
   Login ASC
)
GO

/*==============================================================*/
/* Index: UX_USER                                               */
/*==============================================================*/
create unique index UX_USER on ach.UserPasswordCredential (
   UserId ASC
)
GO

/*==============================================================*/
/* References:                                                  */
/*  PartnerUser                                                 */
/*  Permission                                                  */
/*  User                                                        */
/*  UserPasswordCredential                                      */
/*==============================================================*/
alter table ach.PartnerUser
   add constraint FK_PARTNERUSER_USER foreign key (UserId)
      references ach."User" (UserId)
        on delete cascade
GO

alter table ach.PartnerUser
   add constraint FK_PARTNERUSER_PARTNER foreign key (PartnerId)
      references ach.Partner (PartnerId)
GO

alter table ach.Permission
   add constraint FK_PERMISSION_ROLE foreign key (RoleId)
      references ach.Role (RoleId)
        on delete cascade
GO

alter table ach."User"
   add constraint FK_USER_ROLE foreign key (RoleId)
      references ach.Role (RoleId)
GO

alter table ach.UserPasswordCredential
   add constraint FK_USERPASSWORDCREDENTI_USER foreign key (UserId)
      references ach."User" (UserId)
        on delete cascade
GO


/*==============================================================*/
/* Table: PartnerDetail                                         */
/*==============================================================*/
create table ach.PartnerDetail (
   PartnerDetailId       int                  identity,
   PartnerId             int                  not null,
   ImmediateDestination  nvarchar(10)         null,
   CompanyIdentification nvarchar(10)         null,
   Destination           nvarchar(23)         null,
   OriginOrCompanyName   nvarchar(23)         null,
   CompanyName           nvarchar(16)         null,
   DiscretionaryData     nvarchar(20)         null,
   DFIIdentification     nvarchar(8)          null,
   Created               datetime             not null,
   Modified              datetime             null,
   constraint PK_PARTNERDETAIL primary key (PartnerDetailId)
)
GO

/*==============================================================*/
/* References: PartnerDetail                                    */
/*==============================================================*/
alter table ach.PartnerDetail
   add constraint FK_PARTNERDETAIL_PARTNER foreign key (PartnerId)
      references ach.Partner (PartnerId)
        on delete cascade
GO


/*==============================================================*/
/* Table: AchTransaction                                        */
/*==============================================================*/
create table ach.AchTransaction (
   AchTransactionId     int                  identity,
   PartnerId            int                  not null,
   IndividualIdNumber   nvarchar(15)         not null,
   ReceiverName         nvarchar(22)         not null,
   EntryDescription     nvarchar(10)         not null,
   EntryDate            datetime             null,
   TransactionCode      nvarchar(2)          not null,
   TransitRoutingNumber nvarchar(9)          not null,
   DFIAccountId         nvarchar(17)         not null,
   Amount               decimal(10,2)        not null,
   ServiceClassCode     nvarchar(3)          not null,
   EntryClassCode       nvarchar(3)          not null,
   PaymentRelatedInfo   nvarchar(80)         null,
   CallbackUrl          nvarchar(2000)       not null,
   TransactionStatus    int                  not null,
   NotifiedTransactionStatus int constraint DF_NOTIFIEDTRANSACTIONSTATUS_ACHTRANSACTION DEFAULT (0) not null,
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

/*==============================================================*/
/* References: AchTransaction                                   */
/*==============================================================*/
alter table ach.AchTransaction
   add constraint FK_ACHTRANSACTION_PARTNER foreign key (PartnerId)
      references ach.Partner (PartnerId)
        on delete cascade
GO


/*==============================================================*/
/* Table: AchFile                                               */
/*==============================================================*/
create table ach.AchFile (
   AchFileId            int                  identity,
   PartnerId            int                  not null,
   Name                 nvarchar(16)         not null,
   FileIdModifier       nchar(1)             not null,
   FileStatus           int                  null,
   Created              datetime             not null,
   Modified             datetime             null,
   constraint PK_ACHFILE primary key (AchFileId))
GO

/*==============================================================*/
/* Index: UX_NAME	                                            */
/*==============================================================*/
create unique index UX_NAME on ach.AchFile (
    PartnerId ASC,
    Name ASC
)
GO

/*==============================================================*/
/* References: AchFile                                          */
/*==============================================================*/
alter table ach.AchFile
   add constraint FK_ACHFILE_PARTNER foreign key (PartnerId)
      references ach.Partner (PartnerId)
        on delete cascade
GO


/*==============================================================*/
/* Table: AchFileContent                                        */
/*==============================================================*/
create table ach.AchFileContent(
    AchFileContentId uniqueidentifier ROWGUIDCOL constraint DF_ACHFILECONTENTID_ACHFILECONTENT default (newid()) not null,
    AchFileId int not null,
    SystemFile varbinary(max) filestream null,
    constraint PK_ACHFILECONTENT primary key nonclustered (AchFileContentId)
)
GO

/*==============================================================*/
/* Index: UX_ACHFILEID                                          */
/*==============================================================*/
create unique clustered index UX_ACHFILEID on ach.AchFileContent (
   AchFileId ASC
)
GO

/*==============================================================*/
/* References: AchFileContent                                   */
/*==============================================================*/
alter table ach.AchFileContent
   add constraint FK_ACHFILECONTENT_ACHFILE foreign key (AchFileId)
      references ach.AchFile (AchFileId)
        on delete cascade
GO


/*==============================================================*/
/* Table: AchFileTransaction                                    */
/*==============================================================*/
create table ach.AchFileTransaction (
   AchFileTransactionId int                 identity,
   AchFileId            int                 not null,
   AchTransactionId     int                 not null,
   constraint PK_ACHFILETRANSACTION primary key nonclustered (AchFileTransactionId)
)
GO

/*==============================================================*/
/* Index: UX_ACHFILETRANSACTION                                 */
/*==============================================================*/
create unique clustered index UX_ACHFILEID on ach.AchFileTransaction (
   AchFileId ASC,
   AchTransactionId ASC
)
GO

/*==============================================================*/
/* References: AchFileTransaction                               */
/*==============================================================*/
alter table ach.AchFileTransaction
   add constraint FK_ACHFILETRANSACTION_TRANSACTION foreign key (AchTransactionId)
      references ach.AchTransaction (AchTransactionId)

alter table ach.AchFileTransaction
   add constraint FK_ACHFILETRANSACTION_FILE foreign key (AchFileId)
      references ach.AchFile (AchFileId)
        on delete cascade
GO    