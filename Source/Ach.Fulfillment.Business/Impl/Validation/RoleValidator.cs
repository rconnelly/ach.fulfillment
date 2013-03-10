namespace Ach.Fulfillment.Business.Impl.Validation
{
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;
    using Ach.Fulfillment.Data.Specifications.Roles;
    using Ach.Fulfillment.Persistence;

    using FluentValidation;

    internal class RoleValidator : AbstractValidator<RoleEntity>
    {
        public RoleValidator(IRepository repository)
        {
            this.RuleFor(i => i.Name)
                .NotEmpty()
                .Length(1, MetadataInfo.StringNormal)
                .Uniqness(repository, new RoleUniqness());
        }
    }
}