namespace Ach.Fulfillment.Api
{
    using System;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FulfillmentService" in code, svc and config file together.
    public class FulfillmentService : IFulfillmentService
    {
        #region Public Methods and Operators

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }

            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }

            return composite;
        }

        #endregion
    }
}