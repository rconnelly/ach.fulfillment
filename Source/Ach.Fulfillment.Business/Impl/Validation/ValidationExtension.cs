namespace Ach.Fulfillment.Business.Impl.Validation
{
    using System.Linq;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Persistence;

    using FluentValidation;

    internal static class ValidationExtension
    {
        public static void DemandValid<T>(this IValidator validator, T instance)
        {
            var result = validator.Validate(instance);
            if (!result.IsValid)
            {
                var failures = from error in result.Errors
                               where error != null
                               select
                                   new ValidationFailureInfo(
                                       error.PropertyName,
                                       error.ErrorMessage,
                                       error.CustomState as string,
                                       error.AttemptedValue,
                                       error.CustomState);
                throw new BusinessValidationException(failures.ToList());
            }
        }

        public static IRuleBuilderOptions<T, TProperty> Uniqness<T, TProperty, TBase>(
                this IRuleBuilderOptions<T, TProperty> ruleBuilder,
                IRepository repository,
                IInstanceQueryData<TBase> queryData)
            where T : class, IEntity
            where TBase : class, IEntity
        {
            var validator = new UniquePropertyValidator<TBase>(repository, queryData);
            ruleBuilder.SetValidator(validator);
            return ruleBuilder;
        }

        public static IRuleBuilderOptions<T, string> AlphaNumeric<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var result = ruleBuilder.Matches(@"^[a-zA-Z0-9 ]*$").WithMessage("{PropertyName} must have alphanumeric data");
            return result;
        }
    }
}