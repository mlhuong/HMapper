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
    internal class MapperAPIMember : IMapperAPIMember
    {
        MapperAPI _MapperAPI;
        MemberInfo _TargetMember;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapperAPI"></param>
        /// <param name="targetMember"></param>
        internal MapperAPIMember(MapperAPI mapperAPI, MemberInfo targetMember)
        {
            _MapperAPI = mapperAPI;
            _TargetMember = targetMember;
        }

        /// <summary>
        /// Link the current target member to the specified source member.
        /// </summary>
        /// <param name="sourceMember">Source member.</param>
        /// <param name="inclusionManaged">Indicates if this member is managed by inclusion specification.</param>
        /// <returns></returns>
        public IMapperAPIMember LinkTo(string sourceMember, RetrievalMode retrievalMode)
        {           
            if (_MapperAPI._MapInfo.Members.ContainsKey(_TargetMember))
                throw new Exception(String.Format("The target member {0} of type {1} is already mapped.", _TargetMember.Name, _TargetMember.DeclaringType.FullName));

            var members = _MapperAPI._SourceType.GetMember(sourceMember, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (members.Length == 0)
                throw new Exception(String.Format("No member with name {0} can be found in type {1}.", sourceMember, _MapperAPI._SourceType.FullName));

            if (members.Length > 1)
                throw new Exception(String.Format("More than one member with name {0} can be found in type {1}.", sourceMember, _MapperAPI._SourceType.FullName));

            _MapperAPI._MapInfo.Members.Add(_TargetMember, new MemberMapping(members[0], retrievalMode));
            return this;
        }

        /// <summary>
        /// Ignore the target member.
        /// </summary>
        /// <returns></returns>
        public IMapperAPIMember Ignore()
        {
            _MapperAPI._MapInfo.Members.Add(_TargetMember, new IgnoreMapping());
            return this;
        }
    }
}
