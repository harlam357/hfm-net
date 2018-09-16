
using System;

namespace HFM.Log.Internal
{
   internal static class Environment
   {
      internal static readonly bool IsMonoRuntime = Type.GetType("Mono.Runtime") != null;
   }
}
