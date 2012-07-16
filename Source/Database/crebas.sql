/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2005                    */
/* Created on:     2012.07.16 13:07:49                          */
/*==============================================================*/


alter table ach.PartnerUser
   drop constraint FK_PARTNERUSER_USER
go

alter table ach.PartnerUser
   drop constraint FK_PARTNERUSER_PARTNER
go

alter table ach.PermissionRole
   drop constraint FK_PERMISSIONROLE_ROLE
go

alter table ach.PermissionRole
   drop constraint FK_PERMISSIONROLE_PERMISSION
go

alter table ach."User"
   drop constraint FK_USER_ROLE
go

alter table ach.UserPasswordCredential
   drop constraint FK_USERPASSWORDCREDENTI_USER
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.Partner')
            and   name  = 'UX_PARTNER'
            and   indid > 0
            and   indid < 255)
   drop index ach.Partner.UX_PARTNER
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.PartnerUser')
            and   name  = 'UX_USER'
            and   indid > 0
            and   indid < 255)
   drop index ach.PartnerUser.UX_USER
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.Permission')
            and   name  = 'UX_PERMISSION'
            and   indid > 0
            and   indid < 255)
   drop index ach.Permission.UX_PERMISSION
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.PermissionRole')
            and   name  = 'UX_PERMISSIONROLE'
            and   indid > 0
            and   indid < 255)
   drop index ach.PermissionRole.UX_PERMISSIONROLE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.Role')
            and   name  = 'UX_ROLE'
            and   indid > 0
            and   indid < 255)
   drop index ach.Role.UX_ROLE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.UserPasswordCredential')
            and   name  = 'UX_LOGIN'
            and   indid > 0
            and   indid < 255)
   drop index ach.UserPasswordCredential.UX_LOGIN
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.UserPasswordCredential')
            and   name  = 'UX_USER'
            and   indid > 0
            and   indid < 255)
   drop index ach.UserPasswordCredential.UX_USER
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ach.Partner')
            and   type = 'U')
   drop table ach.Partner
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ach.PartnerUser')
            and   type = 'U')
   drop table ach.PartnerUser
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ach.Permission')
            and   type = 'U')
   drop table ach.Permission
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ach.PermissionRole')
            and   type = 'U')
   drop table ach.PermissionRole
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ach.Role')
            and   type = 'U')
   drop table ach.Role
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ach."User"')
            and   type = 'U')
   drop table ach."User"
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ach.UserPasswordCredential')
            and   type = 'U')
   drop table ach.UserPasswordCredential
go

drop schema ach
go

/*==============================================================*/
/* User: ach                                                    */
/*==============================================================*/
create schema ach authorization dbo
go

/*==============================================================*/
/* Table: Partner                                               */
/*==============================================================*/
create table ach.Partner (
   PartnerId            int                  identity,
   Name                 nvarchar(255)        not null,
   Created              datetime             not null,
   Modified             datetime             null,
   constraint PK_PARTNER primary key (PartnerId)
)
go

/*==============================================================*/
/* Index: UX_PARTNER                                            */
/*==============================================================*/
create unique index UX_PARTNER on ach.Partner (
   Name ASC
)
go

/*==============================================================*/
/* Table: PartnerUser                                           */
/*==============================================================*/
create table ach.PartnerUser (
   PartnerUserId        int                  identity,
   PartnerId            int                  not null,
   UserId               int                  not null,
   constraint PK_PARTNERUSER primary key (PartnerUserId)
)
go

/*==============================================================*/
/* Index: UX_USER                                               */
/*==============================================================*/
create unique index UX_USER on ach.PartnerUser (
   UserId ASC
)
go

/*==============================================================*/
/* Table: Permission                                            */
/*==============================================================*/
create table ach.Permission (
   PermissionId         int                  identity,
   Name                 nvarchar(255)        not null,
   Created              datetime             not null,
   Modified             datetime             null,
   constraint PK_PERMISSION primary key (PermissionId)
)
go

/*==============================================================*/
/* Index: UX_PERMISSION                                         */
/*==============================================================*/
create unique index UX_PERMISSION on ach.Permission (
   Name ASC
)
go

/*==============================================================*/
/* Table: PermissionRole                                        */
/*==============================================================*/
create table ach.PermissionRole (
   PermissionRoleId     int                  identity,
   RoleId               int                  not null,
   PermissionId         int                  not null,
   constraint PK_PERMISSIONROLE primary key (PermissionRoleId)
)
go

/*==============================================================*/
/* Index: UX_PERMISSIONROLE                                     */
/*==============================================================*/
create unique index UX_PERMISSIONROLE on ach.PermissionRole (
   RoleId ASC,
   PermissionId ASC
)
go

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
go

/*==============================================================*/
/* Index: UX_ROLE                                               */
/*==============================================================*/
create unique index UX_ROLE on ach.Role (
   Name ASC
)
go

/*==============================================================*/
/* Table: "User"                                                */
/*==============================================================*/
create table ach."User" (
   UserId               int                  identity,
   RoleId               int                  not null,
   Name                 nvarchar(255)        not null,
   Email                nvarchar(255)        null,
   Created              datetime             not null,
   Modified             datetime             null,
   constraint PK_USER primary key (UserId)
)
go

/*==============================================================*/
/* Table: UserPasswordCredential                                */
/*==============================================================*/
create table ach.UserPasswordCredential (
   UserPasswordCredentialId int                  identity,
   UserId               int                  not null,
   Login                nvarchar(255)        not null,
   PasswordHash         nvarchar(32)         not null,
   PasswordSalt         nvarchar(32)         not null,
   Created              datetime             not null,
   Modified             datetime             null,
   constraint PK_USERPASSWORDCREDENTIAL primary key (UserPasswordCredentialId)
)
go

/*==============================================================*/
/* Index: UX_LOGIN                                              */
/*==============================================================*/
create unique index UX_LOGIN on ach.UserPasswordCredential (
   Login ASC
)
go

/*==============================================================*/
/* Index: UX_USER                                               */
/*==============================================================*/
create unique index UX_USER on ach.UserPasswordCredential (
   UserId ASC
)
go

alter table ach.PartnerUser
   add constraint FK_PARTNERUSER_USER foreign key (UserId)
      references ach."User" (UserId)
go

alter table ach.PartnerUser
   add constraint FK_PARTNERUSER_PARTNER foreign key (PartnerId)
      references ach.Partner (PartnerId)
go

alter table ach.PermissionRole
   add constraint FK_PERMISSIONROLE_ROLE foreign key (RoleId)
      references ach.Role (RoleId)
go

alter table ach.PermissionRole
   add constraint FK_PERMISSIONROLE_PERMISSION foreign key (PermissionId)
      references ach.Permission (PermissionId)
go

alter table ach."User"
   add constraint FK_USER_ROLE foreign key (RoleId)
      references ach.Role (RoleId)
go

alter table ach.UserPasswordCredential
   add constraint FK_USERPASSWORDCREDENTI_USER foreign key (UserId)
      references ach."User" (UserId)
go

