namespace Ach.Fulfillment.Business.Impl
{
    using System;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    using Microsoft.Practices.Unity;

    internal class ApplicationEventRaiseManager : IApplicationEventRaiseManager
    {
        #region Properties

        [Dependency]
        public IQueue Queue { get; set; }

        #endregion

        #region Public Methods

        public void RaiseAchTransactionCreatedNotification(AchTransactionEntity instance)
        {
            this.EnqueueLocalNotification(instance);
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Methods

        private void EnqueueLocalNotification(AchTransactionEntity instance)
        {
            Contract.Assert(instance != null);
            Contract.Assert(instance.Partner != null);
            var referenceEntity = new AchTransactionReferenceEntity { Id = instance.Id, PartnerId = instance.Partner.Id };
            var actionData = new EnqueueAchTransactionCreatedNotification { Instance = referenceEntity };
            this.Queue.Enqueue(actionData);
        }

        private void EnqueueLocalNotification(AchFileEntity instance)
        {
            Contract.Assert(instance != null);
            var referenceEntity = new ReferenceEntity { Id = instance.Id };
            var actionData = new EnqueueAchFileStatusChangedNotification
                                 {
                                     Instance = referenceEntity,
                                     FileStatus = instance.FileStatus
                                 };
            this.Queue.Enqueue(actionData);
        }

        private void EnqueueRemoteNotification(AchFileEntity instance)
        {
            Contract.Assert(instance != null);
            var referenceEntity = new ReferenceEntity { Id = instance.Id };
            var actionData = new EnqueueCallbackNotification { Instance = referenceEntity };
            this.Queue.Enqueue(actionData);
        }

        #endregion
    }
}