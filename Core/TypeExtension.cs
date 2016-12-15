using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMapper
{
    internal static class TypeExtension
    {
        /// <summary>
        /// Replace a type containing IGeneric arguments by specified types.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="associations"></param>
        /// <returns></returns>
        public static Type ReplaceGenerics(this Type type, ReplacementType replacementType, Dictionary<Type, GenericAssociation> associations)
        {
            GenericAssociation association;
            if (associations.TryGetValue(type, out association))
                return replacementType==ReplacementType.Source ? association.SourceType : association.TargetType;

            if (type.IsArray)
                return ReplaceGenerics(type.GetElementType(), replacementType, associations).MakeArrayType();

            if (!type.IsGenericType)
                return type;

            Type[] args = type.GetGenericArguments();
            
            for (int i = 0; i < args.Length; i++)
                 args[i] = ReplaceGenerics(args[i], replacementType, associations);

            return type.GetGenericTypeDefinition().MakeGenericType(args);
        }
        

        /// <summary>
        /// Get an "open" type using IGeneric types.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetGenType(this Type type)
        {
            if (type.ContainsGenericParameters)
                return type.GetGenericTypeDefinition().MakeGenericType(type.GetGenericArguments().Select((x, index) => Type.GetType(String.Format("HMapper.TGen{0}", index+1))).ToArray());

            return type;
        }

        /// <summary>
        /// Indicates if the type is simple (primitive or string, or datetime, or nullable)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimpleType(this Type type)
        {
            return (type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) 
                || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }
    }
}
