USE [Ach]
GO

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

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[DefaultSendQueueAutoend]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [ach].[DefaultSendQueueAutoend]
GO
CREATE PROCEDURE [ach].[DefaultSendQueueAutoend]
AS 
BEGIN
	DECLARE @h UNIQUEIDENTIFIER;
	WHILE(1=1)BEGIN
		BEGIN TRANSACTION
		WAITFOR(
			RECEIVE TOP (1) 
				@h = [conversation_handle] 
			FROM [ach].[DefaultSendQueue]), TIMEOUT 5000
		IF(@@rowcount > 0)
		BEGIN
			END CONVERSATION @h;
		END
		COMMIT TRANSACTION;
	END
END
GO

CREATE QUEUE [ach].[DefaultSendQueue]
   WITH
     STATUS = ON,
     RETENTION = OFF,
     POISON_MESSAGE_HANDLING (STATUS = OFF),
     ACTIVATION (
        PROCEDURE_NAME = [ach].[DefaultSendQueueAutoend],
        MAX_QUEUE_READERS = 1,
        EXECUTE AS SELF ) 
GO

CREATE QUEUE [ach].[CreatedAchTransactionQueue]
   WITH
     STATUS = ON,
     RETENTION = OFF, 
     POISON_MESSAGE_HANDLING (STATUS = OFF)
GO

CREATE QUEUE [ach].[CreatedAchFileQueue]
   WITH
     STATUS = ON,
     RETENTION = OFF, 
     POISON_MESSAGE_HANDLING (STATUS = OFF)
GO

CREATE QUEUE [ach].[GeneratedAchFileQueue]
   WITH
     STATUS = ON,
     RETENTION = OFF, 
     POISON_MESSAGE_HANDLING (STATUS = OFF)
GO

CREATE QUEUE [ach].[UploadedAchFileQueue]
   WITH
     STATUS = ON,
     RETENTION = ON,
     POISON_MESSAGE_HANDLING (STATUS = OFF)
GO

CREATE QUEUE [ach].[CallbackNotificationQueue]
   WITH
     STATUS = ON,
     RETENTION = ON, 
     POISON_MESSAGE_HANDLING (STATUS = OFF)
GO

CREATE SERVICE [DefaultSendService]
	ON QUEUE [ach].[DefaultSendQueue]
GO

CREATE SERVICE [FireAchTransactionCreated]
	ON QUEUE [ach].[CreatedAchTransactionQueue] ([DEFAULT])
GO

CREATE SERVICE [FireAchFileCreated]
	ON QUEUE [ach].[CreatedAchFileQueue] ([DEFAULT])
GO

CREATE SERVICE [FireAchFileGenerated]
	ON QUEUE [ach].[GeneratedAchFileQueue] ([DEFAULT])
GO

CREATE SERVICE [FireAchFileUploaded]
	ON QUEUE [ach].[UploadedAchFileQueue] ([DEFAULT])
GO

CREATE SERVICE [EnqueueCallbackNotification]
	ON QUEUE [ach].[CallbackNotificationQueue] ([DEFAULT])
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[SendReference]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [ach].[SendReference]
GO
CREATE PROCEDURE [ach].[SendReference]
 @Content NVARCHAR(MAX),
 @DestinationService nvarchar(256)
AS 
BEGIN
	DECLARE @DialogHandle UNIQUEIDENTIFIER;
	SET NOCOUNT ON
 
    BEGIN TRANSACTION;
    
	BEGIN TRY
		BEGIN DIALOG CONVERSATION @DialogHandle
			FROM SERVICE [DefaultSendService]
			TO SERVICE @DestinationService
			ON CONTRACT [DEFAULT]
			WITH ENCRYPTION = OFF;

		SEND ON CONVERSATION @DialogHandle
			MESSAGE TYPE [DEFAULT] (@Content);

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN
		DECLARE @em nvarchar(4000);
		DECLARE @es int;
		SELECT @em = ERROR_MESSAGE(), @es = ERROR_SEVERITY();
		RAISERROR (@em, @es, 1)
	END CATCH
END
GO


-- EXEC sp_configure filestream_access_level, 2
-- GO
-- RECONFIGURE
-- GO

-- CREATE DATABASE Ach ON PRIMARY
--     (NAME = Ach_data, FILENAME = N'C:\Ach\ach.data.mdf'),
-- FILEGROUP NachaFileGroup CONTAINS FILESTREAM
--     (NAME = NachaFiles, FILENAME = N'C:\Ach\fsdc\Nacha')
-- LOG ON 
--     (NAME = Ach_log, FILENAME = N'C:\Ach\ach.log.ldf');
-- GO 
	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ach].[AchFileContent]') AND type in (N'U'))
	DROP TABLE [ach].[AchFileContent]
