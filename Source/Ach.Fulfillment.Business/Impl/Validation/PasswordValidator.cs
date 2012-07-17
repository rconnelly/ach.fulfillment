namespace Ach.Fulfillment.Business.Impl.Validation
{
    using Ach.Fulfillment.Data;

    using FluentValidation;

    internal class PasswordValidator : AbstractValidator<string>
    {
        public PasswordValidator()
        {
            this.RuleFor(i => i)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, MetadataInfo.StringNormal)
                .WithName("Password");
        }
    }
}