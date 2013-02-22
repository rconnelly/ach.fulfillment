namespace Ach.Fulfillment.Common.Unity
{
    using System;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    public class UnitOfWorkLifetimeManager : LifetimeManager
    {
        #region Constants and Fields

        private static Lazy<UnitOfWorkLifetimeStore> storeBuilder = new Lazy<UnitOfWorkLifetimeStore>(() => ServiceLocator.Current.GetInstance<UnitOfWorkLifetimeStore>());

        private readonly Guid key = Guid.NewGuid();

        #endregion

        #region Properties

        public static UnitOfWorkLifetimeStore Store
        {
            get
            {
                return storeBuilder.Value;
            }

            set
            {
                var newBuilder = value != null 
                    ? new Lazy<UnitOfWorkLifetimeStore>(() => value)
                    : new Lazy<UnitOfWorkLifetimeStore>(() => ServiceLocator.Current.GetInstance<UnitOfWorkLifetimeStore>());

                if (storeBuilder.IsValueCreated)
                {
                    storeBuilder.Value.Disable();
                    storeBuilder.Value.Clear();
                }

                storeBuilder = newBuilder;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override object GetValue()
        {
            var result = Store.GetValue(this.key);
            return result;
        }

        public override void RemoveValue()
        {
            Store.RemoveValue(this.key);
        }

        public override void SetValue(object newValue)
        {
            Store.SetValue(this.key, newValue);
        }

        #endregion
    }
}