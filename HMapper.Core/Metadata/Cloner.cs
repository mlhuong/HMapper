using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Metadata
{
    /// <summary>
    /// Cloner class.
    /// </summary>
    /// <typeparam name="TSource">Source type.</typeparam>
    public class Cloner<TSource>
    {
        /// <summary>
        /// Delegate used to clone.
        /// </summary>
        public static readonly Action<TSource, TSource> CloneTo;

        static Cloner()
        {
            var paramSource = Expression.Parameter(Meta<TSource>.Type);
            var paramTarget = Expression.Parameter(Meta<TSource>.Type);
            List<Expression> bodyExpressions = new List<Expression>();
            
            foreach (FieldInfo field in Meta<TSource>.Type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                bodyExpressions.Add(
                    Expression.Assign(
                        Expression.Field(paramTarget, field),
                        Expression.Field(paramSource, field)
                    )
                );
            }
            Expression body = Expression.Block(bodyExpressions.ToArray());

            CloneTo = Expression.Lambda<Action<TSource, TSource>>(
                        body,
                        paramSource,
                        paramTarget
                        ).Compile();
        }
    }
}
