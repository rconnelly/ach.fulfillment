﻿namespace Ach.Fulfillment.Api
{
    using System.Runtime.Serialization;
    using System.ServiceModel;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFulfillmentService" in both code and config file together.
    [ServiceContract]
    public interface IFulfillmentService
    {
        #region Public Methods and Operators

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        #endregion

        // TODO: Add your service operations here
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        #region Constants and Fields

        private bool boolValue = true;

        private string stringValue = "Hello ";

        #endregion

        #region Public Properties

        [DataMember]
        public bool BoolValue
        {
            get
            {
                return this.boolValue;
            }

            set
            {
                this.boolValue = value;
            }
        }

        [DataMember]
        public string StringValue
        {
            get
            {
                return this.stringValue;
            }

            set
            {
                this.stringValue = value;
            }
        }

        #endregion
    }
}