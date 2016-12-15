using HMapper.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HMapper.Extensions;

namespace HMapper
{
    public class Mapper
    {
        /// <summary>
        /// Create a mapped object given the source object.
        /// All the relational objects will automatically be retrieved as well.
        /// </summary>
        /// <typeparam name="TTarget">Type of the target object</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>The instance of the associated target object.</returns>
        public static TTarget Map<TSource,TTarget>(TSource source)
        {
            return _Map<TSource, TTarget>(source, MapMode.All, null);
        }

        /// <summary>
        /// Create a mapped object given the source object.
        /// Only relational objects that appear in the inclusions parameter will be retrieved.
        /// </summary>
        /// <typeparam name="TTarget">Type of the target object</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="inclusions">Linq expression representing the relational objects to retrieve.</param>
        /// <returns>The generated target object.</returns>
        public static TTarget Map<TSource, TTarget>(TSource source, params Expression<Func<TTarget, object>>[] inclusions)
        {
            return _Map<TSource, TTarget>(source, MapMode.Include, IncludeChain.CreateFromLambdas(inclusions));
        }

        /// <summary>
        /// Create a mapped object given the source object.
        /// Relational objects that appear in the exclusions parameter will be ignored.
        /// </summary>
        /// <typeparam name="TTarget">Type of the target object</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="exclusions">Linq expression representing the relational objects to ignore.</param>
        /// <returns>The generated target object.</returns>
        public static TTarget MapExclude<TSource, TTarget>(TSource source, params Expression<Func<TTarget, object>>[] exclusions)
        {
            return _Map<TSource, TTarget>(source, MapMode.Exclude, IncludeChain.CreateFromLambdas(exclusions));
        }

        /// <summary>
        /// Fill the specified target object given the source object.
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="target">Target object to fill.</param>
        /// <returns></returns>
        public static void Fill<TSource, TTarget>(TSource source, TTarget target)
        {
            _Fill(source, target, MapMode.All, null);
        }

        /// <summary>
        /// Fill the specified target object given the source object.
        /// Only relational objects that appear in the inclusions parameter will be retrieved.
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="target">Target object to fill.</param>
        /// <param name="inclusions">Linq expression representing the relational objects to retrieve.</param>
        /// <returns></returns>
        public static void Fill<TSource, TTarget>(TSource source, TTarget target, params Expression<Func<TTarget, object>>[] inclusions)
        {
            _Fill(source, target, MapMode.Include, IncludeChain.CreateFromLambdas(inclusions));
        }

        /// <summary>
        /// Fill the specified target object given the source object.
        /// Relational objects that appear in the exclusions parameter will be ignored.
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="target">Target object to fill.</param>
        /// <param name="exclusions">Linq expression representing the relational objects to ignore.</param>
        /// <returns></returns>
        public static void FillExclude<TSource, TTarget>(TSource source, TTarget target, params Expression<Func<TTarget, object>>[] exclusions)
        {
            _Fill(source, target, MapMode.Exclude, IncludeChain.CreateFromLambdas(exclusions));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _Fill<TSource, TTarget>(TSource source, TTarget target, MapMode mapMode, IncludeChain chain)
        {
            if (source == null) return;

            MapperCache.Cache = new Dictionary<Tuple<object, Type>, object>();
            Action<TSource, TTarget, IncludeChain> builder;
            switch (mapMode)
            {
                case MapMode.All:
                    builder = PolymorphismManager<TSource, TTarget>.FillerModeAll.Value;
                    break;
                case MapMode.Include:
                    builder = PolymorphismManager<TSource, TTarget>.FillerModeInclusion.Value;
                    break;
                case MapMode.Exclude:
                    builder = PolymorphismManager<TSource, TTarget>.FillerModeExclusion.Value;
                    break;
                default: throw new NotSupportedException();
            }
            if (builder == null)
                throw new NotMappedException(typeof(TSource), typeof(TTarget));
            builder(source, target, chain);
            MapperCache.Cache = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TTarget _Map<TSource, TTarget>(TSource source, MapMode mapMode, IncludeChain chain)
        {
            TTarget result;

            MapperCache.Cache = new Dictionary<Tuple<object, Type>, object>();
            Func<TSource, IncludeChain, TTarget> builder;
            switch (mapMode)
            {
                case MapMode.All: builder = PolymorphismManager<TSource, TTarget>.CreatorModeAll.Value;
                    break;
                case MapMode.Include:
                    builder = PolymorphismManager<TSource, TTarget>.CreatorModeInclusion.Value;
                    break;
                case MapMode.Exclude:
                    builder = PolymorphismManager<TSource, TTarget>.CreatorModeExclusion.Value;
                    break;
                default: throw new NotSupportedException();
            }
            if (builder == null)
                throw new NotMappedException(typeof(TSource), typeof(TTarget));
            result = builder(source, chain);
            MapperCache.Cache = null;
            return result;
        }
    }
}
