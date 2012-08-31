using System;
using System.Reflection;

namespace Ach.Fulfillment.Shared.Reflection
{
    internal class ReflectProperty<T> : ReflectMember<T> where T : Attribute
    {
        public PropertyInfo PropertyInfo { get { return base._memberInfo as PropertyInfo; } }
        
        public ReflectProperty(T reflectAttribute, MemberInfo memberInfo ) : 
            base(reflectAttribute, memberInfo) { }

		public override Type Type
		{
			get { return PropertyInfo.PropertyType; }
		}

        public override void SetValue(object instance, object value)
        {
            PropertyInfo.SetValue(instance, value, null);
        }

        public override object GetValue(object instance)
        {
            return PropertyInfo.GetValue(instance, null);
        }
    }
}
