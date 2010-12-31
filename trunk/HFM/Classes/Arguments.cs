/*
 * HFM.NET - Command Line Arguments Class
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;

namespace HFM.Classes
{
   internal enum ArgumentType
   {
      Service,
      ResetPrefs,
      OpenFile
   }

   internal static class Arguments
   {
      private const string ServiceArg = "/s";
      private const string ResetPrefsArg = "/r";
      private const string OpenFileArg = "/f";
   
      public static ICollection<Argument> Parse(IList<string> args)
      {
         var arguments = new List<Argument>();
      
         for (int i = 0; i < args.Count; i++)
         {
            if (args[i] == ServiceArg)
            {
               arguments.Add(new Argument { Type = ArgumentType.Service });
            }
            else if (args[i] == ResetPrefsArg)
            {
               arguments.Add(new Argument { Type = ArgumentType.ResetPrefs });
            }
            else if (args[i] == OpenFileArg)
            {
               string data = String.Empty;
               if (i + 1 < args.Count)
               {
                  data = args[++i];
               }
               if (data.Length == 0 || data.StartsWith("-"))
               {
                  throw new ArgumentException("Missing file name argument.");
               }
               arguments.Add(new Argument { Type = ArgumentType.OpenFile, Data = data });
            }
         }

         return arguments.AsReadOnly();
      }
   }
   
   internal class Argument
   {
      public ArgumentType Type { get; set; }
      public string Data { get; set; }
   }
}
