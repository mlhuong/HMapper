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
    internal class MapperAPI : IMapperAPI
    {
        internal MapInfo _MapInfo;
        internal Type _SourceType;
        internal Type _TargetType;

        internal MapperAPI(MapInfo mapInfo)
        {
            _MapInfo = mapInfo;
        }

        /// <summary>
        /// Manages a member of the target type by providing a IMapperAPIMember interface.
        /// </summary>
        /// <param name="memberName">Member name of the target type.</param>
        /// <param name="apiMember">Action providing a IMapperAPIMember interface.</param>
        /// <returns></returns>
        public IMapperAPI WithMember(string memberName, Action<IMapperAPIMember> apiMember)
        {
            var members = _TargetType.GetMember(memberName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (members.Length==0)
                throw new Exception(String.Format("No member with name {0} can be found in type {1}.", memberName, _TargetType.FullName));

            if (members.Length>1)
                throw new Exception(String.Format("More than one member with name {0} can be found in type {1}.", memberName, _TargetType.FullName));

            apiMember(new MapperAPIMember(this, members[0]));
            return this;
        }
        
        /// <summary>
        /// Indicates that generated mapped instances must be cached.
        /// Use this option when there are circular references in the target type.
        /// Also, the same target object will be returned for the same source object (it will not be regenerated).
        /// This option comes with a small performance cost.
        /// </summary>
        /// <returns></returns>
        public IMapperAPI EnableItemsCache()
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
        public IMapperAPI DisableItemsCache()
        {
            _MapInfo.UseItemsCache = false;
            return this;
        }
    }
}
