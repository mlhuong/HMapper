using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HMapper
{
    /// <summary>
    /// Interface API used to work on a mapped member.
    /// </summary>
    public interface IMapperAPIMember
    {
        /// <summary>
        /// Links the member to the specified source member known by its name.
        /// </summary>
        /// <param name="sourceMember">Source member name.</param>
        /// <param name="retrievalMode">Indicates the retrieval mode (by default, primitives, struct, enums and custom functions are always retrieved, and objects are managed by inclusion specification).</param>
        /// <returns></returns>
        IMapperAPIMember LinkTo(string sourceMember, RetrievalMode retrievalMode);

        /// <summary>
        /// Ignore the target member.
        /// </summary>
        /// <returns></returns>
        IMapperAPIMember Ignore();
    }

    /// <summary>
    /// Interface API used to work on a mapped member (generic).
    /// </summary>
    /// <typeparam name="TTarget">Target type.</typeparam>
    /// <typeparam name="TSource">Source type.</typeparam>
    public interface IMapperAPIMember<TSource, TTarget>
    {
        /// <summary>
        /// Links the member to the specified source member specified via Linq expression.
        /// </summary>
        /// <typeparam name="TSourceProperty">Source member type.</typeparam>
        /// <param name="sourceMember">Linq expression targetting the source member.</param>
        /// <param name="retrievalMode">Indicates the retrieval mode (by default, primitives, struct, enums and custom functions are always retrieved, and objects are managed by inclusion specification).</param>
        /// <returns></returns>
        IMapperAPIMember<TSource, TTarget> LinkTo<TSourceProperty>(Expression<Func<TSource, TSourceProperty>> sourceMember, RetrievalMode retrievalMode = RetrievalMode.Default);

        /// <summary>
        /// Links the member to the specified source member specified by its name.
        /// </summary>
        /// <param name="sourceMember">Source member name.</param>
        /// <param name="retrievalMode">Indicates the retrieval mode (by default, primitives, struct, enums and custom functions are always retrieved, and objects are managed by inclusion specification).</param>
        /// <returns></returns>
        IMapperAPIMember<TSource, TTarget> LinkTo(string sourceMember, RetrievalMode retrievalMode = RetrievalMode.Default);

        /// <summary>
        /// Ignore the target member.
        /// </summary>
        /// <returns></returns>
        IMapperAPIMember<TSource, TTarget> Ignore();
    }
}
