/*==============================================================*/
/* Table: "Webhook"                                       */
/*==============================================================*/
create table ach."Webhook" (
   WebhookId		 int                  identity,
   Url				 nvarchar(256)        null,
   Limit			 int				  not null default 20,
   Header			 nvarchar(256)        null,
   Body				 nvarchar(256)        null,
   LastSent          datetime             null,
   Created           datetime             not null,
   Modified          datetime             null,
   constraint PK_WEBHOOK primary key (WebhookId)
)
GO
