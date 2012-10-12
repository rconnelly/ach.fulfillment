namespace Ach.Fulfillment.Scheduler.Configuration
{
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.ServiceProcess;

    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        #region Constants and Fields

        private ServiceInstaller serviceInstaller;

        private ServiceProcessInstaller serviceProcessInstaller;

        #endregion

        #region Constructors and Destructors

        public ProjectInstaller()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.serviceProcessInstaller != null)
                {
                    this.serviceProcessInstaller.Dispose();
                    this.serviceProcessInstaller = null;
                }

                if (this.serviceInstaller != null)
                {
                    this.serviceInstaller.Dispose();
                    this.serviceInstaller = null;
                }
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.serviceInstaller = new ServiceInstaller();
            this.serviceProcessInstaller = new ServiceProcessInstaller { Account = ServiceAccount.LocalService };

            this.serviceInstaller.DisplayName = Registry.ServiceDisplayName;
            this.serviceInstaller.ServiceName = Registry.ServiceName;
            this.serviceInstaller.Description = Registry.ServiceDescription;

            this.Installers.AddRange(new Installer[]
                {
                    this.serviceProcessInstaller, 
                    this.serviceInstaller
                });
        }

        #endregion
    }
}