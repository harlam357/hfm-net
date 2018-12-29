
using System.Linq;
using System.Reflection;

namespace HFM.Log.Internal
{
   internal static class ObjectCopier
   {
      internal static object Copy(object other)
      {
         if (other == null)
         {
            return null;
         }

         var dataType = other.GetType();
         // This check is not all encompassing but should work for the needs of this library.
         // We want classes that are not System.String.
         if (!dataType.IsClass || dataType == typeof(string))
         {
            return other;
         }

         // look for a Copy() method
         var copyMethod = dataType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(x => x.Name == "Copy" && x.ReturnType == dataType && x.GetParameters().Length == 0);
         if (copyMethod != null)
         {
            return copyMethod.Invoke(other, null);
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
