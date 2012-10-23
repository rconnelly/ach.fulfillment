namespace Ach.Fulfillment.Shared.Reflection
{
    using System;
    using System.Reflection;

    internal class ReflectField<T> : ReflectMember<T> where T : Attribute
    {
        public ReflectField(T reflectAttribute, MemberInfo memberInfo)
            : base(reflectAttribute, memberInfo)
        {
        }

        public FieldInfo FieldInfo
        {
            get
            {
                return this.MemberInfo as FieldInfo;
            }
        }

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
