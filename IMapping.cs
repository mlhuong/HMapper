using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HMapper
{
    /// <summary>
    /// Interface used to represent a mapping.
    /// </summary>
    public interface IMapping
    {
        /// <summary>
        /// Indicates the retrieval mode.
        /// </summary>
        RetrievalMode RetrievalMode { get; }

        /// <summary>
        /// Indicates that the target member must be ignored.
        /// </summary>
        bool ToBeIgnored { get; }

        /// <summary>
        /// Source result type.
        /// </summary>
        Type SourceResultType { get; }

        /// <summary>
        /// Returns the Linq expression tree representing the value of the source member.
        /// </summary>
        /// <param name="sourceClosedType">Type of a closed source type</typeparam>
        /// <param name="parameter">Expression parameter.</param>
        /// <param name="closedResultType">Closed type result.n</param>
        /// <param name="genericAssocations">Associations between IGenerics and their actual type.</param>
        /// <returns></returns>
        Expression GetValueExpression(Type sourceClosedType, ParameterExpression parameter, Dictionary<Type, GenericAssociation> genericAssocations);
    }
}
