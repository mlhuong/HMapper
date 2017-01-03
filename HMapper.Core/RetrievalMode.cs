using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper
{
    /// <summary>
    /// Retrieval mode
    /// Default means simple types are always retieved, and mapped type and collections will be retrieved when contained in inclusion expression.
    /// </summary>
    public enum RetrievalMode
    {
        Default = 0,
        RetrievedWhenSpecified = 1,
        AlwaysRetrieved = 2
    }
}
