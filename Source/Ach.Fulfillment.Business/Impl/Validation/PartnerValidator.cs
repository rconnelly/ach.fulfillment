namespace Ach.Fulfillment.Business.Impl.Validation
{
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;
    using Ach.Fulfillment.Persistence;

    using FluentValidation;

    internal class PartnerValidator : AbstractValidator<PartnerEntity>
    {
        public PartnerValidator(IRepository repository)
        {
            this.RuleFor(i => i.Name).NotEmpty().Length(1, MetadataInfo.StringNormal)
                .Uniqness(repository, new PartnerUniqness());
        }
    }
}