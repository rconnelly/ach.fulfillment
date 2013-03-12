namespace Ach.Fulfillment.Business.Impl.Strategies.Enumerators
{
    using System.Collections;
    using System.Collections.Generic;

    public abstract class BaseEnumerator<T> : IEnumerator<T>, IEnumerable<T>
    {
        #region Public Properties

        public T Current { get; protected set; }

        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        #endregion

        #region Public Methods and Operators

        public abstract bool MoveNext();

        public virtual void Reset()
        {
        }

        public virtual void Dispose()
        {
        }

        public IEnumerator<T> GetEnumerator()
        {
            this.Reset();
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}