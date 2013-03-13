/*==============================================================*/
/* Tables                                                       */
/*==============================================================*/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[Webhook]') AND type in (N'U'))
    DROP TABLE [ach].[Webhook]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[AchFileTransaction]') AND type in (N'U'))
    DROP TABLE [ach].[AchFileTransaction]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[AchTransaction]') AND type in (N'U'))
    DROP TABLE [ach].[AchTransaction]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[AchFileContent]') AND type in (N'U'))
    DROP TABLE [ach].[AchFileContent]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[AchFile]') AND type in (N'U'))
    DROP TABLE [ach].[AchFile]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[Permission]') AND type in (N'U'))
    DROP TABLE [ach].[Permission]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[PartnerUser]') AND type in (N'U'))
    DROP TABLE [ach].[PartnerUser]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[PartnerDetail]') AND type in (N'U'))
    DROP TABLE [ach].[PartnerDetail]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[Partner]') AND type in (N'U'))
    DROP TABLE [ach].[Partner]
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[UserPasswordCredential]') AND type in (N'U'))
    DROP TABLE [ach].[UserPasswordCredential]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[User]') AND type in (N'U'))
    DROP TABLE [ach].[User]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[Role]') AND type in (N'U'))
    DROP TABLE [ach].[Role]
GO


/*==============================================================*/
/* Services                                                     */
/*==============================================================*/

IF  EXISTS (SELECT * FROM sys.services WHERE name = N'DefaultSendService')
    DROP SERVICE [DefaultSendService]
GO

IF  EXISTS (SELECT * FROM sys.services WHERE name = N'FireAchTransactionCreated')
    DROP SERVICE [FireAchTransactionCreated]
GO

IF  EXISTS (SELECT * FROM sys.services WHERE name = N'FireAchFileCreated')
    DROP SERVICE [FireAchFileCreated]
GO

IF  EXISTS (SELECT * FROM sys.services WHERE name = N'FireAchFileGenerated')
    DROP SERVICE [FireAchFileGenerated]
GO

IF  EXISTS (SELECT * FROM sys.services WHERE name = N'FireAchFileUploaded')
    DROP SERVICE [FireAchFileUploaded]
GO

IF  EXISTS (SELECT * FROM sys.services WHERE name = N'EnqueueCallbackNotification')
    DROP SERVICE [EnqueueCallbackNotification]
GO

IF  EXISTS (SELECT * FROM sys.service_queues WHERE name = N'DefaultSendQueue')
    DROP QUEUE [ach].[DefaultSendQueue]
GO

IF  EXISTS (SELECT * FROM sys.service_queues WHERE name = N'CreatedAchTransactionQueue')
    DROP QUEUE [ach].[CreatedAchTransactionQueue]
GO

IF  EXISTS (SELECT * FROM sys.service_queues WHERE name = N'CreatedAchFileQueue')
    DROP QUEUE [ach].[CreatedAchFileQueue]
GO

IF  EXISTS (SELECT * FROM sys.service_queues WHERE name = N'GeneratedAchFileQueue')
    DROP QUEUE [ach].[GeneratedAchFileQueue]
GO

IF  EXISTS (SELECT * FROM sys.service_queues WHERE name = N'UploadedAchFileQueue')
    DROP QUEUE [ach].[UploadedAchFileQueue]
GO

IF  EXISTS (SELECT * FROM sys.service_queues WHERE name = N'CallbackNotificationQueue')
    DROP QUEUE [ach].[CallbackNotificationQueue]
GO

/*==============================================================*/
/* Procedures                                                   */
/*==============================================================*/
if exists (select * from sys.objects where object_id = OBJECT_ID(N'ach.DefaultSendQueueAutoend') and type in (N'P', N'PC'))
    drop procedure ach.DefaultSendQueueAutoend
GO

if exists (select * from sys.objects where object_id = OBJECT_ID(N'ach.SendReference') and type in (N'P', N'PC'))
    drop procedure ach.SendReference
GO


/*==============================================================*/
/* Schema                                                       */
/*==============================================================*/
IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'ach')
    DROP SCHEMA [ach]
GO
