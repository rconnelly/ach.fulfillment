namespace Ach.Fulfillment.Data.Common
{
    public class DeleteCompletedFilesActionData : DeleteByQueryActionData
    {
        public override string Query
        {
            get
            {
                return "from AchFileEntity o where o.FileStatus = " + (int)AchFileStatus.Completed;
            }          
        }
    }
}
