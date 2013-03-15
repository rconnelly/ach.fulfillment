namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Business.Impl.Configuration;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchTransactions;
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    using ServiceStack.Text;

    internal class CallbackNotificationProcessor : BaseAchFileRetryingProcessor<ReadyToBeProcessedCallbackNotificationReference>
    {
        #region Constants

        private const string CallbackNotificationContentType = "application/json";

        private readonly Dictionary<long, RetryReferenceEntity> alreadyRescheduled = new Dictionary<long, RetryReferenceEntity>();

        #endregion

        #region Constructors

        public CallbackNotificationProcessor(IQueue queue, IRepository repository)
            : base(queue, repository, MetadataInfo.RepeatIntervalForCallbackNotification)
        {
        }

        #endregion

        #region Methods

        #region Simple failing notification duplicate protaction

        public override void Execute()
        {
            this.alreadyRescheduled.Clear();
            base.Execute();
        }

        protected override bool ShouldBeRescheduled(RetryReferenceEntity reference)
        {
            var should = this.AllowedToNotify(reference) && base.ShouldBeRescheduled(reference);
            return should;
        }

        protected override void Reschedule(RetryReferenceEntity reference)
        {
            base.Reschedule(reference);
            if (!this.alreadyRescheduled.ContainsKey(reference.Id))
            {
                this.alreadyRescheduled.Add(reference.Id, reference);
            }
        }

        protected bool AllowedToNotify(RetryReferenceEntity reference)
        {
            var allowed = !this.alreadyRescheduled.ContainsKey(reference.Id)
                          || this.alreadyRescheduled[reference.Id].Handle == reference.Handle;
            return allowed;
        }

        protected override bool ProcessCore(RetryReferenceEntity reference)
        {
            var result = true;
            if (this.AllowedToNotify(reference))
            {
                result = base.ProcessCore(reference);
            }
            else
            {
                this.Logger.DebugFormat(
                    CultureInfo.InvariantCulture, 
                    "Skipping duplicated notification for ach file {0}", 
                    reference.Id);
            }

            return result;
        }

        #endregion

        protected override bool ProcessCore(RetryReferenceEntity reference, AchFileEntity achFile)
        {
            Contract.Assert(achFile != null);

            var nothingToNotify = true;
            var transactionEntities = this.Repository.FindAll(new UntrackingUnnotifiedAchTransactionByAchFileId { AchFileId = achFile.Id });

            // do not use Any method here to not touch database
            foreach (var transactionEntity in transactionEntities)
            {
                nothingToNotify = false;
                this.PerformCallbackNotification(transactionEntity);
            }

            if (nothingToNotify)
            {
                this.Logger.DebugFormat(CultureInfo.InvariantCulture, "Here nothing to notify in the '{0}'", achFile);
            }

            return true;
        }

        private void PerformCallbackNotification(AchTransactionEntity transactionEntity)
        {
            var previousNotifiedStatus = transactionEntity.NotifiedStatus;
            var actionData = new ActualizeAchTransactionNotificationStatus
            {
                Id = transactionEntity.Id,
                Status = transactionEntity.Status,
                NotifiedStatus = transactionEntity.Status
            };
            this.Repository.Execute(actionData);
            if (actionData.Updated)
            {
                try
                {
                    this.Post(transactionEntity);
                }
                catch (BusinessException ex)
                {
                    actionData.NotifiedStatus = previousNotifiedStatus;
                    actionData.Updated = false;
                    this.Repository.Execute(actionData);
                    if (!actionData.Updated)
                    {
                        this.Logger.FatalFormat(
                            CultureInfo.InvariantCulture, "Post callback compensation error. It should never happen.");
                        throw new InvalidOperationException("Unable to compensate callback notification error.", ex);
                    }

                    throw;
                }
            }
        }

        private void Post(AchTransactionEntity transactionEntity)
        {
            this.Logger.DebugFormat(CultureInfo.InvariantCulture, "Trying to send callback notification for '{0}'", transactionEntity);
            Uri uri;
            if (!string.IsNullOrEmpty(transactionEntity.CallbackUrl)
                && Uri.TryCreate(transactionEntity.CallbackUrl, UriKind.Absolute, out uri))
            {
                var payload = this.GenerateCallbackPayload(transactionEntity);
                this.Post(uri, payload);
                this.Logger.DebugFormat(CultureInfo.InvariantCulture, "Callback notification successfully send for '{0}'", transactionEntity);
            }
            else
            {
                this.Logger.WarnFormat(
                    CultureInfo.InvariantCulture,
                    "Invalid callback uri '{0}' provided by '{1}'",
                    transactionEntity.CallbackUrl,
                    transactionEntity);
            }
        }

        private string GenerateCallbackPayload(AchTransactionEntity transactionEntity)
        {
            var payload = new CallbackNotificationPayload
            {
                TransactionId = transactionEntity.Id,
                IndividualIdNumber = transactionEntity.IndividualIdNumber,
                Status = transactionEntity.Status,
            };
            var result = JsonSerializer.SerializeToString(payload);
            return result;
        }

        private void Post(Uri uri, string payload)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue(CallbackNotificationContentType));
                    using (var content = new StringContent(payload, Encoding.UTF8, CallbackNotificationContentType))
                    {
                        using (var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = content })
                        {
                            var response = client.SendAsync(request);
                            response.Result.EnsureSuccessStatusCode();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var rethrow = ExceptionPolicy.HandleException(ex, BusinessContainerExtension.OperationCallbackNotificationPolicy);
                if (rethrow)
                {
                    throw;
                }
            }
        }

        #endregion

        #region Nested types
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private class CallbackNotificationPayload
        {
            #region Public Properties

            public long TransactionId { get; set; }

            public string IndividualIdNumber { get; set; }

            public AchTransactionStatus Status { get; set; }

            #endregion
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        #endregion
    }
}