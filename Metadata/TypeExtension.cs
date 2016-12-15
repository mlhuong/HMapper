using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Metadata
{
    /// <summary>
    /// Extensions method for Type class.
    /// </summary>
    public static class TypeExtension
    {
        static readonly Type[] IntegerTypes = new Type[] { typeof(Byte), typeof(SByte), typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(Int16), typeof(Int32), typeof(Int64)};
        static readonly Type[] FloatTypes = new Type[] { typeof(Single), typeof(Double), typeof(Decimal) };

        /// <summary>
        /// Indicates if the type is an integer (or equivalent)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsInteger(this Type type)
        {
            return IntegerTypes.Contains(type);
        }

        /// <summary>
        /// Indicates if the type is a float (or equivalent)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsFloat(this Type type)
        {
            return FloatTypes.Contains(type);
        }

        /// <summary>
        /// Indicates if the type is an integer or a int? (or equivalent)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIntegerOrNull(this Type type)
        {
            if (!type.IsGenericType) return IsInteger(type);
            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return IsInteger(type.GetGenericArguments()[0]);
            return false;
        }

        /// <summary>
        /// Indicates if the type is a float or float? (or equivalent)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsFloatOrNull(this Type type)
        {
            if (!type.IsGenericType) return IsFloat(type);
            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return IsFloat(type.GetGenericArguments()[0]);
            return false;
        }

        /// <summary>
        /// Indicates if the type is a Nullable type and returns the innerType if it is the case.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="innerType"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type, ref Type innerType)
        {
            if (!type.IsGenericType) return false;
            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                innerType = type.GetGenericArguments()[0];
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Return the element type of a collection type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetElementTypeOrType(this Type type)
        {
            if (type.IsArray) return type.GetElementType();
            if (type == typeof(ArrayList)) return typeof(object);
            if ((!type.IsGenericType) || (!typeof(IEnumerable).IsAssignableFrom(type))) return type;
            var genTypeDef = type.GetGenericTypeDefinition();
            if ((genTypeDef == typeof(Dictionary<,>)) || (genTypeDef == typeof(IDictionary<,>)))
                return typeof(KeyValuePair<,>).MakeGenericType(type.GetGenericArguments());
            return type.GetGenericArguments().First();
        }

        public static MemberInfo GetInheritedMember(this Type type, string name)
        {
            return
                new[] { type }.Concat(type.GetTypeInfo().ImplementedInterfaces)
                .SelectMany(i => i.GetMember(name))
                .FirstOrDefault();
        }
    }
}
