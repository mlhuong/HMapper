using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Exceptions
{
    /// <summary>
    /// Exeption thrown when a type is not mapped.
    /// </summary>
    public class NotMappedException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public NotMappedException(Type sourceType, Type targetType) :
            base($"Target type {targetType.FullName} is not mapped to source type {sourceType.FullName}.")
        {}
    }
}
