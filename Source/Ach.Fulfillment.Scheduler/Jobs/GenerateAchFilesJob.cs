namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System;
    using System.IO;

    using Ach.Fulfillment.Scheduler.Common;
    using Business;
    using Fulfillment.Common;

    using Microsoft.Practices.Unity;

    using Quartz;

    [DisallowConcurrentExecution]
    public class GenerateAchFilesJob : IJob
    {
        #region Public Properties

        [Dependency]
        public IAchTransactionManager Manager { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (new UnitOfWork())
                {
                    var dataMap = context.JobDetail.JobDataMap;
                    var achfilesStore = dataMap.GetString("AchFilesStore");
                    this.Manager.Generate(achfilesStore);

                    var ftphost = dataMap.GetString("FtpHost");
                    var ftpfilepath = dataMap.GetString("FtpFilePath");
                    var ftpfullpath = "ftp://" + ftphost + ftpfilepath;

                    var userId = dataMap.GetString("UserId");
                    var password = dataMap.GetString("Password");

                    if (!(string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(password)))
                    {
                        var directory = new DirectoryInfo(achfilesStore);
                        var files = directory.GetFiles("*.ach");
                        var ftpUploader = new FtpHelpers();

                        foreach (var file in files)
                        {
                            var destinationPath = ftpfullpath + "/" + file.Name;
                            var sourcePath = achfilesStore + "/" + file.Name;

                            // this.Uploadfiles(destinationPath, sourcePath, userId, password);
                            ftpUploader.AsynchronousUpload(destinationPath, sourcePath, userId, password);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new JobExecutionException(ex);
            }
        }

        #endregion

        #region Private Methods

/*
        private void Uploadfiles(string ftpfullpath, string sourceDirectory, string userId, string password)
        {
            try
                {
                    var ftpRequest = (FtpWebRequest)WebRequest.Create(ftpfullpath);
                    ftpRequest.Timeout = 10000;
                    ftpRequest.ReadWriteTimeout = 10000;
                    ftpRequest.KeepAlive = true;
                    ftpRequest.UseBinary = true;
                    ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                    ftpRequest.Credentials = new NetworkCredential(userId, password);

                    var fileStream = File.OpenRead(sourceDirectory);
                    var fileContents = new byte[fileStream.Length];
                    fileStream.Read(fileContents, 0, fileContents.Length);
                    fileStream.Close();

                    ftpRequest.ContentLength = fileContents.Length;

                    var ftpstream = ftpRequest.GetRequestStream();
                    ftpstream.Write(fileContents, 0, fileContents.Length);
                    ftpstream.Close();

                   var response = (FtpWebResponse)ftpRequest.GetResponse();
                   response.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("\r\n- Problem uploading file. url = " + sourceDirectory + ", Msg = " + ex.Message + ", " + ex.StackTrace);
                }
            }
*/
        }

        #endregion
    }