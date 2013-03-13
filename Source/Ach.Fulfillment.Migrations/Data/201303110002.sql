/*==============================================================*/
/* Queues                                                       */
/*==============================================================*/
if exists (select * from sys.objects where object_id = OBJECT_ID(N'ach.DefaultSendQueueAutoend') and type in (N'P', N'PC'))
    drop procedure ach.DefaultSendQueueAutoend
GO

create procedure ach.DefaultSendQueueAutoend
as
begin
    declare @h uniqueidentifier;
    while(1=1)begin
        begin transaction
        waitfor(
            receive top (1)
                @h = conversation_handle
            from ach.DefaultSendQueue), timeout 5000
        if(@@rowcount > 0)
        begin
            end conversation @h;
        end
        commit transaction;
    end
end
GO

create queue ach.DefaultSendQueue
   with
     STATUS = on,
     RETENTION = off,
     POISON_MESSAGE_HANDLING (STATUS = off),
     ACTIVATION (
        PROCEDURE_NAME = ach.DefaultSendQueueAutoend,
        MAX_QUEUE_READERS = 1,
        execute as SELF)
GO

create queue ach.CreatedAchTransactionQueue
   with
     STATUS = on,
     RETENTION = off,
     POISON_MESSAGE_HANDLING (STATUS = off)
GO

create queue ach.CreatedAchFileQueue
   with
     STATUS = on,
     RETENTION = on,
     POISON_MESSAGE_HANDLING (STATUS = off)
GO

create queue ach.GeneratedAchFileQueue
   with
     STATUS = on,
     RETENTION = on,
     POISON_MESSAGE_HANDLING (STATUS = off)
GO

create queue ach.UploadedAchFileQueue
   with
     STATUS = on,
     RETENTION = on,
     POISON_MESSAGE_HANDLING (STATUS = off)
GO

create queue ach.CallbackNotificationQueue
   with
     STATUS = on,
     RETENTION = on,
     POISON_MESSAGE_HANDLING (STATUS = off)
GO

/*==============================================================*/
/* Services                                                     */
/*==============================================================*/
create service DefaultSendService
    on queue ach.DefaultSendQueue
GO

create service FireAchTransactionCreated
    on queue ach.CreatedAchTransactionQueue ([DEFAULT])
GO

create service FireAchFileCreated
    on queue ach.CreatedAchFileQueue ([DEFAULT])
GO

create service FireAchFileGenerated
    on queue ach.GeneratedAchFileQueue ([DEFAULT])
GO

create service FireAchFileUploaded
    on queue ach.UploadedAchFileQueue ([DEFAULT])
GO

create service EnqueueCallbackNotification
    on queue ach.CallbackNotificationQueue ([DEFAULT])
GO



/*==============================================================*/
/* Procedure: SendReference                                     */
/*==============================================================*/
if exists (select * from sys.objects where object_id = OBJECT_ID(N'ach.SendReference') and type in (N'P', N'PC'))
    drop procedure ach.SendReference
GO

create procedure ach.SendReference
    @Content nvarchar(max),
    @DestinationService nvarchar(256)
as
begin
    declare @DialogHandle uniqueidentifier;
    set NOCOUNT on
    begin transaction;
    begin try
        begin dialog conversation @DialogHandle
            from service DefaultSendService
            to service @DestinationService
            on CONTRACT [DEFAULT]
            WITH ENCRYPTION = OFF;
        send on conversation @DialogHandle
            message type [DEFAULT] (@Content);
        commit transaction;
    end try
    begin catch
        if @@TRANCOUNT > 0
            rollback transaction
        declare @em nvarchar(4000);
        declare @es int;
        select @em = ERROR_MESSAGE(), @es = ERROR_SEVERITY();
        raiserror (@em, @es, 1);
    end catch
end
GO
