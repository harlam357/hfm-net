
#define NET40

using System;
using System.Linq.Expressions;
using System.Reflection;
#if NET35
using System.Reflection.Emit;
#endif

namespace HFM.Preferences.Internal
{
    // Based on http://stackoverflow.com/questions/7723744/expressionfunctmodel-string-to-expressionactiontmodel-getter-to-sette

    internal static class ExpressionExtensions
    {
        internal static Action<T, TValue> ToSetter<T, TValue>(this Expression<Func<T, TValue>> getter)
        {
            ParameterExpression parameter;
            Expression instance;
            MemberExpression propertyOrField;

            GetMemberExpression(getter, out parameter, out instance, out propertyOrField);

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
#if NET35
            else // if (propertyOrField.Member.MemberType == MemberTypes.Field)
            {
               // 1.2x slower than 4.0 method, 5x faster than 3.5 method
               FieldInfo field = propertyOrField.Member as FieldInfo;
               var action = FieldSetter<T, TValue>(field);
               return action;
            }
#endif
            }

            ParameterExpression value = Expression.Parameter(typeof(TValue), "val");

            Expression expr;
#if NET35
         if (propertyOrField.Member.MemberType == MemberTypes.Property)
         {
            var property = (PropertyInfo)propertyOrField.Member;
            MethodInfo setter = property.GetSetMethod();
            expr = Expression.Call(instance, setter, value);
         }
         else // if (propertyOrField.Member.MemberType == MemberTypes.Field)
         {
            expr = FieldSetter(propertyOrField, value);
         }
#endif

#if NET40
            // For field access it's 5x faster than the 3.5 method and 1.2x than "simple" method. For property access nearly same speed (1.1x faster).
            expr = Expression.Assign(propertyOrField, value);
#endif

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
                throw new ArgumentException("Expression does not specify member access.", "expression");
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
                    throw new ArgumentException("Expression does not specify member access.", "expression");
                }
            }

            parameter = (ParameterExpression)current;
        }

#if NET35
      // Based on http://stackoverflow.com/questions/321650/how-do-i-set-a-field-value-in-an-c-expression-tree/321686#321686
      private static Action<T, TValue> FieldSetter<T, TValue>(FieldInfo field)
      {
         DynamicMethod m = new DynamicMethod("setter", typeof(void), new[] { typeof(T), typeof(TValue) }, typeof(ExpressionExtensions));
         ILGenerator cg = m.GetILGenerator();

         // arg0.<field> = arg1
         cg.Emit(OpCodes.Ldarg_0);
         cg.Emit(OpCodes.Ldarg_1);
         cg.Emit(OpCodes.Stfld, field);
         cg.Emit(OpCodes.Ret);

         return (Action<T, TValue>)m.CreateDelegate(typeof(Action<T, TValue>));
      }

      // Based on http://stackoverflow.com/questions/208969/assignment-in-net-3-5-expression-trees/3972359#3972359
      private static Expression FieldSetter(Expression left, Expression right)
      {
         return
             Expression.Call(
                 null,
                 typeof(ExpressionExtensions)
                     .GetMethod("AssignTo", BindingFlags.NonPublic | BindingFlags.Static)
                     .MakeGenericMethod(left.Type),
                 left,
                 right);
      }

      // ReSharper disable once UnusedMember.Local
      // ReSharper disable once RedundantAssignment
      private static void AssignTo<T>(ref T left, T right)  // note the 'ref', which is
      {                                                     // important when assigning
         left = right;                                     // to value types!
      }
#endif
    }
}
