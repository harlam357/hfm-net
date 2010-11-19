
using System;
using System.Collections.Generic;

namespace HFM.Framework.DataTypes
{
   public static class Paths
   {
      /// <summary>
      /// Are two Client Instance Paths Equal?
      /// </summary>
      public static bool Equal(string path1, string path2)
      {
         ICollection<string> path1Variations = GetPathVariations(path1);
         ICollection<string> path2Variations = GetPathVariations(path2);

         foreach (var variation1 in path1Variations)
         {
            foreach (var variation2 in path2Variations)
            {
               if (variation1.Equals(variation2))
               {
                  return true;
               }
            }
         }

         return false;
      }

      private static ICollection<string> GetPathVariations(string path)
      {
         var pathVariations = new List<string>(2);
         if (path.EndsWith("\\") || path.EndsWith("/"))
         {
            pathVariations.Add(path.ToUpperInvariant());
         }
         else
         {
            pathVariations.Add(String.Concat(path.ToUpperInvariant(), "\\"));
            pathVariations.Add(String.Concat(path.ToUpperInvariant(), "/"));
         }
         return pathVariations;
      }
   }
}
