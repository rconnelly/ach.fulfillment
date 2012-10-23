namespace Ach.Fulfillment.Nacha.Attribute
{
    using System;

    using Ach.Fulfillment.Shared.Reflection;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class RecordAttribute : Attribute, IReflectAttribute
    {
        public bool IsRequired { get; set; }

        public string RecordType { get; set; }

        public string ControlPrefix { get; set; }

        public string Prefix { get; set; }

        public string Postfix { get; set; }

        #region IReflectAttribute Members

        public int Position { get; set; }

        #endregion
    }
}
