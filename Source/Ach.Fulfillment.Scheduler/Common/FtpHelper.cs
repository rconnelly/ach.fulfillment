namespace Ach.Fulfillment.Scheduler.Common
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;

    public class FtpHelpers
    {
        public void AsynchronousUpload(string destinationPath, string sourcePath, string userName, string password)
        {
            var target = new Uri(destinationPath);
            var state = new FtpState();
            var ftpRequest = (FtpWebRequest)WebRequest.Create(target);
            ftpRequest.Proxy = null;
            ftpRequest.Timeout = 10000;
            ftpRequest.ReadWriteTimeout = 10000;
            ftpRequest.KeepAlive = true;
            ftpRequest.UseBinary = true;
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

            if (!(string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password)))
            {
                ftpRequest.Credentials = new NetworkCredential(userName, password);
            }

            state.Request = ftpRequest;
            state.FileName = sourcePath;

            var waitObject = state.OperationComplete;
            ftpRequest.BeginGetRequestStream(EndGetStreamCallback, state);
            waitObject.WaitOne();

            if (state.OperationException != null)
            {
                throw state.OperationException;
            }
        }

        private static void EndGetStreamCallback(IAsyncResult ar)
        {
            var state = (FtpState)ar.AsyncState;

            try
            {
                var requestStream = state.Request.EndGetRequestStream(ar);

                var fileStream = File.OpenRead(state.FileName);
                var fileContents = new byte[fileStream.Length];
                fileStream.Read(fileContents, 0, fileContents.Length);
                fileStream.Close();
               
                requestStream.Close();
                state.Request.ContentLength = fileContents.Length;
                state.Request.BeginGetResponse(EndGetResponseCallback, state);
            }
            catch (Exception e)
            {
                state.OperationException = e;
                state.OperationComplete.Set();
            }
        }

        private static void EndGetResponseCallback(IAsyncResult ar)
        {
            var state = (FtpState)ar.AsyncState;
            try
            {
                var response = (FtpWebResponse)state.Request.EndGetResponse(ar);
                response.Close();
                state.StatusDescription = response.StatusDescription;

                // Signal the main application thread that the operation is complete.
                state.OperationComplete.Set();
            }
            catch (Exception e)
            {
                state.OperationException = e;
                state.OperationComplete.Set();
            }
        }

        public class FtpState
        {
            private readonly ManualResetEvent wait;

            public FtpState()
            {
                this.wait = new ManualResetEvent(false);
            }

            public ManualResetEvent OperationComplete
            {
                get
                {
                    return this.wait;
                }
            }

            public FtpWebRequest Request { get; set; }

            public string FileName { get; set; }

            public Exception OperationException { get; set; }

            public string StatusDescription { get; set; }
        }
    }
}
