namespace Ach.Fulfillment.Shared.Reflection
{
    using System;
    using System.Reflection;

    public abstract class ReflectMember<T> where T : Attribute
    {
        protected ReflectMember(T reflectAttribute, MemberInfo memberInfo)
        {
            this.ReflectAttribute = reflectAttribute;
            this.MemberInfo = memberInfo;
        }

        public T ReflectAttribute { get; private set; }

        public abstract Type Type { get; }

        public string MemberName
        {
            get
            {
                return this.MemberInfo.Name;
            }
        }

        protected MemberInfo MemberInfo { get; set; }

        public abstract void SetValue(object instance, object value);

        public abstract object GetValue(object instance);
    }
}
