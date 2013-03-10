namespace Ach.Fulfillment.Data
{
    public static class AchFileStatusExtension
    {
        public static AchTransactionStatus? ToAchTransactionStatus(this AchFileStatus status)
        {
            AchTransactionStatus? childStatus = null;
            switch (status)
            {
                case AchFileStatus.Created:
                    childStatus = AchTransactionStatus.Grouped;
                    break;
                case AchFileStatus.Generated:
                    childStatus = AchTransactionStatus.Batched;
                    break;
                case AchFileStatus.Uploaded:
                    childStatus = AchTransactionStatus.Sent;
                    break;
                case AchFileStatus.Rejected:
                    childStatus = AchTransactionStatus.Rejected;
                    break;
                case AchFileStatus.Accepted:
                    childStatus = AchTransactionStatus.Accepted;
                    break;
            }

            return childStatus;
        }
    }
}