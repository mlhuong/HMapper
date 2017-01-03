using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Exceptions
{
    /// <summary>
    /// Exceptions related to IGenerics
    /// </summary>
    /// <param name="message"></param>
    public class GenericException : Exception
    {
        public GenericException(string message)
            :base(message)
        { }
    }
}
