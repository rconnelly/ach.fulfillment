namespace Ach.Fulfillment.Common.Unity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class UnitOfWorkLifetimeStore
    {
        protected abstract bool Enabled { get; set; }

        protected abstract Dictionary<Guid, object> Values { get; set; }

        public object GetValue(Guid key)
        {
            object result;
            this.EnsureValues();
            this.Values.TryGetValue(key, out result);
            return result;
        }

        public void RemoveValue(Guid key)
        {
            this.EnsureValues();
            this.Values.Remove(key);
        }

        public void SetValue(Guid key, object newValue)
        {
            if (!this.Enabled)
            {
                throw new InvalidOperationException("UnitOfWork not started.");
            }

            this.EnsureValues();

            var prevValue = this.GetValue(key);
            if (prevValue != null)
            {
                var disposable = prevValue as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            this.Values[key] = newValue;
        }

        internal void Clear()
        {
            if (this.Values != null)
            {
                foreach (var value in this.Values.Values.OfType<IDisposable>())
                {
                    value.Dispose();
                }

                this.Values.Clear();
            }
        }

        internal void Disable()
        {
            if (!this.Enabled)
            {
                throw new InvalidOperationException("Already disabled!");
            }

            this.Enabled = false;
        }

        internal void Enable()
        {
            if (this.Enabled)
            {
                throw new InvalidOperationException("Already enabled!");
            }

            this.Enabled = true;
        }

        private void EnsureValues()
        {
            if (this.Values == null)
            {
                this.Values = new Dictionary<Guid, object>();
            }
        }
    }
}