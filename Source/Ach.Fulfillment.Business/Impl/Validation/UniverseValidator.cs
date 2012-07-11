namespace Ach.Fulfillment.Business.Impl.Validation
{
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;
    using Ach.Fulfillment.Persistence;

    using FluentValidation;

    internal class UniverseValidator : AbstractValidator<UniverseEntity>
    {
        public UniverseValidator(IRepository repository)
        {
            this.RuleFor(i => i.Login).NotEmpty().Length(1, MetadataInfo.StringNormal)
                .Uniqness(repository, new UniverseUniqness());
            this.RuleFor(i => i.PasswordHash).NotEmpty().Length(1, MetadataInfo.StringNormal);
            this.RuleFor(i => i.Deleted).Equals(false);
        }
    }
}
