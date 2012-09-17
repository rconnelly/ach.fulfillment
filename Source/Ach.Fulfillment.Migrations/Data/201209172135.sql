
/*==============================================================*/
/* Table: "AchTransaction"                                                */
/*==============================================================*/
create table ach."AchTransaction" (
   AchTransactionId     int                  identity,
   UserId               int                  not null,
   MerchantId			nvarchar(10)         not null,
   MerchantName         nvarchar(16)         not null,
   MerchantDescription  nvarchar(10)         not null,
   CallbackUrl			nvarchar(255)        not null,
   Amount				nvarchar(10)		 not null,
   AccountId			nvarchar(15)         not null,
   RoutingNumber		nvarchar(9)          not null,
   IsQueued				bit					 not null default 0,
   Created              datetime             not null,
   Modified             datetime             null,
   constraint PK_ACHTRANSACTION primary key (AchTransactionId)
)
GO

alter table ach.AchTransaction
   add constraint FK_ACHTRANSACTION_USER foreign key (UserId)
      references ach."User" (UserId)
GO