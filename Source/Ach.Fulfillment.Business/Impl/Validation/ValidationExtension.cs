namespace Ach.Fulfillment.Business.Impl.Validation
{
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
                throw new BusinessValidationException(result.Errors);
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

            // TODO: use state
            /*validator.CustomStateProvider = t => new ValidationFailureState(ViolationCodeNames.NonUniqueState);*/
            return ruleBuilder;
        }
    }
}