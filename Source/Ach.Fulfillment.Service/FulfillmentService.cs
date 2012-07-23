namespace Ach.Fulfillment.Service
{
    using System.ServiceProcess;

    public partial class FulfillmentService : ServiceBase
    {
        #region Constructors and Destructors

        public FulfillmentService()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        #endregion
    }
}