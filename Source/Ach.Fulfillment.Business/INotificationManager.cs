namespace Ach.Fulfillment.Business
{
    using System;
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface INotificationManager
    {
        void RaiseAchTransactionCreatedNotification(AchTransactionEntity instance);

        void RaiseAchFileStatusChangedNotification(AchFileEntity instance);

        bool DeliverRemoteNotifications();

        IEnumerable<AchTransactionReferenceEntity> GetNextReadyToBeGroupedAchTransactionReferences(int limit);

        bool TryGetNextReadyToUploadAchFile(out AchFileEntity instance);

        bool TryGetNextReadyToGenerateAchFile(out AchFileEntity instance);
    }
}