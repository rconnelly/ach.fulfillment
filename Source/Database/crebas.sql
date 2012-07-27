/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2005                    */
/* Created on:     2012.07.27 10:42:15                          */
/*==============================================================*/


alter table ach.PartnerUser
   drop constraint FK_PARTNERUSER_USER
GO

alter table ach.PartnerUser
   drop constraint FK_PARTNERUSER_PARTNER
GO

alter table ach.Permission
   drop constraint FK_PERMISSION_ROLE
GO

alter table ach."User"
   drop constraint FK_USER_ROLE
GO

alter table ach.UserPasswordCredential
   drop constraint FK_USERPASSWORDCREDENTI_USER
GO

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.Partner')
            and   name  = 'UX_PARTNER'
            and   indid > 0
            and   indid < 255)
   drop index ach.Partner.UX_PARTNER
GO

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.PartnerUser')
            and   name  = 'UX_USER'
            and   indid > 0
            and   indid < 255)
   drop index ach.PartnerUser.UX_USER
GO

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.Permission')
            and   name  = 'UX_PERMISSION'
            and   indid > 0
            and   indid < 255)
   drop index ach.Permission.UX_PERMISSION
GO

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.UserPasswordCredential')
            and   name  = 'UX_LOGIN'
            and   indid > 0
            and   indid < 255)
   drop index ach.UserPasswordCredential.UX_LOGIN
GO

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.UserPasswordCredential')
            and   name  = 'UX_USER'
            and   indid > 0
            and   indid < 255)
   drop index ach.UserPasswordCredential.UX_USER
GO

if exists (select 1
            from  sysobjects
           where  id = object_id('ach.Partner')
            and   type = 'U')
   drop table ach.Partner
GO

if exists (select 1
            from  sysobjects
           where  id = object_id('ach.PartnerUser')
            and   type = 'U')
   drop table ach.PartnerUser
GO

if exists (select 1
            from  sysobjects
           where  id = object_id('ach.Permission')
            and   type = 'U')
   drop table ach.Permission
GO

if exists (select 1
            from  sysobjects
           where  id = object_id('ach.Role')
            and   type = 'U')
   drop table ach.Role
GO

if exists (select 1
            from  sysobjects
           where  id = object_id('ach."User"')
            and   type = 'U')
   drop table ach."User"
GO

if exists (select 1
            from  sysobjects
           where  id = object_id('ach.UserPasswordCredential')
            and   type = 'U')
   drop table ach.UserPasswordCredential
GO

drop schema ach
GO

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

