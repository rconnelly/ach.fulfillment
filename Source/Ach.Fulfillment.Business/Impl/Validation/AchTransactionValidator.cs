namespace Ach.Fulfillment.Business.Impl.Validation
{
    using System;
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    using FluentValidation;

    internal class AchTransactionValidator : AbstractValidator<AchTransactionEntity>
    {
        private const string UrlRegex =
    @"^(http|https|ftp)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&amp;%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{2}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&amp;%\$#\=~_\-]+))*$";

        public AchTransactionValidator()
        {
            // IndividualIdNumber
            this.RuleFor(i => i.IndividualIdNumber)
                .Length(15)
                .AlphaNumeric();

            // ReceiverName
            this.RuleFor(i => i.ReceiverName)
                .NotEmpty()
                .Length(1, 22)
                .AlphaNumeric();

            // EntryDescription
            this.RuleFor(i => i.EntryDescription)
                .NotEmpty()
                .Length(1, 10)
                .AlphaNumeric();

            // EntryDate
            this.RuleFor(i => i.EntryDate)
                .NotEmpty();

            // TransactionCode
            var validTransactionCodes = new List<int> { 22, 23, 24, 27, 28, 29, 32, 33, 34, 37, 38, 39 };
            this.RuleFor(i => i.TransactionCode)
                .Must(validTransactionCodes.Contains)
                .GreaterThan(0)
                .WithMessage("Valid codes for {PropertyName} are 22, 23, 24, 27, 28, 29, 32, 33, 34, 37, 38, 39");

            // TransitRoutingNumber
            this.RuleFor(i => i.TransitRoutingNumber)
                .NotEmpty()
                .Length(9)
                .Matches(@"^[0-9]+$").WithMessage("{PropertyName} must have numeric data");

            // DfiAccountId
            this.RuleFor(i => i.DfiAccountId)
                .NotEmpty()
                .Length(17)
                .AlphaNumeric();

            // Amount
            this.RuleFor(i => i.Amount)
                .NotEmpty()
                .Must(v =>
                    {
                        v = Math.Abs(v);
                        var decimalPart = v - Math.Floor(v); // 100.354 - 100 = 0.354
                        var cents = decimalPart * 100; // 35.4
                        var centDecimalPart = cents - Math.Floor(cents); // 35.4 - 35 = 0.4
                        return centDecimalPart == 0; // 0.4 compare to 0
                    }).WithMessage("{PropertyName} must be amount of dollars with two decimal places after ',' ");

            // ServiceClassCode
            var validServiceClassCodes = new List<int> { 200, 220, 225 };
            this.RuleFor(i => i.ServiceClassCode)
                .Must(validServiceClassCodes.Contains)
                .WithMessage("Valid codes for {PropertyName} are 200, 220, 225");

            // EntryClassCode
            this.RuleFor(i => i.EntryClassCode)
                .NotEmpty()
                .Matches(@"^(CCD|PPD)$").WithMessage("Valid codes for {PropertyName} are CCD, PPD");

            // PaymentRelatedInfo
            this.RuleFor(i => i.PaymentRelatedInfo)
                .Length(0, 80);

            // CallbackUrl
            this.RuleFor(i => i.CallbackUrl)
                .NotEmpty()
                .Length(1, 2000)
                .Matches(UrlRegex).WithMessage("{PropertyName} format is wrong");

            // Partner
            this.RuleFor(i => i.Partner)
                .NotNull();
        }
    }
}
