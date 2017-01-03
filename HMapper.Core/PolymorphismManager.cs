using HMapper.Extensions;
using Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("HMapperTests")]
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
            MapInfo mapInfo=null;
            Dictionary<Type, GenericAssociation> genericTypeAssociation = null;
            Expression builderExpression;
            if (source.Type != typeof(object))
            {
                if (source.Type.IsSimpleType())
                    return source;

                mapInfo = MapInfo.Get(source.Type, targetType, false, out genericTypeAssociation);

                if (mapInfo == null)
                    return targetType.IsAssignableFrom(source.Type) ? source : null;
            }

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
            if (source.Type.GetTypeInfo().IsValueType) // enums. Simple types have been handled earlier.
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
            MapInfo mapInfo = null;
            Dictionary<Type, GenericAssociation> genericTypeAssociation = null;

            if (source.Type != typeof(object))
            {
                if (source.Type.IsSimpleType())
                    return source;

                mapInfo = MapInfo.Get(source.Type, target.Type, true, out genericTypeAssociation);

                if (mapInfo == null)
                    return target.Type.IsAssignableFrom(source.Type) ? source : null;
            }

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
