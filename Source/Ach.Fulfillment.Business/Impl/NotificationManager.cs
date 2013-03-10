namespace Ach.Fulfillment.Business.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    using global::Common.Logging;

    using Microsoft.Practices.Unity;

    internal class NotificationManager : INotificationManager
    {
        #region Fields

        private const int DefaultCallbackNotificationRepeatDelay = 5;

        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        [Dependency]
        public IRepository Repository { get; set; }

        #endregion

        #region Public Methods

        public void RaiseAchTransactionCreatedNotification(AchTransactionEntity instance)
        {
            Contract.Assert(instance != null);
            Contract.Assert(instance.Partner != null);
            var referenceEntity = new AchTransactionReferenceEntity { Id = instance.Id, PartnerId = instance.Partner.Id };
            var actionData = new EnqueueAchTransactionCreatedNotification { Instance = referenceEntity };
            this.Repository.Execute(actionData);
        }

        public void RaiseAchFileStatusChangedNotification(AchFileEntity instance)
        {
            Contract.Assert(instance != null);

            switch (instance.FileStatus)
            {
                case AchFileStatus.Created:
                    this.EnqueueLocalNotification(instance);
                    this.EnqueueRemoteNotification(instance);
                    break;
                case AchFileStatus.Generated:
                    this.EnqueueLocalNotification(instance);
                    this.EnqueueRemoteNotification(instance);
                    break;
                case AchFileStatus.Uploaded:
                    this.EnqueueLocalNotification(instance);
                    this.EnqueueRemoteNotification(instance);
                    break;
                case AchFileStatus.Accepted:
                    this.EnqueueRemoteNotification(instance);
                    break;
                case AchFileStatus.Rejected:
                    this.EnqueueRemoteNotification(instance);
                    break;
                case AchFileStatus.Finalized:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IEnumerable<AchTransactionReferenceEntity> GetNextReadyToBeGroupedAchTransactionReferences(int limit)
        {
            var count = 0;
            do
            {
                var current = this.Repository.FindOne(new ReadyToBeGroupedAchTransactionReference());
                if (current == null)
                {
                    yield break;
                }

                count++;
                yield return current;
            }
            while (count < limit);
        }

        public bool TryGetNextReadyToGenerateAchFile(out AchFileEntity instance)
        {
            var result = this.TryGetNextAchFile<ReadyToGenerateAchFileReference>("generate", out instance);
            return result;
        }

        public bool TryGetNextReadyToUploadAchFile(out AchFileEntity instance)
        {
            var result = this.TryGetNextAchFile<ReadyToUploadAchFileReference>("upload", out instance);
            return result;
        }

        public bool DeliverRemoteNotifications()
        {
            var processed = this.Repository
                .ExecuteWithRetry<ReadyToBeProcessedCallbackNotificationReference>(
                this.PerformCallbackNotifications, TimeSpan.FromSeconds(DefaultCallbackNotificationRepeatDelay));

            return processed;
        }

        #endregion

        #region Methods

        private void EnqueueLocalNotification(AchFileEntity instance)
        {
            Contract.Assert(instance != null);
            var referenceEntity = new ReferenceEntity { Id = instance.Id };
            var actionData = new EnqueueAchFileStatusChangedNotification
                                 {
                                     Instance = referenceEntity,
                                     FileStatus = instance.FileStatus
                                 };
            this.Repository.Execute(actionData);
        }

        private void EnqueueRemoteNotification(AchFileEntity instance)
        {
            Contract.Assert(instance != null);
            var referenceEntity = new ReferenceEntity { Id = instance.Id };
            var actionData = new EnqueueCallbackNotification { Instance = referenceEntity };
            this.Repository.Execute(actionData);
        }

        private bool TryGetNextAchFile<TQuery>(
            string operation, out AchFileEntity instance)
            where TQuery : IQueryData<ReferenceEntity>, new()
        {
            ReferenceEntity reference;
            var result = TryGetNextAchFile<TQuery, ReferenceEntity>(operation, out reference, out instance);
            return result;
        }    

        private bool TryGetNextAchFile<TQuery, TReference>(string operation, out TReference reference, out AchFileEntity instance)
            where TQuery : IQueryData<TReference>, new()
            where TReference : ReferenceEntity
        {
            var fetched = false;
            instance = null;
            reference = this.Repository.FindOne(new TQuery());
            if (reference != null)
            {
                fetched = true;
                instance = this.Repository.Get<AchFileEntity>(reference.Id);
                if (instance == null)
                {
                    Logger.WarnFormat(CultureInfo.InvariantCulture, "Cannot find ready for {0} ach file '{1}'", operation, reference.Id);
                }
            }

            return fetched;
        }

        private void PerformCallbackNotifications(AchFileEntity achFile)
        {
            Contract.Assert(achFile != null);

            //todo: implement it
            return;
            throw new NotImplementedException();
        }

        #endregion
    }
}