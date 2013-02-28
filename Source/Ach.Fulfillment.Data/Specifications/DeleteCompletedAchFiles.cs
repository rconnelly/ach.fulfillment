namespace Ach.Fulfillment.Data.Specifications
{
    using Ach.Fulfillment.Data.Common;

    public class DeleteCompletedAchFiles : IDeleteByQueryActionData
    {
        public string Query
        {
            get
            {
                return "from AchFileEntity o where o.FileStatus = " + (int)AchFileStatus.Completed;
            }
        }
    }
}
