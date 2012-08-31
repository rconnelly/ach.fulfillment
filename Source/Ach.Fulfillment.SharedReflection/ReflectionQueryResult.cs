using System;
using System.Collections.Generic;

namespace Ach.Fulfillment.Shared.Reflection
{
    public class ReflectionQueryResult<T> where T : Attribute
    {
        public Dictionary<string,ReflectMember<T>> ReflectableMembers { get; private set; }

        public Type SystemType { get; private set; }

        public ReflectionQueryResult(Type type, Dictionary<string,ReflectMember<T>> reflectableMembers)
        {
            SystemType = type;
            ReflectableMembers = reflectableMembers;
        }
    }
}
