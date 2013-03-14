namespace Ach.Fulfillment.Business.Impl.FileTransmission
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    using Ach.Fulfillment.Business.Properties;
    using Ach.Fulfillment.Data;

    internal class FileSystemRemoteAccessManager : IRemoteAccessManager
    {
        public void Upload(string name, Stream stream)
        {
            Contract.Assert(!string.IsNullOrEmpty(name));
            Contract.Assert(stream != null);

            CheckDirectory();

            var filename = Path.Combine(Settings.Default.FsLocalPath, name + ".ach");
            using (var fs = File.OpenWrite(filename))
            {
                stream.CopyTo(fs);
            }
        }

        public AchFileStatus GetStatus(string name)
        {
            Contract.Assert(!string.IsNullOrEmpty(name));

            CheckDirectory();

            var result = AchFileStatus.None;
            var filename = Path.Combine(Settings.Default.FsLocalPath, name + ".ach.response");
            if (File.Exists(filename))
            {
                var content = File.ReadAllText(filename);
                if (string.Equals("A", content, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = AchFileStatus.Accepted;
                }
                else if (string.Equals("R", content, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = AchFileStatus.Rejected;
                }
                else
                {
                    throw new InvalidDataException("Invalid file content: " + content);
                }
            }

            return result;
        }

        private static void CheckDirectory()
        {
            if (!Directory.Exists(Settings.Default.FsLocalPath))
            {
                Directory.CreateDirectory(Settings.Default.FsLocalPath);
            }
        }
    }
}
