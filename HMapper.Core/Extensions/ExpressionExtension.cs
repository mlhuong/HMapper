using Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Extensions
{
    public static class ExpressionExtension
    {
        /// <summary>
        /// Convert an expression to the specified type.
        /// Returns TypeAs if the specified type is a class, the source expression if same type, Convert otherwise.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Expression Convert(this Expression expression, Type type)
        {
            if (type==expression.Type) return expression;
            if (type.GetTypeInfo().IsClass) return Expression.TypeAs(expression, type);
            return Expression.Convert(expression, type);
        }

        /// <summary>
        /// Get the expression that does a "For" loop.
        /// </summary>
        /// <param name="loopVar"></param>
        /// <param name="initValue"></param>
        /// <param name="condition"></param>
        /// <param name="increment"></param>
        /// <param name="loopContent"></param>
        /// <returns></returns>
        public static Expression For(this Expression loopContent, ParameterExpression loopVar, Expression initValue, Expression condition, Expression increment)
        {
            var initAssign = Expression.Assign(loopVar, initValue);

            var breakLabel = Expression.Label("LoopBreak");

            var loop = Expression.Block(new[] { loopVar },
                initAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        condition,
                        Expression.Block(
                            loopContent,
                            increment
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );

            return loop;
        }

        public static Expression ForEach(this Expression loopContent, Expression collection, ParameterExpression loopVar)
        {
            var getEnumeratorExpr = Expression.Call(collection, collection.Type.GetInheritedMember("GetEnumerator") as MethodInfo);
            var enumeratorType = getEnumeratorExpr.Type;
            var enumeratorVar = Expression.Variable(enumeratorType);
            var breakLabel = Expression.Label("LoopBreak");

            var loop = Expression.Block(new[] { enumeratorVar },
                Expression.Assign(enumeratorVar, getEnumeratorExpr),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Equal(Expression.Call(enumeratorVar, enumeratorType.GetInheritedMember("MoveNext") as MethodInfo), Expression.Constant(true)),
                        Expression.Block(
                            Expression.Assign(loopVar, Expression.Property(enumeratorVar, "Current").Convert(loopVar.Type)),
                            loopContent
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );

            return loop;
        }
    }
}
