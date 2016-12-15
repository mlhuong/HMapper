using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HMapper
{
    /// <summary>
    /// Mapping API Interface 
    /// </summary>
    public interface IMapperAPI
    {
        /// <summary>
        /// Declares a member mapping element.
        /// </summary>
        /// <param name="targetMember">Target member name.</param>
        /// <param name="apiMember">Action delegate in which users can use an instance of IMapperAPIMember.</param>
        /// <returns></returns>
        IMapperAPI WithMember(string targetMember, Action<IMapperAPIMember> apiMember);

        /// <summary>
        /// Indicates that generated mapped instances must be cached.
        /// Use this option when there are circular references in the target type.
        /// Also, the same target object will be returned for the same source object (it will not be regenerated).
        /// This option comes with a small performance cost.
        /// </summary>
        /// <returns></returns>
        IMapperAPI EnableItemsCache();

        /// <summary>
        /// Indicates that generated mapped instances must not be cached.
        /// Use this option when there are no circular references in the target type.
        /// Also, different target objects will be returned for the same source object.
        /// This option reduces performance cost.
        /// </summary>
        /// <returns></returns>
        IMapperAPI DisableItemsCache();
    }

    /// <summary>
    /// Mapping API Interface (generics).
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    public interface IMapperAPI<TSource, TTarget>
    {
        /// <summary>
        /// Declares a member mapping element.
        /// </summary>
        /// <typeparam name="TProperty">Property type.</typeparam>
        /// <param name="targetMember">Linq expression targetting the target member.</param>
        /// <param name="apiMember">Action delegate in which users can use an instance of IMapperAPIMember.</param>
        /// <returns></returns>
        IMapperAPI<TSource, TTarget> WithMember<TProperty>(Expression<Func<TTarget, TProperty>> targetMember, Action<IMapperAPIMember<TSource, TTarget>> apiMember);

        /// <summary>
        /// Declares a member mapping element.
        /// </summary>
        /// <param name="targetMember">Target member name.</param>
        /// <param name="apiMember">Action delegate in which users can use an instance of IMapperAPIMember.</param>
        /// <returns></returns>
        /// 
        IMapperAPI<TSource, TTarget> WithMember(string targetMember, Action<IMapperAPIMember<TSource, TTarget>> apiMember);

        /// <summary>
        /// Declares a delegate that must be called before the mapping starts.
        /// </summary>
        /// <param name="afterMap"></param>
        /// <returns></returns>
        IMapperAPI<TSource, TTarget> BeforeMap(Action<TSource, TTarget> beforeMap);

        /// <summary>
        /// Declares a delegate that must be called after the mapping is done.
        /// </summary>
        /// <param name="afterMap"></param>
        /// <returns></returns>
        IMapperAPI<TSource, TTarget> AfterMap(Action<TSource, TTarget> afterMap);

        /// <summary>
        /// Indicates that generated mapped instances must be cached.
        /// Use this option when there are circular references in the target type.
        /// Also, the same target object will be returned for the same source object (it will not be regenerated).
        /// This option comes with a small performance cost.
        /// </summary>
        /// <returns></returns>
        IMapperAPI<TSource, TTarget> EnableItemsCache();

        /// <summary>
        /// Indicates that generated mapped instances must not be cached.
        /// Use this option when there are no circular references in the target type.
        /// Also, different target objects will be returned for the same source object.
        /// This option reduces performance cost.
        /// </summary>
        /// <returns></returns>
        IMapperAPI<TSource, TTarget> DisableItemsCache();
    }
}
