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
    internal class BaseCollectionBuilder
    {
        public static Expression GetExpression(Type openConstructorType, MapMode mapMode, Type targetCollType, Type sourceCollType, Expression listExpr, Expression includeChain, List<Tuple<Type, Type>> usedBuilders)
        {
            Type sourceType = sourceCollType.GetElementTypeOrType();
            Type targetType = targetCollType.GetElementTypeOrType();
            var closedConstructorType = openConstructorType.MakeGenericType(targetType);
            var resultExpr = Expression.Variable(targetCollType);
            var itemExpr = Expression.Variable(sourceType);

            var itemMapExpression = PolymorphismManager.GetMostConcreteExpressionCreator(mapMode, itemExpr, targetType, includeChain, usedBuilders);
            if (itemMapExpression == null) return null;

            var loopContent = Expression.Block(
                Expression.Call(
                resultExpr,
                closedConstructorType.GetTypeInfo().GetMethod("Add"),
                itemMapExpression
                )
            );
            var result = Expression.Block(
                new ParameterExpression[] { resultExpr, itemExpr },
                Expression.Assign(resultExpr, Expression.New(closedConstructorType)),
                loopContent.ForEach(
                    listExpr,
                    itemExpr
                ),
                resultExpr
            );
            return result;

        }
    }
}
