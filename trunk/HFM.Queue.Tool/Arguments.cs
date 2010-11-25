
using System;
using System.Collections.Generic;
using System.Text;

namespace HFM.Queue.Tool
{
   internal enum ArgumentType
   {
      Unknown = -1,
      Usage,
      CurrentOnly,
      QueueFile,
      PrintProjectString,
      ShowAll,
      VersionOnly
   }

   internal static class Arguments
   {
      public static IEnumerable<Argument> Parse(string[] args)
      {
         var arguments = new List<Argument>();
      
         for (int i = 0; i < args.Length; i++)
         {
            if (args[i] == "-u")
            {
               arguments.Add(new Argument { Type = ArgumentType.Usage });
            }
            else if (args[i] == "-C")
            {
               arguments.Add(new Argument { Type = ArgumentType.CurrentOnly });
            }
            else if (args[i] == "-P")
            {
               arguments.Add(new Argument { Type = ArgumentType.PrintProjectString });
            }
            else if (args[i] == "-a")
            {
               arguments.Add(new Argument { Type = ArgumentType.ShowAll });
            }
            else if (args[i] == "-q")
            {
               string data = String.Empty;
               if (i + 1 < args.Length)
               {
                  data = args[++i];
               }
               if (data.Length == 0 || data.StartsWith("-"))
               {
                  throw new ArgumentException("Missing queue file name argument.");
               }
               arguments.Add(new Argument { Type = ArgumentType.QueueFile, Data = data });
            }
            else if (args[i] == "-v")
            {
               arguments.Add(new Argument { Type = ArgumentType.VersionOnly });
            }
            else
            {
               arguments.Add(new Argument { Type = ArgumentType.Unknown });
            }
         }

         return arguments;
      }

      public static string Usage
      {
         get
         {
            var sb = new StringBuilder();
            sb.AppendLine(" -u      Print this usage message (and exit)");
            sb.AppendLine(" -C      Only show current index");
            sb.AppendLine(" -P      Print extra Project, Run, Clone, Generation line like in FAHlog.txt");
            sb.AppendLine(" -a      Show all, also displays the passkey if it's set");
            sb.AppendLine(" -q file Explicitly specify queue data file");
            sb.Append(" -v      Just print version information and stop");
            return sb.ToString();
         }
      }
   }
   
   internal class Argument
   {
      public ArgumentType Type { get; set; }
      public string Data { get; set; }
   }
}
