using HMapper.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace HMapper
{
    /// <summary>
    /// Initializer for API mapping.
    /// </summary>
    internal class MapperAPIInitializer : IMapperAPIInitializer
    {
        bool ItemCacheEnabledByDefault;

        /// <summary>
        /// Indicates that all mappings have items cache enabled by default. We can however disable the cache for each mapping  with "DisableItemsCache()".
        /// </summary>
        public void EnableItemsCacheByDefault()
        {
            ItemCacheEnabledByDefault = true;
        }

        /// <summary>
        /// Map a target type to a source type.
        /// </summary>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <typeparam name="TSource">Source type.</typeparam>
        public IMapperAPI<TSource, TTarget> Map<TSource, TTarget>()
        {
            Tuple<Type, Type> tuple = new Tuple<Type, Type>(typeof(TSource), typeof(TTarget));
            if (MapInfo._CacheGenericMaps.ContainsKey(tuple))
                throw new Exception($"The target type {typeof(TTarget).FullName} is already mapped to the type {typeof(TSource).FullName}.");

            CheckGenerics<TSource, TTarget>();
            var mapInfo = MapInfo._CacheGenericMaps.GetOrAdd(tuple, t => new MapInfo(tuple, ItemCacheEnabledByDefault));
            return new MapperAPI<TSource, TTarget>(mapInfo);
        }

        /// <summary>
        /// Map a target type to a source type.
        /// </summary>
        /// <param name="targetType">Target type.</param>
        /// <param name="sourceType">Source type.</param>
        /// <returns></returns>
        public IMapperAPI Map(Type sourceType, Type targetType)
        {
            Type openTargetType = targetType.GetGenType();
            Type openSourceType = sourceType.GetGenType();

            Tuple<Type, Type> tuple = new Tuple<Type, Type>(openSourceType, openTargetType);
            if (MapInfo._CacheGenericMaps.ContainsKey(tuple))
               throw new Exception($"The target type {targetType.FullName} is already mapped to the type {sourceType.FullName}.");

            var mapInfo = MapInfo._CacheGenericMaps.GetOrAdd(tuple, t => new MapInfo(tuple, ItemCacheEnabledByDefault));
            return new MapperAPI(mapInfo);
        }

        /// <summary>
        /// Map a target type to a source type by providing a custom target type builder.
        /// </summary>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <param name="builder">Custom target type builder.</param>
        public IMapperAPI ManualMap<TSource, TTarget>(Expression<Func<TSource, TTarget>> builder)
        {
            Tuple<Type, Type> tuple = new Tuple<Type, Type>(typeof(TSource), typeof(TTarget));
            if (MapInfo._CacheGenericMaps.ContainsKey(tuple))
                throw new Exception($"The target type {typeof(TTarget).FullName} is already mapped to the type {typeof(TSource).FullName}.");

            CheckGenerics<TSource, TTarget>();
            var mapInfo = MapInfo._CacheGenericMaps.GetOrAdd(tuple, t => new MapInfo(tuple, ItemCacheEnabledByDefault)
            {
                ManualBuilder = Expression.Lambda(
                typeof(Func<TSource, TTarget>),
                builder.Body,
                builder.Parameters[0]
                )
            });
            return new MapperAPI(mapInfo);
        }

        /// <summary>
        /// Map a target type to a source type by providing a Linq expression tree that returns the target type.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="expression"></param>
        internal void MapExpression<TSource, TTarget>(MapBuilder.MapExpressionDlg expressionDlg)
        {
            Tuple<Type, Type> tuple = new Tuple<Type, Type>(typeof(TSource), typeof(TTarget));
            if (MapInfo._CacheGenericMaps.ContainsKey(tuple))
                throw new Exception($"The target type {typeof(TTarget).FullName} is already mapped to the type {typeof(TSource).FullName}.");

            CheckGenerics<TSource, TTarget>();
            MapInfo._CacheGenericMaps.GetOrAdd(tuple, t => new MapInfo(tuple, false) { ExpressionBuilder = expressionDlg });
        }

        private void CheckGenerics<TSource, TTarget>()
        {
            if (typeof(TTarget).GetTypeInfo().IsGenericType)
            {
                var genArgsTarget = typeof(TTarget).GetGenericArguments().Where(a => typeof(IGeneric).IsAssignableFrom(a)).ToArray();
                if (!genArgsTarget.Any()) return;
                if (!typeof(TSource).GetTypeInfo().IsGenericType)
                    throw new GenericException($"The source type [{typeof(TTarget).FullName}] contains unresolved generic types. They must match generics of the source type.");
                var genArgsSource = typeof(TSource).GetGenericArguments().Where(a => typeof(IGeneric).IsAssignableFrom(a)).ToArray();
                if (genArgsTarget.Any(a => !genArgsSource.Contains(a)))
                    throw new GenericException($"The source type [{typeof(TTarget).FullName}] contains unresolved generic types. They must match generics of the source type.");
            }
        }
    }
}
