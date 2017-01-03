using HMapper.Extensions;
using Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Collections
{
    /// <summary>
    /// Will generate an expression that loops through the specified collection and maps the elements.
    /// The returned expression will be an array.
    /// </summary>
    internal class ArrayBuilder
    {
        /// <summary>
        /// Get the expression that returns the mapped array.
        /// </summary>
        /// <param name="mapMode"></param>
        /// <param name="targetCollType"></param>
        /// <param name="sourceCollType"></param>
        /// <param name="listExpr"></param>
        /// <param name="includeChain"></param>
        /// <param name="usedBuilders"></param>
        /// <returns></returns>
        public static Expression GetExpression(MapMode mapMode, Type targetCollType, Type sourceCollType, Expression listExpr, Expression includeChain, List<Tuple<Type, Type>> usedBuilders)
        {
            Type targetType = targetCollType.GetElementTypeOrType();
            Type sourceType = sourceCollType.GetElementTypeOrType();
            ParameterExpression varSource = Expression.Variable(sourceType);
            ParameterExpression varObjectsArray = Expression.Variable(sourceType.MakeArrayType());
            ParameterExpression varResult = Expression.Variable(targetType.MakeArrayType());
            ParameterExpression varIndex = Expression.Parameter(typeof(int));
            
            var buildExpression = PolymorphismManager.GetMostConcreteExpressionCreator(mapMode, varSource, targetType, includeChain, usedBuilders);
            if (buildExpression == null) return null;

            Expression sourceAsArrayExpr = Expression.Call(
                        Meta.Method(() => Enumerable.ToArray<object>(null)).GetGenericMethodDefinition().MakeGenericMethod(sourceType),
                        listExpr
                    );
            
            Expression loopContent = Expression.Block(
                Expression.Assign(varSource, Expression.ArrayAccess(varObjectsArray, varIndex)),
                Expression.Assign(
                    Expression.ArrayAccess(varResult, varIndex),
                    buildExpression.Convert(targetType)
                )
            );

            return Expression.Block(
                new ParameterExpression[] { varSource, varResult, varObjectsArray },
                Expression.Assign(varObjectsArray, sourceAsArrayExpr),
                Expression.Assign(
                    varResult,
                    Expression.NewArrayBounds(targetType, Expression.Property(varObjectsArray, Meta<Array>.Property(x => x.Length)))
                ),
                loopContent.For(
                    varIndex,
                    Expression.Assign(varIndex, Expression.Constant(0)),
                    Expression.LessThan(varIndex, Expression.Property(varObjectsArray, Meta<Array>.Property(x => x.Length))),
                    Expression.Assign(varIndex, Expression.Increment(varIndex))
                    ),
                varResult
            );

        }
    }
}
