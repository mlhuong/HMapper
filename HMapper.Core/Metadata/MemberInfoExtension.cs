using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Metadata
{
    /// <summary>
    /// Extensions methods for MemberInfo class.
    /// </summary>
    public static class MemberInfoExtension
    {
        /// <summary>
        /// Returns the type of the specified MemberInfo when it is a field or a property.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static Type PropertyOrFieldType (this MemberInfo member)
        {
            PropertyInfo prop = member as PropertyInfo;
            if (prop != null) return prop.PropertyType;

            FieldInfo field = member as FieldInfo;
            if (field != null) return field.FieldType;

            throw new Exception($"PropertyOrFieldType can be only used on a field or property. {member.Name} is a {member.GetType()}.");
        }
    }
}
