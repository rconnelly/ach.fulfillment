namespace Ach.Fulfillment.Business
{
    using System;
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;

    using Renci.SshNet;

    public interface IAchFileManager
    {
        void Generate();

        void Upload(PasswordConnectionInfo connectionInfo);

        void Cleanup();

        /*List<AchFileEntity> AchFilesToUpload(bool lockRecords = true);*/
    }
}
