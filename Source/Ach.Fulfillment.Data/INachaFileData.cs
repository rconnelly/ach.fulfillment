namespace Ach.Fulfillment.Data
{
    public interface INachaFileData
    {
        string Destination { get; }

        string FileIdModifier { get; }

        string ImmediateDestination { get; }

        string CompanyIdentification { get; }

        string OriginOrCompanyName { get; }

        string ReferenceCode { get; }

        string CompanyName { get; }

        string DfiIdentification { get; }
    }
}