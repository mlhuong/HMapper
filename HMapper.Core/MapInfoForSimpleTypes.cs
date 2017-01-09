using HMapper.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HMapper
{
    internal class MapInfoForSimpleTypes : MapInfo
    {
        static ConcurrentDictionary<Tuple<Type, Type>, MapInfoForSimpleTypes> _Cache = new ConcurrentDictionary<Tuple<Type, Type>, MapInfoForSimpleTypes>();
        internal MapInfoForSimpleTypes(Tuple<Type, Type> tuple) : base(tuple, false)
        {
            ExpressionBuilder = GetExpression;
        }

        static Expression GetExpression(MapMode mapMode, Type targetType, Type sourceType, Expression sourceExpr, Expression includeChain, List<Tuple<Type, Type>> usedBuilders)
        {
            if (targetType.IsAssignableFrom(sourceType)) return sourceExpr.Convert(targetType);
            
            // Nullable types
            if (sourceType.GetTypeInfo().IsGenericType && sourceType.GetGenericTypeDefinition()==typeof(Nullable<>))
                return Expression.Call(sourceExpr, sourceType.GetMethod("GetValueOrDefault", Type.EmptyTypes));

            return null;
        }

        internal static MapInfoForSimpleTypes Get(Tuple<Type, Type> tuple)
        {
            return _Cache.GetOrAdd(tuple, t => new MapInfoForSimpleTypes(t));
        }
    }
}
