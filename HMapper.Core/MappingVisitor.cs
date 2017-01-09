using HMapper.Extensions;
using Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace HMapper
{
    /// <summary>
    /// This class creates an access expression for a closed generic type from a mapping based on an open generic type.    
    /// Remark: the mapping cannot contain real open generic types. Instead, IGeneric type will be used.
    /// </summary>
    internal class MappingVisitor : ExpressionVisitor
    {
        Dictionary<Type, GenericAssociation> _GenericAssociations;
        LambdaExpression _InitialExpression;
        Expression _SourceEntity;
        List<ParameterExpression> _ListConvertedParameters;
        Dictionary<ParameterExpression, Expression> _DicConvertedParameters;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="genericAssociations"></param>
        /// <param name="initialExpression"></param>
        public MappingVisitor(Dictionary<Type, GenericAssociation> genericAssociations, LambdaExpression initialExpression, Expression sourceEntity)
            : this(genericAssociations, initialExpression, sourceEntity, new Dictionary<ParameterExpression, Expression>())
        {
            _DicConvertedParameters[initialExpression.Parameters.First()] = sourceEntity;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="genericAssociations"></param>
        /// <param name="initialExpression"></param>
        /// <param name="sourceEntity"></param>
        /// <param name="dicParameters"></param>
        private MappingVisitor(Dictionary<Type, GenericAssociation> genericAssociations, LambdaExpression initialExpression, Expression sourceEntity, Dictionary<ParameterExpression, Expression> dicParameters)
        {
            _GenericAssociations = genericAssociations;
            _InitialExpression = initialExpression;
            _SourceEntity = sourceEntity;
            _ListConvertedParameters = new List<ParameterExpression>();
            _DicConvertedParameters = dicParameters;
            foreach (var par in initialExpression.Parameters)
            {
                ParameterExpression expr = Expression.Parameter(par.Type.ReplaceGenerics(ReplacementType.Source, genericAssociations));
                _ListConvertedParameters.Add(expr);
                _DicConvertedParameters.Add(par, expr);
            }
        }

        /// <summary>
        /// Method that converts the expression.
        /// </summary>
        /// <returns></returns>
        public Expression Convert()
        {
            var result = Visit(_InitialExpression.Body);
            return result;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression == null) // static members
                return Expression.MakeMemberAccess(null, node.Member.DeclaringType.ReplaceGenerics(ReplacementType.Source, _GenericAssociations).GetMember(node.Member.Name).Single());
            else
            {
                Expression container = Visit(node.Expression);
                MemberInfo member = container.Type.GetMember(node.Member.Name).Single();
                if (container.Type.GetTypeInfo().IsClass)
                {
                    ParameterExpression result = ParameterExpression.Variable(member.PropertyOrFieldType());

                    return Expression.Block(
                        new[] { result },
                        Expression.IfThenElse(
                            Expression.Equal(container, Expression.Constant(null)),
                            Expression.Assign(result, Expression.Default(member.PropertyOrFieldType())),
                            Expression.Assign(result, Expression.MakeMemberAccess(container, member))
                        ),
                        result
                        );
                }
                else
                    return Expression.MakeMemberAccess(container, member);
            }
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            Expression convertedParam;
            if (_DicConvertedParameters.TryGetValue(node, out convertedParam))
                return convertedParam;

            return base.VisitParameter(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Type == typeof(Type))
                return Expression.Constant((node.Value as Type).ReplaceGenerics(ReplacementType.Source, _GenericAssociations));
            return base.VisitConstant(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var methods = node.Method.DeclaringType.ReplaceGenerics(ReplacementType.Source, _GenericAssociations)
                .GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.Name == node.Method.Name).ToArray();

            MethodInfo method = null;
            ParameterInfo[] methodParameters = null;
            Type[] nodeGenericArguments = node.Method.IsGenericMethod ? nodeGenericArguments = node.Method.GetGenericArguments().Select(arg => arg.ReplaceGenerics(ReplacementType.Source, _GenericAssociations)).ToArray() : null;
            if (methods.Length == 1)
            {
                method = methods[0];
                if (method.ContainsGenericParameters)
                    method = method.MakeGenericMethod(nodeGenericArguments);
                methodParameters = method.GetParameters();
            }
            else
            {
                var parametersType = node.Method.GetParameters().Select(p => Tuple.Create(p.Name, p.ParameterType.ReplaceGenerics(ReplacementType.Source, _GenericAssociations))).ToArray();
                foreach (MethodInfo mi in methods)
                {
                    methodParameters = mi.GetParameters();

                    if (node.Method.IsGenericMethod)
                    {
                        if (!mi.IsGenericMethod)
                            continue;
                        if (nodeGenericArguments.Length != mi.GetGenericArguments().Length)
                            continue;
                        
                        MethodInfo miClosed = mi.MakeGenericMethod(nodeGenericArguments);
                        methodParameters = miClosed.GetParameters();
                        if (AreSameParameters(methodParameters, parametersType))
                        {
                            method = miClosed;
                            break;
                        }
                    }
                    else
                    {
                        if (mi.IsGenericMethod)
                            continue;
                        if (AreSameParameters(methodParameters, parametersType))
                        {
                            method = mi;
                            break;
                        }
                    }
                }
                if (method == null) throw new Exception("Critical : No method found");
            }

            // primitives must be boxed if necessary
            List<Expression> arguments = new List<Expression>();
            for (int i=0; i< methodParameters.Length; i++)
            {
                Expression resultArg = Visit(node.Arguments[i]);
                if (resultArg.Type != methodParameters[i].ParameterType)
                    arguments.Add(methodParameters[i].ParameterType.GetTypeInfo().IsValueType ? resultArg.Convert(methodParameters[i].ParameterType) : resultArg);
                else
                    arguments.Add(resultArg);
            }
                
            if (node.Object == null)
                return Expression.Call(
                    method,
                    arguments 
                    );
            var visitedNode = Visit(node.Object);

            if (visitedNode.Type.GetTypeInfo().IsClass)
            {
                ParameterExpression result = ParameterExpression.Variable(method.ReturnType);
                return Expression.Block(
                    new[] { result },
                    Expression.IfThenElse(
                        Expression.Equal(visitedNode, Expression.Constant(null)),
                        Expression.Assign(result, Expression.Default(method.ReturnType)),
                        Expression.Assign(result, Expression.Call(
                            visitedNode,
                            method,
                            arguments
                            ))
                    ),
                    result
                );
            }
            else
            {
                return Expression.Call(
                    visitedNode,
                    method,
                    arguments
                    );
            }
             
        }

        private bool AreSameParameters(ParameterInfo[] parameters, Tuple<string, Type>[] parametersType)
        {
            if (parameters.Length != parametersType.Length) return false;
            for (int i = 0; i < parameters.Length; i++)
                if (parameters[i].Name != parametersType[i].Item1 || parameters[i].ParameterType != parametersType[i].Item2)
                    return false;
            return true;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            string parameters = String.Join(",", node.Constructor.GetParameters().Select(p=>p.Name));
            return Expression.New(
                node.Constructor.DeclaringType.ReplaceGenerics(ReplacementType.Source, _GenericAssociations).GetConstructors().Single(ctor => String.Join(",", ctor.GetParameters().Select(p => p.Name)) == parameters),
                node.Arguments.Select(x => Visit(x))
                );
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var mappingVisitor = new MappingVisitor(_GenericAssociations, node, _SourceEntity, _DicConvertedParameters);
            var arguments = node.Type.GetGenericArguments().Select(arg => arg.ReplaceGenerics(ReplacementType.Source, _GenericAssociations)).ToArray();
            var funcType = node.Type.GetGenericTypeDefinition().MakeGenericType(arguments);
            var funcBody = mappingVisitor.Convert();
            return Expression.Lambda(
                funcType,
                funcBody.Convert(arguments.Last()),
                mappingVisitor._ListConvertedParameters);
        }
    }
}
