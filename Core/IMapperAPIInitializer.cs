using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HMapper
{
    /// <summary>
    /// Initializer interface.
    /// </summary>
    public interface IMapperAPIInitializer
    {
        /// <summary>
        /// Indicates that all mappings have items cache enabled by default. We can however disable the cache for each mapping  with "DisableItemsCache()".
        /// </summary>
        /// <returns></returns>
        void EnableItemsCacheByDefault();

        /// <summary>
        /// Map a target type to a source type.
        /// </summary>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <returns></returns>
        IMapperAPI<TSource, TTarget> Map<TSource, TTarget>();

        /// <summary>
        /// Map a target type to a source type.
        /// </summary>
        /// <param name="targetType">Target type.</param>
        /// <param name="sourceType">Source type.</param>
        /// <returns></returns>
        IMapperAPI Map(Type sourceType, Type targetType);

        /// <summary>
        /// Map a target type to a source type by providing a custom target type builder.
        /// </summary>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <param name="builder">Custom target type builder.</param>
        IMapperAPI ManualMap<TSource, TTarget>(Expression<Func<TSource, TTarget>> builder);
    }
}
