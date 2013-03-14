namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    using Ach.Fulfillment.Business.Properties;
    using Ach.Fulfillment.Data;

    internal static class RemoterAccessManagerExtension
    {
        public static IRemoteAccessManager Build(this Func<string, IRemoteAccessManager> builder, AchFileEntity achFile)
        {
            Contract.Assert(builder != null);
            Contract.Assert(achFile != null);
            Contract.Assert(achFile.Partner != null);
            var key = Settings.Default.RemoteAccessManagerType;
            var manager = builder(key);
            return manager;
        }

        public static void Upload(this Func<string, IRemoteAccessManager> builder, AchFileEntity achFile, Stream stream)
        {
            var manager = builder.Build(achFile);
            manager.Upload(achFile.Name, stream);
        }

        public static AchFileStatus GetStatus(this Func<string, IRemoteAccessManager> builder, AchFileEntity achFile)
        {
            var manager = builder.Build(achFile);
            var result = manager.GetStatus(achFile.Name);
            return result;
        }
    }
}