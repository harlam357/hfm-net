
using System.Linq;
using System.Reflection;

using HFM.Log;

namespace HFM.Core
{
   // TODO: Implement in DataAggregator classes
   internal static class LogLineCopier
   {
      internal static LogLine Copy(LogLine other)
      {
         return new LogLine(other.Raw, other.Index, other.LineType, other.TimeStamp, ObjectCopier.Copy(other.Data));
      }

      private static class ObjectCopier
      {
         internal static object Copy(object other)
         {
            if (other == null)
            {
               return null;
            }

            var dataType = other.GetType();
            // This check is not all encompassing but should work for the needs of this class.
            // We want classes that are not System.String.
            if (!dataType.IsClass || dataType == typeof(string))
            {
               return other;
            }

            // look for a copy constructor
            var copyConstructor = dataType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
               .FirstOrDefault(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == dataType);
            if (copyConstructor != null)
            {
               return copyConstructor.Invoke(new[] { other });
            }
            return other;
         }
      }
   }
}
