namespace Ach.Fulfillment.Business
{
    using System.IO;

    using Ach.Fulfillment.Data;

    public interface IRemoteAccessManager
    {
        void Upload(string name, Stream stream);

        AchFileStatus GetStatus(string name);
    }
}