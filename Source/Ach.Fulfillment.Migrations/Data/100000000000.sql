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
/* Table: "User"                                                */
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
   UserPasswordCredentialId int                  identity,
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

alter table ach.PartnerUser
   add constraint FK_PARTNERUSER_USER foreign key (UserId)
      references ach."User" (UserId)
GO

alter table ach.PartnerUser
   add constraint FK_PARTNERUSER_PARTNER foreign key (PartnerId)
      references ach.Partner (PartnerId)
GO

alter table ach.Permission
   add constraint FK_PERMISSION_ROLE foreign key (RoleId)
      references ach.Role (RoleId)
GO

alter table ach."User"
   add constraint FK_USER_ROLE foreign key (RoleId)
      references ach.Role (RoleId)
GO

alter table ach.UserPasswordCredential
   add constraint FK_USERPASSWORDCREDENTI_USER foreign key (UserId)
      references ach."User" (UserId)
GO

