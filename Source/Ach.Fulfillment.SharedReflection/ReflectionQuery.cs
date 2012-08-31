using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ach.Fulfillment.Shared.Reflection
{
	public class ReflectionQuery<T> where T : Attribute
    {
        private static Dictionary<string, ReflectionQueryResult<T>> _reflectionCache = new Dictionary<string, ReflectionQueryResult<T>>();

        public ReflectionQueryResult<T> Reflect(Type type)
        {
			if ( type == null )
				throw new ArgumentNullException( "type" );
			
            ReflectionQueryResult<T> reflectorCacheEntry;

            if (!_reflectionCache.TryGetValue(type.FullName, out reflectorCacheEntry)) //check
            {
                lock (_reflectionCache) //lock
                {
                    //double-check
					if ( !_reflectionCache.TryGetValue( type.FullName, out reflectorCacheEntry ) )
					{
						Dictionary<string, ReflectMember<T>> reflectorEntries = new Dictionary<string, ReflectMember<T>>( );
						QueryType( type, reflectorEntries );

						reflectorCacheEntry = new ReflectionQueryResult<T>( type, reflectorEntries ); //add to cache for all future requests

						_reflectionCache.Add(type.FullName, reflectorCacheEntry);
					}
                }
            }
            return reflectorCacheEntry;
        }

		private void QueryType( Type type, Dictionary<string, ReflectMember<T>> catalog )
		{
			if ( type.BaseType != null )
				QueryType( type.BaseType, catalog );

			var reflectableMembers =
				from memberInfo in type.FindMembers( MemberTypes.Field | MemberTypes.Property,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Default, null, null )
				where ((memberInfo.GetCustomAttributes( typeof( T ), false ) as T[])).Count( ) > 0
				//orderby ((memberInfo.GetCustomAttributes(typeof(T), false) as T[])[0] as IReflectAttribute).Position ascending
				select new { MemberInfo = memberInfo, ReflectAttributes = (memberInfo.GetCustomAttributes(typeof(T), false) as T[]) };

			if (reflectableMembers.Count() > 0 && reflectableMembers.First().ReflectAttributes.First() is IReflectAttribute)
				reflectableMembers = reflectableMembers.OrderBy(x => (x.ReflectAttributes.First() as IReflectAttribute).Position);

			foreach ( var entry in reflectableMembers )
			{
				if ( entry.MemberInfo.MemberType == MemberTypes.Property )
					catalog.Add( entry.MemberInfo.Name, new ReflectProperty<T>( entry.ReflectAttributes.First( ), entry.MemberInfo ) );
				else if ( entry.MemberInfo.MemberType == MemberTypes.Field )
					catalog.Add( entry.MemberInfo.Name, new ReflectField<T>( entry.ReflectAttributes.First( ), entry.MemberInfo ) );
			}
		}
    }
}
