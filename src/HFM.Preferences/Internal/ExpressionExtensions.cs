using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HFM.Preferences.Internal
{
    // Based on http://stackoverflow.com/questions/7723744/expressionfunctmodel-string-to-expressionactiontmodel-getter-to-sette

    internal static class ExpressionExtensions
    {
        internal static Action<T, TValue> ToSetter<T, TValue>(this Expression<Func<T, TValue>> getter)
        {
            GetMemberExpression(getter, out ParameterExpression parameter, out Expression instance, out MemberExpression propertyOrField);

            // Very simple case: p => p.Property or p => p.Field
            if (parameter == instance)
            {
                if (propertyOrField.Member.MemberType == MemberTypes.Property)
                {
                    // This is FASTER than Expression trees! (5x on my benchmarks) but works only on properties
                    var property = (PropertyInfo)propertyOrField.Member;
                    MethodInfo setter = property.GetSetMethod();
                    var action = (Action<T, TValue>)Delegate.CreateDelegate(typeof(Action<T, TValue>), setter);
                    return action;
                }
            }

            ParameterExpression value = Expression.Parameter(typeof(TValue), "val");

            // For field access it's 5x faster than the 3.5 method and 1.2x than "simple" method. For property access nearly same speed (1.1x faster).
            Expression expr = Expression.Assign(propertyOrField, value);

            return Expression.Lambda<Action<T, TValue>>(expr, parameter, value).Compile();
        }

        private static void GetMemberExpression<T, TValue>(Expression<Func<T, TValue>> expression,
            out ParameterExpression parameter, out Expression instance, out MemberExpression propertyOrField)
        {
            Expression current = expression.Body;

            while (current.NodeType == ExpressionType.Convert || current.NodeType == ExpressionType.TypeAs)
            {
                current = ((UnaryExpression)current).Operand;
            }

            if (current.NodeType != ExpressionType.MemberAccess)
            {
                throw new ArgumentException("Expression does not specify member access.", nameof(expression));
            }

            propertyOrField = (MemberExpression)current;
            current = propertyOrField.Expression;
            instance = current;

            while (current.NodeType != ExpressionType.Parameter)
            {
                if (current.NodeType == ExpressionType.Convert || current.NodeType == ExpressionType.TypeAs)
                {
                    current = ((UnaryExpression)current).Operand;
                }
                else if (current.NodeType == ExpressionType.MemberAccess)
                {
                    current = ((MemberExpression)current).Expression;
                }
                else
                {
                    throw new ArgumentException("Expression does not specify member access.", nameof(expression));
                }
            }

            parameter = (ParameterExpression)current;
        }
    }
}