GO

CREATE TABLE [ach].[AchFileContent](
	[Id] uniqueidentifier ROWGUIDCOL constraint DF_ID DEFAULT (newid()) NOT NULL,
    [AchFileId] int NOT NULL,
    [SystemFile] varbinary(max) FILESTREAM NULL,
    CONSTRAINT [PK_AchFileContent] PRIMARY KEY NONCLUSTERED 
	(
		[Id] ASC
	)
) 
GO

create unique clustered index UX_AchFileId on [ach].[AchFileContent] (
   [AchFileId] ASC
)
GO

alter table ach.[AchFileContent]
   add constraint FK_ACHFILECONTENT_ACHFILE foreign key ([AchFileId])
      references ach.[AchFile] ([AchFileId])
		on delete cascade
GO

-- ALTER DATABASE Ach SET NEW_BROKER WITH ROLLBACK IMMEDIATE;

-- delete from [ach].[AchFileContent]
--select * from [ach].[AchFile]
--select * from [ach].[AchFileContent]

--declare @id int;
--set @id = 2
--insert into [ach].[AchFileContent]([AchFileId], [SystemFile]) values(@id, cast('' as varbinary(max)));
--select SystemFile.PathName() from [ach].[AchFileContent] where [AchFileId] = 2

--delete from [ach].[AchFileContent]
--where AchFileId = 2

if exists (select * from sys.objects where object_id = OBJECT_ID(N'[ach].[ReceiveCallbackNotificationReference]') and type in (N'P', N'PC'))
	drop procedure [ach].[ReceiveCallbackNotificationReference]
go

create procedure [ach].[ReceiveCallbackNotificationReference]
as
begin
	set nocount on
	declare @conversation_handle uniqueidentifier;
	declare @message_type_name nvarchar(max);
	declare @message_body varbinary(max);
	declare @retry_count int;
	
	receive top(1) 
		@conversation_handle = conversation_handle,
		@message_type_name = message_type_name,
		@message_body = message_body,
		@retry_count = 1
	from [ach].[CallbackNotificationQueue];
	
	if(@message_type_name = 'http://schemas.microsoft.com/SQL/ServiceBroker/DialogTimer')
	begin
		select top(1)
			@message_body = message_body
		from [ach].[CallbackNotificationQueue]
		where 
			conversation_handle = @conversation_handle 
			and message_type_name = 'DEFAULT';

		select 
			@retry_count = count(*) 
		from [ach].[CallbackNotificationQueue]
		where 
			conversation_handle = @conversation_handle;
	end
	
	select 
		convert(nvarchar(max), @message_body) as Content, 
		@conversation_handle as conversation_handle, 
		@retry_count as retry_count 
	where @conversation_handle is not null;
	
end
go


/*
DECLARE @DialogHandle UNIQUEIDENTIFIER;
DECLARE @Content nvarchar(max);
set @Content = 'bbb'
BEGIN DIALOG CONVERSATION @DialogHandle
			FROM SERVICE DefaultSendService
			TO SERVICE 'EnqueueCallbackNotification'
			ON CONTRACT [DEFAULT]
			WITH ENCRYPTION = OFF;

SEND ON CONVERSATION @DialogHandle
	MESSAGE TYPE [DEFAULT] (@Content);
*/


-- exec [ach].[ReceiveAchTransactionCallbackNotificationReference]
-- exec [ach].[CompleteAchTransactionCallbackNotification] '75C9F82E-1C89-E211-8A7F-70568189F040'
-- exec [ach].[RescheduleAchTransactionCallbackNotification] '75C9F82E-1C89-E211-8A7F-70568189F040', 2
-- select CONVERT(NVARCHAR(MAX), message_body), * from [ach].[CallbackNotificationQueue]




/*
DECLARE @conversation_handle UNIQUEIDENTIFIER;
receive top(1) @conversation_handle = conversation_handle from [ach].[CallbackNotificationQueue]
BEGIN CONVERSATION TIMER (@conversation_handle) timeout = 5
-- end conversation 'AAE9B215-0D89-E211-8A7F-70568189F040' @conversation_handle;

*/

alter table ach.AchTransaction
	add NotifiedTransactionStatus int constraint DF_NotifiedTransactionStatus DEFAULT (0) not null
	
	