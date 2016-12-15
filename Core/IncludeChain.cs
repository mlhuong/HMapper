using Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace HMapper
{
    // Defines the chain of object inclusions as a chain of dictionaries (Member name / chain) 
    internal class IncludeChain
    {
        private Dictionary<string, IncludeChain> _Includes;
        public virtual Dictionary<string, IncludeChain> Includes { get { return _Includes; } }
        public static IncludeChain NullValue = new IncludeChain() { _Includes = new Dictionary<string, IncludeChain>() };

        protected  IncludeChain()
        {
        }

        /// <summary>
        /// Construct a new chain given a member name and a child chain.
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="childChain"></param>
        private IncludeChain(string memberName, IncludeChain childChain)
        {
            _Includes = new Dictionary<string,IncludeChain>() {{memberName, childChain}};
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IncludeChain CreateFromLambdas(LambdaExpression[] inclusions)
        {
            IncludeChain finalIncludeChain = NullValue;
            if (inclusions != null && inclusions.Length > 0)
            {
                finalIncludeChain = IncludeChain.CreateFromLambda(inclusions[0]);
                for (int i = 1; i < inclusions.Length; i++)
                    finalIncludeChain.Merge(IncludeChain.CreateFromLambda(inclusions[i]));
            }
            return finalIncludeChain;
        }

        /// <summary>
        /// Creates a chain from a Lamdba expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="linqInclusion"></param>
        /// <returns></returns>
        private static IncludeChain CreateFromLambda(LambdaExpression linqInclusion)
        {
            return _CreateFromLambda(linqInclusion.Body, NullValue);
        }

        /// <summary>
        /// Creates a chain from an Expression and a child chain.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="childChain"></param>
        /// <returns></returns>
        private static IncludeChain _CreateFromLambda(Expression expression, IncludeChain childChain)
        {
            MemberExpression me = expression as MemberExpression;
            if (me != null)
            {
                ParameterExpression pe = me.Expression as ParameterExpression;
                if (pe != null)
                    return new IncludeChain(me.Member.Name, childChain);
                return _CreateFromLambda(me.Expression, new IncludeChain(me.Member.Name, childChain));
            }

            MethodCallExpression mc = expression as MethodCallExpression;
            if (mc != null)
            {
                if (mc.Method.Name == "Select")
                    return _CreateFromLambda(mc.Arguments[0], _CreateFromLambda((mc.Arguments[1] as LambdaExpression).Body, childChain));
            }
                

            UnaryExpression ue = expression as UnaryExpression;
            if (ue != null)
            {
                if (ue.NodeType == ExpressionType.Convert)
                    return _CreateFromLambda(ue.Operand, childChain);
            }

            throw new Exception(String.Format("Unsupported linq expression: {0}", expression.ToString()));

        }

        /// <summary>
        /// Merge the current IncludeChain with the specified one. The two chains must target the same type.
        /// </summary>
        /// <param name="chain"></param>
        protected virtual void Merge(IncludeChain chain)
        {
            foreach (var kp in chain.Includes)
            {
                IncludeChain originalChain;
                if (Includes.TryGetValue(kp.Key, out originalChain) && (originalChain != NullValue))                
                    originalChain.Merge(kp.Value);
                else
                {
                    Includes[kp.Key] = kp.Value;
                }
            }
        }
    }
}
