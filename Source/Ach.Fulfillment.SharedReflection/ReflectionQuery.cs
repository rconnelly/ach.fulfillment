namespace Ach.Fulfillment.Shared.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    public class ReflectionQuery<T> where T : Attribute
    {
        private static readonly Dictionary<string, ReflectionQueryResult<T>> ReflectionCache = new Dictionary<string, ReflectionQueryResult<T>>();

        public ReflectionQueryResult<T> Reflect(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            ReflectionQueryResult<T> reflectorCacheEntry;

            // check
            Debug.Assert(type.FullName != null, "type.FullName != null");
            if (!ReflectionCache.TryGetValue(type.FullName, out reflectorCacheEntry))
            {
                // lock
                lock (ReflectionCache) 
                {
                    // double-check
                    if (!ReflectionCache.TryGetValue(type.FullName, out reflectorCacheEntry))
                    {
                        var reflectorEntries = new Dictionary<string, ReflectMember<T>>();
                        this.QueryType(type, reflectorEntries);

                        reflectorCacheEntry = new ReflectionQueryResult<T>(type, reflectorEntries); // add to cache for all future requests

                        ReflectionCache.Add(type.FullName, reflectorCacheEntry);
                    }
                }
            }

            return reflectorCacheEntry;
        }

        private void QueryType(Type type, IDictionary<string, ReflectMember<T>> catalog)
        {
            if (type.BaseType != null)
            {
                this.QueryType(type.BaseType, catalog);
            }

            var reflectableMembers =
                (from memberInfo in
                    type.FindMembers(
                        MemberTypes.Field | MemberTypes.Property,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Default,
                        null,
                        null)
                where memberInfo != null && memberInfo.GetCustomAttributes(typeof(T), false).OfType<T>().Any() /*orderby ((memberInfo.GetCustomAttributes(typeof(T), false) as T[])[0] as IReflectAttribute).Position ascending*/
                select
                    new
                        {
                            MemberInfo = memberInfo,
                            ReflectAttributes = memberInfo.GetCustomAttributes(typeof(T), false) as T[]
                        }).ToList().AsQueryable();

            if (reflectableMembers.Any() && reflectableMembers.First().ReflectAttributes.First() is IReflectAttribute)
            {
                reflectableMembers =
                    reflectableMembers.OrderBy(x => (x.ReflectAttributes.First() as IReflectAttribute).Position);
            }

            foreach (var entry in reflectableMembers)
            {
                if (entry.MemberInfo.MemberType == MemberTypes.Property)
                {
                    catalog.Add(
                        entry.MemberInfo.Name, new ReflectProperty<T>(entry.ReflectAttributes.First(), entry.MemberInfo));
                }
                else if (entry.MemberInfo.MemberType == MemberTypes.Field)
                {
                    catalog.Add(
                        entry.MemberInfo.Name, new ReflectField<T>(entry.ReflectAttributes.First(), entry.MemberInfo));
                }
            }
        }
    }
}
