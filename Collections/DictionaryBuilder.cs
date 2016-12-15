using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Metadata;
using System.Collections.ObjectModel;
using HMapper.Extensions;

namespace HMapper.Collections
{
    /// <summary>
    /// Will generate an expression that loops through the specified dictionary and maps the elements.
    /// The returned expression will be a Dictionary.
    /// </summary>
    internal class DictionaryBuilder
    {
        /// <summary>
        /// Returns the expression that returns the mapped dictionary.
        /// </summary>
        /// <param name="mapMode"></param>
        /// <param name="targetCollType"></param>
        /// <param name="sourceCollType"></param>
        /// <param name="source"></param>
        /// <param name="includeChain"></param>
        /// <param name="usedBuilders"></param>
        /// <returns></returns>
        public static Expression GetExpression(MapMode mapMode, Type targetCollType, Type sourceCollType, Expression source, Expression includeChain, List<Tuple<Type, Type>> usedBuilders)
        {
            Type sourceType = sourceCollType.GetElementTypeOrType(); // KeyValuePair
            Type targetType = targetCollType.GetElementTypeOrType(); // KeyValuePair
            var targetArgTypes = targetType.GetGenericArguments(); // args of KeyValuePair
            var sourceArgTypes = sourceType.GetGenericArguments(); // args of KeyValuePair
            var dicType = typeof(Dictionary<,>).MakeGenericType(targetArgTypes[0], targetArgTypes[1]);
            
            var dicResultExpr = Expression.Variable(targetCollType);
            var kpExpr = Expression.Variable(typeof(KeyValuePair<,>).MakeGenericType(sourceArgTypes[0], sourceArgTypes[1]));

            var keyMapExpression = PolymorphismManager.GetMostConcreteExpressionCreator(mapMode, Expression.Property(kpExpr, "Key"), targetArgTypes[0], includeChain, usedBuilders);
            var valueMapExpression = PolymorphismManager.GetMostConcreteExpressionCreator(mapMode, Expression.Property(kpExpr, "Value"), targetArgTypes[1], includeChain, usedBuilders);
            if (keyMapExpression == null || valueMapExpression == null) return null;

            var loopContent = Expression.Block(
                Expression.Call(
                dicResultExpr,
                dicType.GetMethod("Add"),
                keyMapExpression,
                valueMapExpression
                )
            );
            
            return Expression.Block(
                new ParameterExpression[] { dicResultExpr, kpExpr },
                Expression.Assign(dicResultExpr, Expression.New(dicType)),
                loopContent.ForEach(
                    source,
                    kpExpr
                ),
                dicResultExpr
            );
        }
    }
}
