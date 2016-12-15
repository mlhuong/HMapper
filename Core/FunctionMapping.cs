using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Metadata;

namespace HMapper
{
    /// <summary>
    /// Mapping to use when dealing with a Func.
    /// </summary>
    internal class FunctionMapping : IMapping
    {
        /// <summary>
        /// Indicates the retrieval mode.
        /// </summary>
        public RetrievalMode RetrievalMode { get; private set; }

        /// <summary>
        /// Expression representing the mapped lambda expression.
        /// </summary>
        public LambdaExpression MappedExpression { get; private set; }

        /// <summary>
        /// Source type.
        /// </summary>
        public Type SourceResultType { get; private set; }

        /// <summary>
        /// Source declaring type.
        /// </summary>
        public Type SourceDeclaringType { get; private set; }

        /// <summary>
        /// Mapping to be ignored.
        /// </summary>
        public bool ToBeIgnored => false;

        /// <summary>
        /// Private Constructor
        /// </summary>
        /// <param name="sourceDeclaringType"></params
        /// <param name="resultType"></param>
        /// <param name="expression"></param>
        /// <param name="retrievalMode"></param>
        private FunctionMapping(Type sourceDeclaringType, Type resultType, LambdaExpression expression, RetrievalMode retrievalMode)
        {
            SourceDeclaringType = sourceDeclaringType;
            SourceResultType = resultType;
            MappedExpression = expression;
            RetrievalMode = retrievalMode;
        }

        /// <summary>
        /// Builds a FunctionMapping given a Func parameter.
        /// </summary>
        /// <param name="function"></param>
        static internal FunctionMapping Create<TSource, TResult>(Expression<Func<TSource, TResult>> function, RetrievalMode retrievalMode)
        {
            return new FunctionMapping(typeof(TSource), typeof(TResult), function, retrievalMode);
        }

        /// <summary>
        /// Returns the Linq expression tree representing the value of the source member.
        /// </summary>
        /// <param name="sourceClosedType">Type of a closed source type</typeparam>
        /// <param name="parameter">Expression parameter.</param>
        /// <param name="closedResultType">Closed type result.</param>
        /// <param name="genericAssociations">Associations between IGenerics and their actual type.</param>
        /// <returns></returns>
        public Expression GetValueExpression(Type sourceClosedType, ParameterExpression parameter, Dictionary<Type,GenericAssociation> genericAssociations)
        {
             return new MappingVisitor(genericAssociations, MappedExpression, parameter).Convert();
        }
    }
}
