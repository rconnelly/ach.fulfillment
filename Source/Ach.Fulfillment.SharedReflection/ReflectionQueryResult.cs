namespace Ach.Fulfillment.Shared.Reflection
{
    using System;
    using System.Collections.Generic;

    public class ReflectionQueryResult<T> where T : Attribute
    {
        public ReflectionQueryResult(Type type, Dictionary<string, ReflectMember<T>> reflectableMembers)
        {
            this.SystemType = type;
            this.ReflectableMembers = reflectableMembers;
        }

        public Dictionary<string, ReflectMember<T>> ReflectableMembers { get; private set; }

        public Type SystemType { get; private set; }
    }
}
