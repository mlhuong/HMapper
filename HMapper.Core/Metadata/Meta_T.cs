using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Metadata
{
    /// <summary>
    /// Defines method helpers to ease and speed up reflection jobs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Meta<T>
    {
        static private readonly Dictionary<string, Func<T, object>> DELEGATES_GET;
        static private readonly Dictionary<string, Action<T, object>> DELEGATES_SET;
        static private readonly Dictionary<string, Func<object, object[], object>> DELEGATES_METHOD;
        /// <summary>
        /// Type.
        /// </summary>
        static public readonly Type Type;
        private const BindingFlags INSTANCE_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags STATIC_BINDING_FLAGS = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly object MUTEX;

        /// <summary>
        /// Statics
        /// </summary>
        static Meta()
        {
            Type = typeof(T);
            DELEGATES_GET = new Dictionary<string, Func<T, object>>();
            DELEGATES_SET = new Dictionary<string, Action<T, object>>();
            DELEGATES_METHOD = new Dictionary<string, Func<object, object[], object>>();
            MUTEX = new object();
        }

        /// <summary>
        /// Protected construction.
        /// </summary>
        protected Meta()
        {
        }

        /// <summary>
        /// Get the value of the specified member (Property or Field)
        /// </summary>
        /// <param name="obj">Instance de l'objet dont on recherche la valeur de la propriété</param>
        /// <param name="memberName"></param>
        /// <returns>La valeur de la propriété de l'objet.</returns>
        public static object GetValue(T obj, string memberName)
        {
            Func<T, object> getFunc;
            lock (MUTEX)
            {
                DELEGATES_GET.TryGetValue(memberName, out getFunc);
                if (getFunc == null)
                {
                    ParameterExpression paramInstance = Expression.Parameter(Type, "instance");
                    getFunc = Expression.Lambda<Func<T, object>>(
                        Expression.Convert(
                            Expression.PropertyOrField(paramInstance, memberName),
                            typeof(object)),
                            paramInstance
                        ).Compile();
                    DELEGATES_GET.Add(memberName, getFunc);
                }
            }

            return getFunc(obj);
        }

        /// <summary>
        /// Set the value of the specified member (Property or Field)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        public static void SetValue(T obj, string memberName, object value)
        {
            Action<T, object> setAction;
            lock (MUTEX)
            {
                DELEGATES_SET.TryGetValue(memberName, out setAction);
                if (setAction == null)
                {
                    ParameterExpression paramInstance = Expression.Parameter(Type, "instance");
                    ParameterExpression paramValue = Expression.Parameter(typeof(object), "val");
                    MemberInfo member = Type.GetMember(memberName, INSTANCE_BINDING_FLAGS)[0];
                    setAction = Expression.Lambda<Action<T, object>>(
                        Expression.Assign(
                            Expression.PropertyOrField(paramInstance, memberName),
                            Expression.Convert(paramValue, (member is PropertyInfo) ? (member as PropertyInfo).PropertyType : (member as FieldInfo).FieldType)
                        ),
                        paramInstance,
                        paramValue
                        ).Compile();
                    DELEGATES_SET.Add(memberName, setAction);
                }
            }

            setAction(obj, value);
        }

        /// <summary>
        /// Gets the MethodInfo designed by the lambda expression
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="binding"></param>
        /// <returns></returns>
        public static MethodInfo Method(Expression<Action<T>> expression, BindingFlags binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            MethodCallExpression callExpr = expression.Body as MethodCallExpression;
            if (callExpr != null)
                return callExpr.Method;

            throw new Exception("Lamda expression not supported for Meta<T>.Method() method.");
        }

        /// <summary>
        /// Gets the FieldInfo designed by the lambda expression
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="expression"></param>
        /// <param name="binding"></param>
        /// <returns></returns>
        public static FieldInfo Field<U>(Expression<Func<T, U>> expression, BindingFlags binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            MemberExpression memberExpr = expression.Body as MemberExpression;
            if (memberExpr != null)
                return memberExpr.Member as FieldInfo;

            throw new Exception("Lamda expression not supported for Meta<T>.Field() method.");
        }

        /// <summary>
        /// Gets the PropertyInfo designed by the lambda expression
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="expression"></param>
        /// <param name="binding"></param>
        /// <returns></returns>
        public static PropertyInfo Property<U>(Expression<Func<T, U>> expression, BindingFlags binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            MemberExpression memberExpr = expression.Body as MemberExpression;
            if (memberExpr != null)
                return memberExpr.Member as PropertyInfo;

            throw new Exception("Lamda expression not supported for Meta<T>.Property() method.");
        }

        /// <summary>
        /// Invoke a method using a delegate
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="genericTypes"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static object InvokeMethod(T obj, string methodName, Type[] genericTypes, params object[] arguments)
        {
            Func<object, object[], object> methodFunc;
            string key = genericTypes == null ? methodName : methodName + string.Join("_", genericTypes.Select(x => x.Name));
            lock (MUTEX)
            {
                DELEGATES_METHOD.TryGetValue(key, out methodFunc);
                if (methodFunc == null)
                {
                    ParameterExpression paramInstance = Expression.Parameter(typeof(object));
                    ParameterExpression paramArgs = Expression.Parameter(typeof(object[]));

                    MethodInfo methodInfo = Type.GetMethod(methodName, obj==null ? STATIC_BINDING_FLAGS : INSTANCE_BINDING_FLAGS);
                    if (methodInfo.IsGenericMethodDefinition)
                    {
                        if (genericTypes == null || methodInfo.GetGenericArguments().Length != genericTypes.Length)
                            throw new Exception("The specified types and the generic types of the method don't match");
                        methodInfo = methodInfo.MakeGenericMethod(genericTypes);
                    }
                    List<Expression> body = new List<Expression>();
                    Expression exprInstance = obj == null ? null : Expression.TypeAs(paramInstance, Type);

                    if (methodInfo.ReturnType == typeof(void))
                    {
                        body.Add(
                            Expression.Call(
                                exprInstance,
                                methodInfo,
                                methodInfo.GetParameters().Select((p, index) => Expression.Convert(Expression.ArrayIndex(paramArgs, Expression.Constant(index)), p.ParameterType)).ToArray()
                            ));
                        body.Add(Expression.Constant(null));
                    }
                    else
                    {
                        body.Add(Expression.Convert(
                            Expression.Call(
                                exprInstance,
                                methodInfo,
                                methodInfo.GetParameters().Select((p, index) => Expression.Convert(Expression.ArrayIndex(paramArgs, Expression.Constant(index)), p.ParameterType)).ToArray()
                            ),
                            typeof(object)));
                    }
                    methodFunc = Expression.Lambda<Func<object, object[], object>>(
                        Expression.Block(body),
                        paramInstance,
                        paramArgs
                        ).Compile();
                    DELEGATES_METHOD.Add(key, methodFunc);
                }
            }

            return methodFunc(obj, arguments);
        }

        
    }
}
