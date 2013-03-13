namespace Ach.Fulfillment.Data
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    public class AchFileEntity : BaseEntity, INachaFileData
    {
        public virtual PartnerEntity Partner { get; set; }

        /// <summary>
        /// AchFileEntity can contain a lot of AchTransactionEntities, so it is preferable to not load Transactions that way
        /// </summary>
        public virtual IList<AchTransactionEntity> Transactions { get; set; }

        public virtual string Name { get; set; }

        public virtual string FileIdModifier { get; set; }

        public virtual AchFileStatus FileStatus { get; set; }

        string INachaFileData.Destination
        {
            get
            {
                this.CheckPartnerDetails();
                return this.Partner.Details.Destination;
            }
        }

        string INachaFileData.ImmediateDestination
        {
            get
            {
                this.CheckPartnerDetails();
                return this.Partner.Details.ImmediateDestination;
            }
        }

        string INachaFileData.CompanyIdentification
        {
            get
            {
                this.CheckPartnerDetails();
                return this.Partner.Details.CompanyIdentification;
            }
        }

        string INachaFileData.OriginOrCompanyName
        {
            get
            {
                this.CheckPartnerDetails();
                return this.Partner.Details.OriginOrCompanyName;
            }
        }

        string INachaFileData.ReferenceCode
        {
            get
            {
                this.CheckPartnerDetails();
                return this.Id.ToString(CultureInfo.InvariantCulture);
            }
        }

        string INachaFileData.CompanyName
        {
            get
            {
                this.CheckPartnerDetails();
                return this.Partner.Details.CompanyName;
            }
        }

        string INachaFileData.DfiIdentification
        {
            get
            {
                this.CheckPartnerDetails();
                return this.Partner.Details.DfiIdentification;
            }
        }

        private void CheckPartnerDetails()
        {
            Contract.Assert(this.Partner != null);
            Contract.Assert(this.Partner.Details != null);
        }
    }
}
