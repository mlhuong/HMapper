//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace HMapper
//{
//    /// <summary>
//    /// Class containing the instance creators or fillers for each couple (source type, target type)
//    /// </summary>
//    /// <typeparam name="TSource"></typeparam>
//    /// <typeparam name="TTarget"></typeparam>
//    internal class MapBuilder<TSource, TTarget>
//    {
//        public static Lazy<Func<TSource, IncludeChain, TTarget>> CreatorModeAll = new Lazy<Func<TSource, IncludeChain, TTarget>>(() => GetCreateDelegate(MapMode.All));
//        public static Lazy<Func<TSource, IncludeChain, TTarget>> CreatorModeInclusion = new Lazy<Func<TSource, IncludeChain, TTarget>>(() => GetCreateDelegate(MapMode.Include));
//        public static Lazy<Func<TSource, IncludeChain, TTarget>> CreatorModeExclusion = new Lazy<Func<TSource, IncludeChain, TTarget>>(() => GetCreateDelegate(MapMode.Exclude));
//        public static Lazy<Action<TSource, TTarget, IncludeChain>> FillerModeAll = new Lazy<Action<TSource, TTarget, IncludeChain>>(() => GetFillDelegate(MapMode.All));
//        public static Lazy<Action<TSource, TTarget, IncludeChain>> FillerModeInclusion = new Lazy<Action<TSource, TTarget, IncludeChain>>(() => GetFillDelegate(MapMode.Include));
//        public static Lazy<Action<TSource, TTarget, IncludeChain>> FillerModeExclusion = new Lazy<Action<TSource, TTarget, IncludeChain>>(() => GetFillDelegate(MapMode.Exclude));

//        /// <summary>
//        /// Creates the creator delegate.
//        /// </summary>
//        /// <param name="mapMode"></param>
//        /// <returns></returns>
//        private static Func<TSource, IncludeChain, TTarget> GetCreateDelegate(MapMode mapMode)
//        {
//            Type sourceType = typeof(TSource);
//            Type targetType = typeof(TTarget);
//            ParameterExpression paramSource = Expression.Parameter(sourceType);
//            ParameterExpression paramInclude = Expression.Parameter(typeof(IncludeChain));
//            Dictionary<Type, GenericAssociation> genericTypeAssociation;
//            var mapInfo = MapInfo.Get(sourceType, targetType, out genericTypeAssociation);
//            if (mapInfo == null) return null;
//            var body = MapBuilder.CreateBuilderExpression(
//                                mapMode: mapMode,
//                                mapInfo: mapInfo,
//                                genericTypeAssociation: genericTypeAssociation,
//                                paramSource: paramSource,
//                                varResult: null,
//                                paramIncludeChain: paramInclude,
//                                usedBuilders: null);
//            return Expression.Lambda<Func<TSource, IncludeChain, TTarget>>(
//                body,
//                paramSource,
//                paramInclude
//                ).Compile();
//        }

//        /// <summary>
//        /// Create the filler delegate.
//        /// </summary>
//        /// <param name="mapMode"></param>
//        /// <returns></returns>
//        private static Action<TSource, TTarget, IncludeChain> GetFillDelegate(MapMode mapMode)
//        {
//            Type sourceType = typeof(TSource);
//            Type targetType = typeof(TTarget);
//            ParameterExpression paramSource = Expression.Parameter(sourceType);
//            ParameterExpression paramTarget = Expression.Parameter(targetType);
//            ParameterExpression paramInclude = Expression.Parameter(typeof(IncludeChain));
//            Dictionary<Type, GenericAssociation> genericTypeAssociation;
//            var mapInfo = MapInfo.Get(sourceType, targetType, out genericTypeAssociation);
//            if (mapInfo == null) return null;
//            var body = MapBuilder.CreateBuilderExpression(
//                                mapMode: mapMode,
//                                mapInfo: mapInfo,
//                                genericTypeAssociation: genericTypeAssociation,
//                                paramSource: paramSource,
//                                varResult: paramTarget,
//                                paramIncludeChain: paramInclude,
//                                usedBuilders: null);
//            return Expression.Lambda<Action<TSource, TTarget, IncludeChain>>(
//                body,
//                paramSource,
//                paramTarget,
//                paramInclude
//                ).Compile();
//        }
//    }
//}
