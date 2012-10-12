namespace Ach.Fulfillment.Scheduler.Configuration
{
    using System.Collections.Specialized;
    using System.Configuration;

    internal class Registry
    {
        private const string PrefixServerConfiguration = "quartz.server";
        private const string KeyServiceName = PrefixServerConfiguration + ".serviceName";
        private const string KeyServiceDisplayName = PrefixServerConfiguration + ".serviceDisplayName";
        private const string KeyServiceDescription = PrefixServerConfiguration + ".serviceDescription";

        private const string DefaultServiceName = "AchFulfillmentScheduler";
        private const string DefaultServiceDisplayName = "Ach Fulfillment Scheduler";
        private const string DefaultServiceDescription = "Ach Fulfillment Scheduling Server";

        private static readonly NameValueCollection Configuration;

        static Registry()
        {
            Configuration = (NameValueCollection) ConfigurationManager.GetSection("quartz");
        }

        public static string ServiceName
        {
            get { return GetConfigurationOrDefault(KeyServiceName, DefaultServiceName); }
        }

        public static string ServiceDisplayName
        {
            get { return GetConfigurationOrDefault(KeyServiceDisplayName, DefaultServiceDisplayName); }
        }

        public static string ServiceDescription
        {
            get { return GetConfigurationOrDefault(KeyServiceDescription, DefaultServiceDescription); }
        }

        private static string GetConfigurationOrDefault(string configurationKey, string defaultValue)
        {
            string retValue = null;
            if (Configuration != null)
            {
                retValue = Configuration[configurationKey];
            }

            if (retValue == null || retValue.Trim().Length == 0)
            {
                retValue = defaultValue;
            }

            return retValue;
        }
    }
}
