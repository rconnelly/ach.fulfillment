namespace Ach.Fulfillment.Business.Impl.Validation
{
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.Users;
    using Ach.Fulfillment.Persistence;

    using FluentValidation;

    internal class UserPasswordCredentialValidator : AbstractValidator<UserPasswordCredentialEntity>
    {
        public UserPasswordCredentialValidator(IRepository repository)
        {
            this.RuleFor(i => i.Login).NotEmpty().Length(1, MetadataInfo.StringNormal).Uniqness(repository, new UserPasswordCredentialUniqness());
            this.RuleFor(i => i.PasswordHash).NotEmpty().Length(1, 100);
            this.RuleFor(i => i.PasswordSalt).NotEmpty().Length(1, 100);
            this.RuleFor(i => i.User).NotNull();
        }
    }
}