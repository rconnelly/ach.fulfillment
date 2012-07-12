namespace Ach.Fulfillment.Service
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public class WorkerRole : RoleEntryPoint
    {
        #region Public Methods and Operators

        public override bool OnStart()
        {
            var config = DiagnosticMonitor.GetDefaultInitialConfiguration();

           var tmp = new DiagnosticMonitorTraceListener();

           Trace.Listeners.Add(tmp);

           config.Logs.BufferQuotaInMB = 200;

           config.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1.0);

           DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", config);

           Trace.Write("Test");

           return base.OnStart();
        }

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("Ach.Fulfillment.Service entry point called", "Information");

            while (true)
            {
                Thread.Sleep(10000);
                Trace.WriteLine("Working", "Information");
            }
        }

        #endregion
    }
}