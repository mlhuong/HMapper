using HMapper.Extensions;
using Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace HMapper
{
    /// <summary>
    /// Cache used to store temporary generated instances.
    /// </summary>
    internal class MapperCache
    {
        [ThreadStatic]
        public static Dictionary<Tuple<object, Type>, object> Cache;

        /// <summary>
        /// Get a mapped object from cache if present. Create it otherwise by using the given delegate.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetType"></param>
        /// <param name="includeChain"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static Expression GetOrSet(Expression source, Type targetType, Expression includeChain, Expression builder)
        {
            object dummy;
            ParameterExpression varResult = Expression.Variable(typeof(object));
            var resultExpression = Expression.Block(
                new[] { varResult },
                Expression.IfThen(Expression.IsFalse(
                    Expression.Call(
                        Expression.Field(null, Meta.Field(() => MapperCache.Cache)),
                        Meta<Dictionary<Tuple<object, Type>, object>>.Method(x => x.TryGetValue(default(Tuple<object, Type>), out dummy)),
                        Expression.Call(
                            null,
                            Meta.Method(() => Tuple.Create<object, Type>(null, default(Type))),
                            source.Convert(typeof(object)),
                            Expression.Constant(targetType)
                            ),
                        varResult
                    )),
                    Expression.Assign(varResult, builder.Convert(typeof(object)))
               ),
               varResult.Convert(targetType)
            );

            return resultExpression;
        }
    }
}
