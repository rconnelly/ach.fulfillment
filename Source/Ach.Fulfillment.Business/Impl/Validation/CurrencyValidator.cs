namespace Ach.Fulfillment.Business.Impl.Validation
{
    using System;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;
    using Ach.Fulfillment.Persistence;

    using FluentValidation;

    [Obsolete]
    internal class CurrencyValidator : AbstractValidator<CurrencyEntity>
    {
        public CurrencyValidator(IRepository repository)
        {
            this.RuleFor(i => i.Name).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().Length(1, MetadataInfo.StringNormal)
                .Uniqness(repository, new CurrencyUniqness());
            this.RuleFor(i => i.Description).Length(0, MetadataInfo.StringLong);
            this.When(
                i => !string.IsNullOrEmpty(i.CurrencyCode),
                () => this.RuleFor(i => i.CurrencyCode).Length(0, MetadataInfo.StringTiny).Matches("^[a-zA-Z0-9]*$"));
            this.RuleFor(i => i.Universe).NotNull();
        }
    }
}