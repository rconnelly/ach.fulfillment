namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System.IO;
    using System.Linq;

    using Ach.Fulfillment.Scheduler.Common;

    using Business;

    using Microsoft.Practices.Unity;

    using Quartz;

    using global::Common.Logging;

    [DisallowConcurrentExecution]
    public class GenerateAchFilesJob : BaseJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CheckStatusFilesJob));

        #region Public Properties

        [Dependency]
        public IAchFileManager Manager { get; set; }

        #endregion

        #region Public Methods and Operators

        protected override void InternalExecute(IJobExecutionContext context)
        {
            Logger.Info("GenerateAchFilesJob started...");

            var dataMap = context.JobDetail.JobDataMap;
            var achfilesStore = dataMap.GetString("AchFilesStore");

            this.Manager.Generate();

            var achFiles = this.Manager.GetAchFilesDataForUploading();

            // todo to be deleted
            foreach (var achfile in achFiles)
            {
                var newPath = Path.Combine(achfilesStore, achfile.Key.Name + ".ach");
                using (var file = new StreamWriter(newPath))
                {
                    file.Write(achfile.Value);
                    file.Flush();
                    file.Close();
                }
            }

            Logger.Info("GenerateAchFilesJob finished...");
        }

        #endregion
    }
}