namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchTransactions;
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    using ServiceStack.Text;

    public class CallbackNotificationProcessor : BaseAchFileRetryingProcessor<ReadyToBeProcessedCallbackNotificationReference>
    {
        #region Constants

        private const int DefaultCallbackNotificationRepeatDelay = 30;

        private const string CallbackNotificationContentType = "application/json";

        #endregion

        #region Constructors

        public CallbackNotificationProcessor(IQueue queue, IRepository repository)
            : base(queue, repository, TimeSpan.FromSeconds(DefaultCallbackNotificationRepeatDelay))
        {
        }

        #endregion

        #region Methods

        protected override void ProcessCore(RetryReferenceEntity reference, AchFileEntity achFile)
        {
            Contract.Assert(achFile != null);

            var transactionEntities = this.Repository.FindAll(new UnnotifiedAchTransactions { AchFileId = achFile.Id });
            foreach (var transactionEntity in transactionEntities)
            {
                this.PerformCallbackNotification(achFile, transactionEntity);
            }
        }

        private void PerformCallbackNotification(AchFileEntity achFile, AchTransactionEntity transactionEntity)
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
                    this.Post(achFile, transactionEntity);
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

        private void Post(AchFileEntity achFile, AchTransactionEntity transactionEntity)
        {
            Uri uri;
            if (!string.IsNullOrEmpty(transactionEntity.CallbackUrl)
                && Uri.TryCreate(transactionEntity.CallbackUrl, UriKind.Absolute, out uri))
            {
                var payload = this.GenerateCallbackPayload(achFile, transactionEntity);

                this.Post(uri, payload);
            }
            else
            {
                this.Logger.WarnFormat(
                    CultureInfo.InvariantCulture,
                    "Invalid callback uri '{0}' provided by ach transaction '{1}'",
                    transactionEntity.CallbackUrl,
                    transactionEntity.Id);
            }
        }

        private string GenerateCallbackPayload(AchFileEntity achFile, AchTransactionEntity transactionEntity)
        {
            string extra = null;
            if (achFile.FileStatus == AchFileStatus.Rejected)
            {
                // todo: fill Extra field with error message in case of Failed state
                extra = "error";
            }

            var payload = new CallbackNotificationPayload
            {
                TransactionId = transactionEntity.Id,
                Status = transactionEntity.Status,
                Extra = extra
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
            catch (Exception)
            {
                // todo: use ehab here to wrap necessary exceptions into BusinessException
                throw;
            }
        }

        #endregion

        #region Nested types
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private class CallbackNotificationPayload
        {
            #region Public Properties

            public string Extra { get; set; }

            public AchTransactionStatus Status { get; set; }

            public long TransactionId { get; set; }

            #endregion
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        #endregion
    }
}