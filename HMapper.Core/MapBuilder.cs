using HMapper.Extensions;
using Metadata;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Runtime.CompilerServices;

namespace HMapper
{
    /// <summary>
    /// Class generating, or filling the instance of the desired target type.
    /// </summary>
    internal class MapBuilder
    {
        public delegate Expression MapExpressionDlg(MapMode mapMode, Type targetType, Type sourceType, Expression sourceExpr, Expression includeChain, List<Tuple<Type, Type>> usedBuilders);

        /// <summary>
        /// Generate the expression that creates or fills the instance of the desired target type.
        /// The type must be a closed type and must be the most concrete type.
        /// </summary>
        /// <param name="mapMode"></param>
        /// <param name="mapInfo"></param>
        /// <param name="genericTypeAssociation"></param>
        /// <param name="paramSource"></param>
        /// <param name="varResult"></param>
        /// <param name="paramIncludeChain"></param>
        /// <param name="usedBuilders"></param>
        /// <returns></returns>
        internal static Expression CreateBuilderExpression(MapMode mapMode, MapInfo mapInfo, Dictionary<Type, GenericAssociation> genericTypeAssociation, Expression paramSource, Expression varResult, Expression paramIncludeChain, List<Tuple<Type, Type>> usedBuilders=null)
        {
            var targetType = mapInfo.TargetType.ReplaceGenerics(ReplacementType.Target, genericTypeAssociation);
            var sourceType = mapInfo.SourceType.ReplaceGenerics(ReplacementType.Source, genericTypeAssociation);
            
            // Generation of the CreateInstance function
            ParameterExpression varEntityRealType = Expression.Variable(sourceType);
            ParameterExpression varIncludeChain = Expression.Variable(typeof(IncludeChain));
            ParameterExpression varToBeCreated = Expression.Variable(typeof(bool));
            var parameters = new List<ParameterExpression> { varEntityRealType, varIncludeChain, varToBeCreated };

            // List of expressions for the actual creation / modification of the target object
            List<Expression> mainExpressions = new List<Expression>();
            

            // Manual mapping
            if (mapInfo.ManualBuilder != null)
            {
                return Expression.Block(
                        new ParameterExpression[] { varEntityRealType },
                        Expression.Assign(varEntityRealType, paramSource.Convert(sourceType)),
                        new MappingVisitor(genericTypeAssociation, mapInfo.ManualBuilder, varEntityRealType).Convert()
                        );
            }

            // Expression mapping
            if (mapInfo.ExpressionBuilder != null)
            {
                varResult = Expression.Parameter(targetType);
                var resultExpr = mapInfo.ExpressionBuilder(mapMode, targetType, sourceType, paramSource, paramIncludeChain, usedBuilders.ToList());
                if (resultExpr == null) return null;
                if (mapInfo.UseItemsCache)
                {
                    return Expression.Block(
                        new ParameterExpression[] { (ParameterExpression)varResult },
                        Expression.Assign(varResult, resultExpr),
                        AddToCache(paramSource, varResult),
                        varResult
                        );
                }
                else return resultExpr;
            }

            if (usedBuilders == null)
                usedBuilders = new List<Tuple<Type, Type>>() { Tuple.Create(sourceType, targetType) };
            else
                usedBuilders.Add(Tuple.Create(sourceType, targetType));

            mainExpressions.Add(Expression.Assign(varEntityRealType, paramSource.Convert(sourceType)));

            if (varResult == null)
            {
                varResult = Expression.Parameter(targetType);
                parameters.Add((ParameterExpression)varResult);
                mainExpressions.Add(Expression.Assign(
                varResult,
                targetType.CreateInstance()
                ));
            }

            //Adding to cache
            if (mapInfo.UseItemsCache)
                mainExpressions.Add(AddToCache(varEntityRealType, varResult));

            // Call BeforeMap
            foreach (var beforeMap in mapInfo.BeforeMaps)
            {
                var typeAction = typeof(Action<,>).MakeGenericType(beforeMap.GetMethodInfo().GetParameters().Select(p=>p.ParameterType).ToArray());
                mainExpressions.Add(Expression.Call(
                    Expression.TypeAs(Expression.Constant(beforeMap), typeAction),
                    typeAction.GetMethod("Invoke"),
                    paramSource.Convert(sourceType),
                    varResult
                    ));
            }

            var closedTargetMembers = targetType.GetMembers().Where(x => (x is FieldInfo || x is PropertyInfo));

            // Loop through the members to assign the values individually
            foreach (var kpMember in mapInfo.Members)
            {
                // Ignored mappings
                if (kpMember.Value.ToBeIgnored)
                    continue;
                var memberMemberInfo = varResult.Type.GetMember(kpMember.Key.Name);
                if (memberMemberInfo.Length == 0) // Can happen when specified target object is less concrete than target type of MapInfo. 
                    continue;

                Expression targetMemberExpression = kpMember.Key is FieldInfo ? Expression.Field(varResult, kpMember.Key.Name) : Expression.Property(varResult, kpMember.Key.Name);

                Expression entityMember = kpMember.Value.GetValueExpression(sourceType, varEntityRealType, genericTypeAssociation);
                MemberInfo targetMemberInfo = closedTargetMembers.Single(x => x.Name == kpMember.Key.Name);
                Expression assignExpr;
                
                Type targetMemberType = targetMemberInfo.PropertyOrFieldType();

                Expression includeExpr = mapMode == MapMode.All ? paramIncludeChain : varIncludeChain;

                Expression GetTargetMemberExpr = PolymorphismManager.GetMostConcreteExpressionCreator(mapMode, entityMember, targetMemberType, includeExpr, usedBuilders);
                if (GetTargetMemberExpr == null)
                    continue;
                var exceptionParam = Expression.Parameter(typeof(Exception));
                var exceptionMsg = Expression.Call(Meta.Method(() => String.Format("", new object[0])),
                    Expression.Constant("Mapper exception while assigning [{0}] of [{1}]."),
                    Expression.NewArrayInit(typeof(object), Expression.Constant(kpMember.Key.Name), Expression.Constant(mapInfo.TargetType.Name)));
                var newExpceptionExpr = Expression.New(
                    typeof(Exception).GetConstructor(new[] { typeof(string), typeof(Exception) }),
                    exceptionMsg,
                    exceptionParam);
                assignExpr = Expression.TryCatch(
                    Expression.Block(typeof(void), Expression.Assign(targetMemberExpression, GetTargetMemberExpr)),
                    Expression.Catch(exceptionParam, Expression.Throw(newExpceptionExpr))
                );

                IncludeChain dummyChain;

                if (mapMode != MapMode.All
                    && (kpMember.Value.RetrievalMode == RetrievalMode.RetrievedWhenSpecified || (kpMember.Value.RetrievalMode == RetrievalMode.Default && (!targetMemberType.IsSimpleType()) && (targetMemberType.GetTypeInfo().IsClass || targetMemberType.GetTypeInfo().IsInterface))))
                {
                    mainExpressions.Add(Expression.Assign(varToBeCreated, Expression.Constant(false)));
                    mainExpressions.Add(Expression.Assign(varIncludeChain, paramIncludeChain));
                    var tryGetValueExpr = Expression.Call(
                                    Expression.Property(paramIncludeChain, Meta<IncludeChain>.Property(x => x.Includes)),
                                    Meta<Dictionary<string, IncludeChain>>.Method(x => x.TryGetValue(null, out dummyChain)),
                                    Expression.Constant(kpMember.Key.Name),
                                    varIncludeChain
                                );
                    if (mapMode == MapMode.Include)
                    {
                        mainExpressions.Add(
                            Expression.Assign(
                                    varToBeCreated,
                                    tryGetValueExpr
                                )
                        );
                    }
                    else
                    {
                        mainExpressions.Add(Expression.IfThenElse(
                            tryGetValueExpr,
                            Expression.Assign(
                                varToBeCreated,
                                Expression.NotEqual(varIncludeChain, Expression.Constant(IncludeChain.NullValue))),
                            Expression.Block(
                                Expression.Assign(varIncludeChain, Expression.Constant(IncludeChain.NullValue)),
                                Expression.Assign(varToBeCreated, Expression.Constant(true)))
                            ));
                    }
                    mainExpressions.Add(
                        Expression.IfThen(
                            Expression.IsTrue(varToBeCreated),
                            assignExpr
                        )
                    );
                }
                else
                    mainExpressions.Add(assignExpr);

            } // end foreach (member..)

            // Call AfterMap
            foreach (var afterMap in mapInfo.AfterMaps)
            {
                var typeAction = typeof(Action<,>).MakeGenericType(sourceType, targetType);
                mainExpressions.Add(Expression.Call(
                    Expression.TypeAs(Expression.Constant(afterMap), typeAction),
                    typeAction.GetMethod("Invoke"),
                    paramSource.Convert(sourceType),
                    varResult
                    ));
            }

            mainExpressions.Add(varResult);
            
            var result = Expression.Block(parameters, mainExpressions);
            return result;
        }

        /// <summary>
        /// Returns an expression representing the addition of a target result to the cache.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static Expression AddToCache(Expression source, Expression target)
        {
            //return Expression.Call(
            //        Expression.Field(null, Meta.Field(() => MapperCache.Cache)),
            //        Meta<Dictionary<Tuple<object, Type>, object>>.Method(x => x.Add(default(Tuple<object, Type>), null)),
            //        Expression.Call(
            //            null,
            //            Meta.Method(() => Tuple.Create<object, Type>(null, default(Type))),
            //            source.Convert(typeof(object)),
            //            Expression.Constant(target.Type)
            //        ),
            //        target.Convert(typeof(object))
            //        );
            return Expression.Call(
                    Meta.Method(() => DictionaryExtension.TryAdd<Tuple<object, Type>, object>(null,null,null)),
                    Expression.Field(null, Meta.Field(() => MapperCache.Cache)),
                    Expression.Call(
                        null,
                        Meta.Method(() => Tuple.Create<object, Type>(null, default(Type))),
                        source.Convert(typeof(object)),
                        Expression.Constant(target.Type)
                    ),
                    target.Convert(typeof(object))
                );
        }
    }
}
