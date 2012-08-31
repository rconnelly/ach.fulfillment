using System;
using System.Reflection;

namespace Ach.Fulfillment.Shared.Reflection
{
    internal class ReflectField<T> : ReflectMember<T> where T : Attribute
    {
        public FieldInfo FieldInfo { get { return base._memberInfo as FieldInfo; } }

        public ReflectField(T reflectAttribute, MemberInfo memberInfo) : 
            base(reflectAttribute, memberInfo) { }

		public override Type Type
		{
			get { return FieldInfo.FieldType; }
		}

        public override void SetValue(object instance, object value)
        {
            FieldInfo.SetValue(instance, value);
        }

        public override object GetValue(object instance)
        {
            return FieldInfo.GetValue(instance);
        }
    }
}
