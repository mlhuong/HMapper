using HMapper.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMapper
{
    /// <summary>
    /// Class containing the cached delegates that create or fill an instance.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    internal static class PolymorphismManager<TSource, TTarget>
    {
        static ConcurrentDictionary<Tuple<Type, MapMode>, Func<TSource, IncludeChain, TTarget>> _CacheCreators;
        static ConcurrentDictionary<Tuple<Type, Type, MapMode>, Action<TSource, TTarget, IncludeChain>> _CacheFillers;
        public static Lazy<Func<TSource, IncludeChain, TTarget>> CreatorModeAll = new Lazy<Func<TSource, IncludeChain, TTarget>>(() => GetMostConcreteCreator(MapMode.All));
        public static Lazy<Func<TSource, IncludeChain, TTarget>> CreatorModeInclusion = new Lazy<Func<TSource, IncludeChain, TTarget>>(() => GetMostConcreteCreator(MapMode.Include));
        public static Lazy<Func<TSource, IncludeChain, TTarget>> CreatorModeExclusion = new Lazy<Func<TSource, IncludeChain, TTarget>>(() => GetMostConcreteCreator(MapMode.Exclude));
        public static Lazy<Action<TSource, TTarget, IncludeChain>> FillerModeAll = new Lazy<Action<TSource, TTarget, IncludeChain>>(() => GetMostConcreteFiller(MapMode.All));
        public static Lazy<Action<TSource, TTarget, IncludeChain>> FillerModeInclusion = new Lazy<Action<TSource, TTarget, IncludeChain>>(() => GetMostConcreteFiller(MapMode.Include));
        public static Lazy<Action<TSource, TTarget, IncludeChain>> FillerModeExclusion = new Lazy<Action<TSource, TTarget, IncludeChain>>(() => GetMostConcreteFiller(MapMode.Exclude));

        /// <summary>
        /// Static initialization
        /// </summary>
        static PolymorphismManager()
        {
            _CacheCreators = new ConcurrentDictionary<Tuple<Type, MapMode>, Func<TSource, IncludeChain, TTarget>>();
            _CacheFillers = new ConcurrentDictionary<Tuple<Type, Type, MapMode>, Action<TSource, TTarget, IncludeChain>>();
        }

        /// <summary>
        /// Create the creator delegate.
        /// </summary>
        /// <param name="paramSource"></param>
        /// <param name="paramInclude"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static Func<TSource, IncludeChain, TTarget> GetCreateDelegate(ParameterExpression paramSource, ParameterExpression paramInclude, Expression body)
        {
            if (body == null) return null;
            return Expression.Lambda<Func<TSource, IncludeChain, TTarget>>(
                body,
                paramSource,
                paramInclude
                ).Compile();
        }

        /// <summary>
        /// Create the filler delegate.
        /// </summary>
        /// <param name="paramSource"></param>
        /// <param name="paramTarget"></param>
        /// <param name="paramInclude"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static Action<TSource, TTarget, IncludeChain> GetFillDelegate(ParameterExpression paramSource, ParameterExpression paramTarget, ParameterExpression paramInclude, Expression body)
        {
            if (body == null) return null;
            return Expression.Lambda<Action<TSource, TTarget, IncludeChain>>(
                body,
                paramSource,
                paramTarget,
                paramInclude
                ).Compile();
        }

        /// <summary>
        /// Get the most concrete creator delegate.
        /// </summary>
        /// <param name="mapMode"></param>
        /// <returns></returns>
        private static Func<TSource, IncludeChain, TTarget> GetMostConcreteCreator(MapMode mapMode)
        {
            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);
            ParameterExpression paramSource = Expression.Parameter(sourceType);
            ParameterExpression paramInclude = Expression.Parameter(typeof(IncludeChain));
            var body = PolymorphismManager.GetMostConcreteExpressionCreator(mapMode, paramSource, targetType, paramInclude, new List<Tuple<Type, Type>>());
            return GetCreateDelegate(paramSource, paramInclude, body);
        }


        /// <summary>
        /// Get the most concrete filler delegate.
        /// </summary>
        /// <param name="mapMode"></param>
        /// <returns></returns>
        private static Action<TSource, TTarget, IncludeChain> GetMostConcreteFiller(MapMode mapMode)
        {
            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);
            ParameterExpression paramSource = Expression.Parameter(sourceType);
            ParameterExpression paramTarget = Expression.Parameter(targetType);
            ParameterExpression paramInclude = Expression.Parameter(typeof(IncludeChain));
            var body = PolymorphismManager.GetMostConcreteExpressionFiller(mapMode, paramSource, paramTarget, paramInclude);
            return GetFillDelegate(paramSource, paramTarget, paramInclude, body);

        }

        /// <summary>
        /// Get the right creator delegate from the cache depending on the real type of the source.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Func<TSource, IncludeChain, TTarget> GetCreatorByType(Tuple<Type, MapMode> key)
        {
            return _CacheCreators.GetOrAdd(key, k =>
            {
                Type sourceType = typeof(TSource);
                Type targetType = typeof(TTarget);

                ParameterExpression paramSource = Expression.Parameter(sourceType);
                ParameterExpression paramInclude = Expression.Parameter(typeof(IncludeChain));
                Dictionary<Type, GenericAssociation> genericTypeAssociation;
                var mapInfo = MapInfo.Get(k.Item1, targetType, false, out genericTypeAssociation);
                if (mapInfo == null) return (source, include) => default(TTarget);
                var builderExpr = MapBuilder.CreateBuilderExpression(
                                        mapMode: k.Item2,
                                        mapInfo: mapInfo,
                                        genericTypeAssociation: genericTypeAssociation,
                                        paramSource: paramSource,
                                        varResult: null,
                                        paramIncludeChain: paramInclude,
                                        usedBuilders: null);

                var body = mapInfo.UseItemsCache ?
                    MapperCache.GetOrSet(
                        paramSource,
                        targetType,
                        paramInclude,
                        builderExpr)
                    : builderExpr;

                return Expression.Lambda<Func<TSource, IncludeChain, TTarget>>(
                    body,
                    paramSource,
                    paramInclude
                    ).Compile();
            });
        }

        /// <summary>
        /// Get the right filler delegate from the cache depending on the real type of the source and the target.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Action<TSource, TTarget, IncludeChain> GetFillerByType(Tuple<Type, Type, MapMode> key)
        {
            return _CacheFillers.GetOrAdd(key, k =>
            {
                Type sourceType = typeof(TSource);
                Type targetRealType = k.Item2;
                ParameterExpression paramSource = Expression.Parameter(sourceType);
                ParameterExpression paramTarget = Expression.Parameter(typeof(TTarget));
                ParameterExpression paramInclude = Expression.Parameter(typeof(IncludeChain));
                ParameterExpression varRealTypeTarget = Expression.Parameter(targetRealType);
                Dictionary<Type, GenericAssociation> genericTypeAssociation;
                var mapInfo = MapInfo.Get(k.Item1, targetRealType, true, out genericTypeAssociation);
                if (mapInfo == null) return (source, target, include)=> { };
                var body = Expression.Block(
                    new ParameterExpression[] { varRealTypeTarget },
                    Expression.Assign(varRealTypeTarget, paramTarget.Convert(targetRealType)),
                    MapBuilder.CreateBuilderExpression(
                                        mapMode: k.Item3,
                                        mapInfo: mapInfo,
                                        genericTypeAssociation: genericTypeAssociation,
                                        paramSource: paramSource,
                                        varResult: varRealTypeTarget,
                                        paramIncludeChain: paramInclude,
                                        usedBuilders: null),
                    varRealTypeTarget
                    );
                return Expression.Lambda<Action<TSource, TTarget, IncludeChain>>(
                    body,
                    paramSource,
                    paramTarget,
                    paramInclude
                    ).Compile();
            });
        }
    }
}
