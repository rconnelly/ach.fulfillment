namespace Ach.Fulfillment.Business.Impl.Validation
{
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Persistence;

    using FluentValidation;

    internal class AchTransactionValidator : AbstractValidator<AchTransactionEntity>
    {
        public AchTransactionValidator(IRepository repository)
        {
            this.Repository = repository;
            this.RuleFor(i => i.Amount).NotNull();
            this.RuleFor(i => i.CallbackUrl).Length(1, 255);
            this.RuleFor(i => i.DfiAccountId).NotEmpty().NotNull().Length(17)
                .SetValidator(new FluentValidation.Validators.RegularExpressionValidator(@"^[a-zA-Z0-9]*$"));
            this.RuleFor(i => i.EntryClassCode).NotEmpty().NotNull().Length(3)
                .SetValidator(new FluentValidation.Validators.RegularExpressionValidator(@"^[A-Z]*$"));
            this.RuleFor(i => i.EntryDate).NotEmpty().NotNull();
            this.RuleFor(i => i.EntryDescription).NotEmpty().NotNull().Length(1, 10)
                .SetValidator(new FluentValidation.Validators.RegularExpressionValidator(@"^[a-zA-Z0-9]*$"));
            this.RuleFor(i => i.IndividualIdNumber).NotEmpty().NotNull().Length(15)
                .SetValidator(new FluentValidation.Validators.RegularExpressionValidator(@"^[a-zA-Z0-9]*$"));
            this.RuleFor(i => i.PaymentRelatedInfo).Length(0, 94)
                .SetValidator(new FluentValidation.Validators.RegularExpressionValidator(@"^[a-zA-Z0-9]*$"));
            this.RuleFor(i => i.ReceiverName).NotEmpty().NotNull().Length(1, 22)
                .SetValidator(new FluentValidation.Validators.RegularExpressionValidator(@"^[a-zA-Z0-9]*$"));
            this.RuleFor(i => i.ServiceClassCode).Length(3)
                .SetValidator(new FluentValidation.Validators.RegularExpressionValidator(@"^(200|220|225)$"));
            this.RuleFor(i => i.TransactionCode).NotEmpty().NotNull().Length(2)
                .SetValidator(new FluentValidation.Validators.RegularExpressionValidator(@"^(22|23|24|27|28|29|32|33|34|37|38|39)$"));
            this.RuleFor(i => i.TransitRoutingNumber).NotEmpty().NotNull().Length(9)
                .SetValidator(new FluentValidation.Validators.RegularExpressionValidator(@"^[0-9]+$"));

            this.RuleFor(i => i.TransactionStatus).NotNull();
            this.RuleFor(i => i.Partner).NotNull();
        }

        public IRepository Repository { get; set; }
    }
}
