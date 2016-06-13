
using System.Text.RegularExpressions;

namespace HFM.Log
{
   internal static class UnitInfoLogRegex
   {
      private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;

      internal static readonly Regex RegexProjectNumberFromTag =
         new Regex("P(?<ProjectNumber>.*)R(?<Run>.*)C(?<Clone>.*)G(?<Gen>.*)", Options);
   }
}