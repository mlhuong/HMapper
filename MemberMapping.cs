using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Metadata;
using System.Linq.Expressions;

namespace HMapper
{
    /// <summary>
    /// Mapping to use when dealing with a class member. 
    /// </summary>
    internal class MemberMapping : IMapping
    {
        /// <summary>
        /// Memberinfo contained in this member mapping.
        /// </summary>
        public MemberInfo Member { get; private set; }

        /// <summary>
        /// Source result type.
        /// </summary>
        public Type SourceResultType { get { return Member.PropertyOrFieldType(); } }

        /// <summary>
        /// Source declaring type.
        /// </summary>
        public Type SourceDeclaringType { get { return Member.DeclaringType; } }

        public bool ToBeIgnored => false;

        public RetrievalMode RetrievalMode { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="member"></param>
        internal MemberMapping(MemberInfo member, RetrievalMode retrievalMode)
        {
            Member = member;
            RetrievalMode = retrievalMode;
        }

        /// <summary>
        /// Returns the Linq expression tree representing the value of the source member.
        /// </summary>
        /// <param name="sourceClosedType">Source type.</param>
        /// <param name="parameter">Expression parameter.</param>
        /// <param name="closedResultType">Closed type result.n</param>
        /// <param name="genericAssocations">Associations between IGenerics and their actual type.</param>
        /// <returns></returns>
        public Expression GetValueExpression(Type sourceClosedType, ParameterExpression parameter, Dictionary<Type, GenericAssociation> genericAssocations)
        {
            return Expression.PropertyOrField(parameter, Member.Name);
        }

    }
}
