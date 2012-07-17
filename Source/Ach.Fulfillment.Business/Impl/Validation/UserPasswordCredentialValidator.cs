namespace Ach.Fulfillment.Business.Impl.Validation
{
    using Ach.Fulfillment.Data;

    using FluentValidation;

    internal class UserPasswordCredentialValidator : AbstractValidator<UserPasswordCredentialEntity>
    {
        public UserPasswordCredentialValidator()
        {
            this.RuleFor(i => i.Login).NotEmpty().Length(1, MetadataInfo.StringNormal);
            this.RuleFor(i => i.PasswordHash).NotEmpty().Length(1, 100);
            this.RuleFor(i => i.PasswordSalt).NotEmpty().Length(1, 100);
            this.RuleFor(i => i.User).NotNull();
        }
    }
}