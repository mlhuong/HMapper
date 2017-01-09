using HMapper.Dynamic;
using Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace HMapper.Extensions
{
    public static class CoreExtension
    {
        private static ConcurrentDictionary<Type, object> dicDynamicConstructrors = new ConcurrentDictionary<Type, object>();

        public static Expression CreateInstance(this Type type)
        {
            var hasParameterlessCtor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any(x=>!x.GetParameters().Any());
            if (!hasParameterlessCtor)
            {
                if (!type.GetTypeInfo().IsValueType)
                    throw new Exception($"Type {type.FullName} does not have a parameterless constructor.");

                var funcType = typeof(Func<>).MakeGenericType(type);

                var dlg = dicDynamicConstructrors.GetOrAdd(type, t =>
                {
                    var dynMethod = new DynamicMethod(string.Empty, type, Type.EmptyTypes, DynamicModule.ModuleBuilder, true);
                    ILGenerator il = dynMethod.GetILGenerator();
                    il.DeclareLocal(type);
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ret);

                    return dynMethod.CreateDelegate(funcType);
                });
                return Expression.Call(Expression.Constant(dlg).Convert(funcType), funcType.GetMethod("Invoke"), null);
            }
            return Expression.New(type);
        }
    }
}
