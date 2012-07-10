/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2005                    */
/* Created on:     2012.07.10 19:14:27                          */
/*==============================================================*/


alter table ach.Currency
   drop constraint FK_CURRENCY_UNIVERSE
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.Currency')
            and   name  = 'IX_UNIQUENAME'
            and   indid > 0
            and   indid < 255)
   drop index ach.Currency.IX_UNIQUENAME
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('ach.Universe')
            and   name  = 'IX_UNIQUELOGIN'
            and   indid > 0
            and   indid < 255)
   drop index ach.Universe.IX_UNIQUELOGIN
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ach.Currency')
            and   type = 'U')
   drop table ach.Currency
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ach.Universe')
            and   type = 'U')
   drop table ach.Universe
go

drop schema ach
go

/*==============================================================*/
/* User: ach                                                    */
/*==============================================================*/
create schema ach authorization dbo
go

/*==============================================================*/
/* Table: Currency                                              */
/*==============================================================*/
create table ach.Currency (
   CurrencyId           int                  identity,
   UniverseId           int                  not null,
   Name                 nvarchar(255)        not null,
   Description          nvarchar(4000)       null,
   CurrencyCode         char(3)              null,
   Hidden               bit                  not null constraint DF_HIDDEN_CURRENCY default 0,
   Position             float                not null constraint DF_POSITION_CURRENCY default 0,
   Created              datetime             not null constraint DF_CREATED_CURRENCY default getdate(),
   Modified             datetime             null,
   constraint PK_CURRENCY primary key (CurrencyId)
)
go

/*==============================================================*/
/* Index: IX_UniqueName                                         */
/*==============================================================*/
create unique index IX_UNIQUENAME on ach.Currency (
UniverseId ASC,
Name ASC
)
go

/*==============================================================*/
/* Table: Universe                                              */
/*==============================================================*/
create table ach.Universe (
   UniverseId           int                  identity,
   Login                nvarchar(255)        not null,
   PasswordHash         nvarchar(255)        not null,
   Created              datetime             not null constraint DF_CREATED_UNIVERSE default getdate(),
   Modified             datetime             null,
   Deleted              bit                  not null constraint DF_DELETED_UNIVERSE default 0,
   constraint PK_UNIVERSE primary key (UniverseId)
)
go

/*==============================================================*/
/* Index: IX_UniqueLogin                                        */
/*==============================================================*/
create unique index IX_UNIQUELOGIN on ach.Universe (
Login ASC
)
go

alter table ach.Currency
   add constraint FK_CURRENCY_UNIVERSE foreign key (UniverseId)
      references ach.Universe (UniverseId)
go

