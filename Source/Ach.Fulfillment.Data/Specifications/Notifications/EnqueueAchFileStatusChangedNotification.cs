namespace Ach.Fulfillment.Data.Specifications.Notifications
{
    public class EnqueueAchFileStatusChangedNotification : CommonEnqueueData<ReferenceEntity>
    {
        public AchFileStatus FileStatus { get; set; }
    }
}