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
            if (!type.GetTypeInfo().IsGenericType) return IsInteger(type);
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
            if (!type.GetTypeInfo().IsGenericType) return IsFloat(type);
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
            if (!type.GetTypeInfo().IsGenericType) return false;
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
            if ((!type.GetTypeInfo().IsGenericType) || (!typeof(IEnumerable).IsAssignableFrom(type))) return type;
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

        //public static bool IsCastableTo(this Type from, Type to)
        //{
        //    return to.IsAssignableFrom(from)
        //        || IsCastDefined(to, m => m.GetParameters()[0].ParameterType, _ => from, false)
        //        || IsCastDefined(from, _ => to, m => m.ReturnType, true);
        //}

        ////little irrelevant DRY method
        //static bool IsCastDefined(Type type, Func<MethodInfo, Type> baseType, Func<MethodInfo, Type> derivedType,
        //                          bool lookInBase)
        //{
        //    var bindinFlags = BindingFlags.Public
        //                    | BindingFlags.Static
        //                    | (lookInBase ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);
        //    return type.GetMethods(bindinFlags).Any(m => (m.Name == "op_Implicit" || m.Name == "op_Explicit")
        //                                              && baseType(m).IsAssignableFrom(derivedType(m)));
        //}

        public static bool IsCastableTo(this Type from, Type to, bool implicitly = false)
        {
            return to.IsAssignableFrom(from) || from.HasCastDefined(to, implicitly);
        }

        static bool HasCastDefined(this Type from, Type to, bool implicitly)
        {
            if ((from.GetTypeInfo().IsPrimitive || from.GetTypeInfo().IsEnum) && (to.GetTypeInfo().IsPrimitive || to.GetTypeInfo().IsEnum))
            {
                if (!implicitly)
                    return from == to || (from != typeof(Boolean) && to != typeof(Boolean));

                Type[][] typeHierarchy = {
            new Type[] { typeof(Byte),  typeof(SByte), typeof(Char) },
            new Type[] { typeof(Int16), typeof(UInt16) },
            new Type[] { typeof(Int32), typeof(UInt32) },
            new Type[] { typeof(Int64), typeof(UInt64) },
            new Type[] { typeof(Single) },
            new Type[] { typeof(Double) }
        };
                IEnumerable<Type> lowerTypes = Enumerable.Empty<Type>();
                foreach (Type[] types in typeHierarchy)
                {
                    if (types.Any(t => t == to))
                        return lowerTypes.Any(t => t == from);
                    lowerTypes = lowerTypes.Concat(types);
                }

                return false;   // IntPtr, UIntPtr, Enum, Boolean
            }
            return IsCastDefined(to, m => m.GetParameters()[0].ParameterType, _ => from, implicitly, false)
                || IsCastDefined(from, _ => to, m => m.ReturnType, implicitly, true);
        }

        static bool IsCastDefined(Type type, Func<MethodInfo, Type> baseType,
                                Func<MethodInfo, Type> derivedType, bool implicitly, bool lookInBase)
        {
            var bindinFlags = BindingFlags.Public | BindingFlags.Static
                            | (lookInBase ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);
            return type.GetMethods(bindinFlags).Any(
                m => (m.Name == "op_Implicit" || (!implicitly && m.Name == "op_Explicit"))
                    && baseType(m).IsAssignableFrom(derivedType(m)));
        }
    }
}
