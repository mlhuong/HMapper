using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace HMapper
{
    /// <summary>
    /// API used to configure the mappings.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    internal class MapperAPI<TSource, TTarget> : IMapperAPI<TSource, TTarget>
    {
        internal MapInfo _MapInfo;

        internal MapperAPI(MapInfo mapInfo)
        {
            _MapInfo = mapInfo;
        }

        /// <summary>
        /// Manages a member of the target type by providing a IMapperAPIMember interface.
        /// </summary>
        /// <typeparam name="TProperty">Type of property to manage.</typeparam>
        /// <param name="memberExpression">Linq expression representing the property to manage.</param>
        /// <param name="apiMember">Action providing a IMapperAPIMember interface.</param>
        /// <returns></returns>
        public IMapperAPI<TSource, TTarget> WithMember<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Action<IMapperAPIMember<TSource, TTarget>> apiMember)
        {
            MemberExpression memberExpr = memberExpression.Body as MemberExpression;
            if (memberExpr == null)
                throw new Exception("The first argument of WithMember() method must be a member expression.");
            apiMember(new MapperAPIMember<TSource, TTarget>(this, memberExpr.Member));
            return this;
        }

        /// <summary>
        /// Declares a member mapping element.
        /// </summary>
        /// <param name="targetMember">Target member name.</param>
        /// <param name="apiMember">Action delegate in which users can use an instance of IMapperAPIMember.</param>
        /// <returns></returns>
        public IMapperAPI<TSource, TTarget> WithMember(string targetMember, Action<IMapperAPIMember<TSource, TTarget>> apiMember)
        {
            var members = typeof(TTarget).GetMember(targetMember, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (members.Length == 0)
                throw new Exception(String.Format("No member with name {0} can be found in type {1}.", targetMember, typeof(TTarget).FullName));

            if (members.Length > 1)
                throw new Exception(String.Format("More than one member with name {0} can be found in type {1}.", targetMember, typeof(TTarget).FullName));

            apiMember(new MapperAPIMember<TSource, TTarget>(this, members[0]));
            return this;
        }

        /// <summary>
        /// Declares a delegate that must be called before the mapping starts.
        /// </summary>
        /// <param name="afterMap"></param>
        /// <returns></returns>
        public IMapperAPI<TSource, TTarget> BeforeMap(Action<TSource, TTarget> beforeMap)
        {
            _MapInfo.BeforeMaps.Add(beforeMap);
            return this;
        }

        /// <summary>
        /// Declares a delegate that must be called after the mapping is done.
        /// </summary>
        /// <param name="afterMap"></param>
        /// <returns></returns>
        public IMapperAPI<TSource, TTarget> AfterMap(Action<TSource, TTarget> afterMap)
        {
            _MapInfo.AfterMaps.Add(afterMap);
            return this;
        }

        /// <summary>
        /// Indicates that generated mapped instances must be cached.
        /// Use this option when there are circular references in the target type.
        /// Also, the same target object will be returned for the same source object (it will not be regenerated).
        /// This option comes with a small performance cost.
        /// </summary>
        /// <returns></returns>
        public IMapperAPI<TSource, TTarget> EnableItemsCache()
        {
            _MapInfo.UseItemsCache = true;
            return this;
        }

        /// <summary>
        /// Indicates that generated mapped instances must not be cached.
        /// Use this option when there are no circular references in the target type.
        /// Also, different target objects will be returned for the same source object.
        /// This option reduces performance cost.
        /// </summary>
        /// <returns></returns>
        public IMapperAPI<TSource, TTarget> DisableItemsCache()
        {
            _MapInfo.UseItemsCache = false;
            return this;
        }
    }
}
