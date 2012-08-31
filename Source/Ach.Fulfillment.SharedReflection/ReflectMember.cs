using System;
using System.Reflection;

namespace Ach.Fulfillment.Shared.Reflection
{
    public abstract class ReflectMember<T> where T : Attribute
    {
        public T ReflectAttribute { get; private set; }
        protected MemberInfo _memberInfo;

        public ReflectMember(T reflectAttribute, MemberInfo memberInfo)
        {
            ReflectAttribute = reflectAttribute;
            _memberInfo = memberInfo;
        }

		public abstract Type Type { get; }

        public abstract void SetValue(object instance, object value);
        public abstract object GetValue(object instance);

		public string MemberName { get { return _memberInfo.Name; } }
    }
}
