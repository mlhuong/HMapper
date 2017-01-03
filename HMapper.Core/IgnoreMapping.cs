using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMapper
{
    /// <summary>
    /// Mapping to be ignored.
    /// </summary>
    internal class IgnoreMapping : IMapping
    {
        public Type SourceResultType 
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public RetrievalMode RetrievalMode
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool ToBeIgnored => true;

        public Expression GetValueExpression(Type sourceClosedType, ParameterExpression parameter, Dictionary<Type, GenericAssociation> genericAssocations)
        {
            throw new NotImplementedException();
        }
    }
}
