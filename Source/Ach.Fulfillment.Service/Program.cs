namespace Ach.Fulfillment.Service
{
    using System.ServiceProcess;

    internal static class Program
    {
        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            var servicesToRun = new ServiceBase[] { new FulfillmentService() };
            ServiceBase.Run(servicesToRun);
        }

        #endregion
    }
}