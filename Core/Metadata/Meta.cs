using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Metadata
{
    /// <summary>
    /// Class managing meta information.
    /// </summary>
    public class Meta
    {
        private static readonly ConcurrentDictionary<Type, Meta> m_DicMetas;

        private readonly ConcurrentDictionary<string, Func<object, object>> DELEGATES_GET;
        private readonly ConcurrentDictionary<string, Action<object, object>> DELEGATES_SET;
        /// <summary>
        /// Type.
        /// </summary>
        public readonly Type Type;
        private const BindingFlags INSTANCE_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags STATIC_BINDING_FLAGS = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        static Meta()
        {
            m_DicMetas = new ConcurrentDictionary<Type, Meta>();
        }

        #region Static methods

        /// <summary>
        /// Gets a Meta object given its type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Meta Get(Type type)
        {
            return m_DicMetas.GetOrAdd(type, t => new Meta(t));
        }

        /// <summary>
        /// Gets the MethodInfo designed by the lambda expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MethodInfo Method(Expression<Action> expression)
        {
            MethodCallExpression callExpr = expression.Body as MethodCallExpression;
            if (callExpr != null)
                return callExpr.Method;

            throw new Exception("Lamda expression not supported for Meta.Method() method.");
        }

        /// <summary>
        /// Gets the FieldInfo designed by the lambda expression
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="expression"></param>
        /// <param name="binding"></param>
        /// <returns></returns>
        public static FieldInfo Field<U>(Expression<Func<U>> expression, BindingFlags binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            MemberExpression memberExpr = expression.Body as MemberExpression;
            if (memberExpr != null)
                return memberExpr.Member as FieldInfo;

            throw new Exception("Lamda expression not supported for Meta.Field() method.");
        }

        /// <summary>
        /// Gets the PropertyInfo designed by the lambda expression
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="expression"></param>
        /// <param name="binding"></param>
        /// <returns></returns>
        public static PropertyInfo Property<U>(Expression<Func<U>> expression, BindingFlags binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            MemberExpression memberExpr = expression.Body as MemberExpression;
            if (memberExpr != null)
                return memberExpr.Member as PropertyInfo;

            throw new Exception("Lamda expression not supported for Meta<T>.Property() method.");
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Contructeurs
        /// </summary>
        protected Meta(Type type)
        {
            Type = type;
            DELEGATES_GET = new ConcurrentDictionary<string, Func<object, object>>();
            DELEGATES_SET = new ConcurrentDictionary<string, Action<object, object>>();
        }

        #endregion

        #region Instance methods

        /// <summary>
        /// Get the value of the specified member (Property of Field) of the specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public object GetValue(object obj, string memberName)
        {
            Func<object, object> getFunc = DELEGATES_GET.GetOrAdd(memberName, m =>
            {
                ParameterExpression paramInstance = Expression.Parameter(typeof(object));
                return Expression.Lambda<Func<object, object>>(
                    Expression.Convert(
                        Expression.PropertyOrField(Expression.TypeAs(paramInstance, Type), memberName),
                        typeof(object)
                    ),
                    paramInstance
                ).Compile();
            });

            return getFunc(obj);
        }

        /// <summary>
        /// Set the value of the specified member (Property of Field) of the specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        public void SetValue(object obj, string memberName, object value)
        {
            Action<object, object> setAction = DELEGATES_SET.GetOrAdd(memberName, m =>
            {
                ParameterExpression paramInstance = Expression.Parameter(typeof(object));
                ParameterExpression paramValue = Expression.Parameter(typeof(object));
                MemberInfo member = Type.GetMember(memberName, INSTANCE_BINDING_FLAGS)[0];
                return Expression.Lambda<Action<object, object>>(
                    Expression.Assign(
                        Expression.PropertyOrField(Expression.TypeAs(paramInstance, Type), memberName),
                        Expression.Convert(paramValue, (member is PropertyInfo) ? (member as PropertyInfo).PropertyType : (member as FieldInfo).FieldType)
                    ),
                    paramInstance,
                    paramValue
                    ).Compile();
            });

            setAction(obj, value);
        }
        

        #endregion
    }

}
