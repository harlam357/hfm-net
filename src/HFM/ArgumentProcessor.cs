
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

using HFM.Core;

namespace HFM
{
   internal sealed class ArgumentProcessor
   {
      private readonly IPreferenceSet _prefs;

      public ArgumentProcessor(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }

      /// <summary>
      /// Process arguments and return a value that indicates whether to continue or exit the program.
      /// </summary>
      /// <param name="arguments">Collection of arguments to process.</param>
      /// <returns>A value that indicates whether to continue or exit the program (true to continue).</returns>
      public bool Process(IEnumerable<Argument> arguments)
      {
         var argument = arguments.FirstOrDefault(x => x.Type.Equals(ArgumentType.ResetPrefs));
         if (argument != null)
         {
            var userConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            if (userConfig.HasFile)
            {
               File.Delete(userConfig.FilePath);
            }
            // Reset
            _prefs.Reset();
            // Exit Application
            return false;
         }

         return true;
      }
   }
}
