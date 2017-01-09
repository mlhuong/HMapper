using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace HMapper
{
    /// <summary>
    /// API managing a target member.
    /// </summary>
    /// <typeparam name="TSource">Source type.</typeparam>
    /// <typeparam name="TTarget">Target type.</typeparam>
    internal class MapperAPIMember<TSource, TTarget> : IMapperAPIMember<TSource, TTarget>
    {
        MapperAPI<TSource, TTarget> _MapperAPI;
        MemberInfo _TargetMember;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapperAPI"></param>
        /// <param name="targetMember"></param>
        internal MapperAPIMember(MapperAPI<TSource, TTarget> mapperAPI, MemberInfo targetMember)
        {
            _MapperAPI = mapperAPI;
            _TargetMember = targetMember;
        }

        /// <summary>
        /// Link the current target member to the specified source member.
        /// </summary>
        /// <typeparam name="TSourceProperty">Type of the source property.</typeparam>
        /// <param name="sourceMember">Expression representing the source property.</param>
        /// <param name="retrievalMode">Indicates the retrieval mode.</param>
        /// <returns></returns>
        public IMapperAPIMember<TSource, TTarget> LinkTo<TSourceProperty>(Expression<Func<TSource, TSourceProperty>> sourceMember, RetrievalMode retrievalMode)
        {
            if (_MapperAPI._MapInfo.Members.ContainsKey(_TargetMember))
                throw new Exception(String.Format("The target member {0} of type {1} is already mapped.", _TargetMember.Name, typeof(TTarget).FullName));

            MemberExpression memberExpr = sourceMember.Body as MemberExpression;
            if (memberExpr == null || !(memberExpr.Expression is ParameterExpression))
                _MapperAPI._MapInfo.Members.Add(_TargetMember, FunctionMapping.Create(sourceMember, retrievalMode));
            else
                _MapperAPI._MapInfo.Members.Add(_TargetMember, new MemberMapping(memberExpr.Member, retrievalMode));
            return this;
        }

        /// <summary>
        /// Links the member to the specified source member specified by its name.
        /// </summary>
        /// <param name="sourceMember">Source member name.</param>
        /// <param name="retrievalMode">Indicates the retrieval mode of the member</param>
        /// <returns></returns>
        public IMapperAPIMember<TSource, TTarget> LinkTo(string sourceMember, RetrievalMode retrievalMode)
        {
            if (_MapperAPI._MapInfo.Members.ContainsKey(_TargetMember))
                throw new Exception(String.Format("The target member {0} of type {1} is already mapped.", _TargetMember.Name, _TargetMember.DeclaringType.FullName));

            var members = typeof(TSource).GetMember(sourceMember, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (members.Length == 0)
                throw new Exception(String.Format("No member with name {0} can be found in type {1}.", sourceMember, typeof(TSource).FullName));

            if (members.Length > 1)
                throw new Exception(String.Format("More than one member with name {0} can be found in type {1}.", sourceMember, typeof(TSource).FullName));

            _MapperAPI._MapInfo.Members.Add(_TargetMember, new MemberMapping(members[0], retrievalMode));
            return this;
        }

        /// <summary>
        /// Ignore the target member.
        /// </summary>
        /// <returns></returns>
        public IMapperAPIMember<TSource, TTarget> Ignore()
        {
            _MapperAPI._MapInfo.Members.Add(_TargetMember, new IgnoreMapping());
            return this;
        }
    }
}
