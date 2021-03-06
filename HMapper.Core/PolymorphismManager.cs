﻿using HMapper.Extensions;
using Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("HMapperTests,PublicKey=" +
                              "0024000004800000940000000602000000240000525341310004000001000100250d6b0b688d0b"
                              +"f8510e3424c0da75348d0f8c848c9c3028b80cb7ab1031797488e9d414cf162b1356a068e6cd32"
                              +"43bc141ee5da80b6412b6e146150c76cf9bcbe37e8cbc0c78fae4d8b0d77e306b4be278ed037a5"
                              +"f017c3d9c4dccc6dee3e0fc2749f783902fcc142adf085eefea797d2d8ea62a9888156f0c29cc6"
                              +"26e202bc")]
namespace HMapper
{
    /// <summary>
    /// Class that calls the builder if the source type is not polymorphic, or call "getType()" and use a cached builder if the source type is polymorphic.
    /// This class is also used to avoid circular reference issue.
    /// Example: if A contains B and B contains A, instead of having getBuilder(A), getBuilder(B), getBuilder(A)...., we will have getBuilder(A), getBuilder(B), call cached builder(A).
    /// </summary>
    internal static class PolymorphismManager
    {
        /// <summary>
        /// Get the expression that creates an instance. Call GetPolymorphedCreator(source.GetType()) if polymorphic.
        /// </summary>
        /// <param name="mapMode"></param>
        /// <param name="source"></param>
        /// <param name="targetType"></param>
        /// <param name="sourceType"></param>
        /// <param name="includeChain"></param>
        /// <param name="usedBuilders"></param>
        /// <returns></returns>
        internal static Expression GetMostConcreteExpressionCreator(MapMode mapMode, Expression source, Type targetType, Expression includeChain, List<Tuple<Type, Type>> usedBuilders)
        {
            Dictionary<Type, GenericAssociation> genericTypeAssociation;
            Expression builderExpression;

            if (MapInfo._PolymorphTypes.Contains(source.Type) || usedBuilders.Contains(Tuple.Create(source.Type, targetType)))
            {
                Expression sourceTypeExpr = MapInfo._PolymorphTypes.Contains(source.Type) ?
                    (Expression)Expression.Call(source, typeof(object).GetMethod("GetType"))
                    : Expression.Constant(source.Type);

                builderExpression = Expression.Call(
                    Expression.Call(
                            typeof(PolymorphismManager<,>).MakeGenericType(source.Type, targetType).GetMethod(nameof(PolymorphismManager<object, object>.GetCreatorByType)),
                            Expression.Call(
                                Meta.Method(() => Tuple.Create<Type, MapMode>(null, MapMode.All)),
                                sourceTypeExpr,
                                Expression.Constant(mapMode)
                                )),
                    typeof(Func<,,>).MakeGenericType(source.Type, typeof(IncludeChain), targetType).GetMethod("Invoke"),
                    source, 
                    includeChain
                    );
            }
            else
            {
                MapInfo mapInfo = MapInfo.Get(source.Type, targetType, false, out genericTypeAssociation);
                if (mapInfo == null)
                    return targetType.IsAssignableFrom(source.Type) ? source.Convert(targetType) : null;

                var actualBuilderExpression = MapBuilder.CreateBuilderExpression(
                                mapMode: mapMode,
                                mapInfo: mapInfo,
                                genericTypeAssociation: genericTypeAssociation,
                                paramSource: source,
                                varResult: null,
                                paramIncludeChain: includeChain,
                                usedBuilders: usedBuilders.ToList());
                builderExpression = mapInfo.UseItemsCache ? MapperCache.GetOrSet(source, targetType, includeChain, actualBuilderExpression) : actualBuilderExpression;
            }
            if (builderExpression == null) return null;

            var resultVar = Expression.Variable(targetType);

            Expression result;
            if (source.Type.IsSimpleType()) // enums. Simple types have been handled earlier.
                result = builderExpression;
            else
                result = Expression.Block(
                    new ParameterExpression[] { resultVar },
                    Expression.IfThenElse(
                        Expression.Equal(source, Expression.Default(source.Type)),
                        Expression.Assign(resultVar, Expression.Default(targetType)),
                        Expression.Assign(resultVar, builderExpression)
                    ),
                    resultVar
                );
            return result;
        }

        /// <summary>
        /// Get the expression that fills an instance. Call GetPolymorphedFiller(source.GetType(), target.GetType()) if polymorphic.
        /// </summary>
        /// <param name="mapMode"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="includeChain"></param>
        /// <returns></returns>
        internal static Expression GetMostConcreteExpressionFiller(MapMode mapMode, Expression source, Expression target, Expression includeChain)
        {
            Dictionary<Type, GenericAssociation> genericTypeAssociation = null;

            if (MapInfo._PolymorphTypes.Contains(source.Type))
            {
                return Expression.Call(
                    Expression.Call(
                            typeof(PolymorphismManager<,>).MakeGenericType(source.Type, target.Type).GetMethod(nameof(PolymorphismManager<object, object>.GetFillerByType)),
                            Expression.Call(
                                Meta.Method(() => Tuple.Create<Type, Type, MapMode>(null, null, MapMode.All)),
                                Expression.Call(source, typeof(object).GetMethod("GetType")),
                                Expression.Call(target, typeof(object).GetMethod("GetType")),
                                Expression.Constant(mapMode)
                                )),
                    typeof(Action<,,>).MakeGenericType(source.Type, target.Type, typeof(IncludeChain)).GetMethod("Invoke"),
                    source,
                    target,
                    includeChain
                    );
            }
            else
            {
                MapInfo mapInfo = MapInfo.Get(source.Type, target.Type, true, out genericTypeAssociation);
                if (mapInfo == null)
                    return target.Type.IsAssignableFrom(source.Type) ? source : null;

                return MapBuilder.CreateBuilderExpression(
                                mapMode: mapMode,
                                mapInfo: mapInfo,
                                genericTypeAssociation: genericTypeAssociation,
                                paramSource: source,
                                varResult: target,
                                paramIncludeChain: includeChain,
                                usedBuilders: null);
            }
        }
    }
}
