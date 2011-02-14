
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HFM.Queue.Tool
{
   class Program
   {
      static void Main(string[] args)
      {
         const string app = "HFM.Queue.Tool";

         FileVersionInfo fi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
         Console.WriteLine("{0} version {1} (qd clone)", app, fi.FileVersion);
         Console.WriteLine("Copyright (C) 2002-2005 Richard P. Howell IV.");
         Console.WriteLine("Copyright (C) 2005-2008 Sebastiaan Couwenberg");
         Console.WriteLine("Copyright (C) 2009-2010 Ryan Harlamert");
         
         IEnumerable<Argument> arguments;
         try
         {
            arguments = Arguments.Parse(args);
         }
         catch (Exception ex)
         {
            Console.WriteLine();
            Console.WriteLine("{0}", ex.Message);
            return;
         }

         bool versionOnly = arguments.FirstOrDefault(a => a.Type == ArgumentType.VersionOnly) != null ? true : false;
         if (versionOnly)
         {
            return;
         }

         bool showUsage = arguments.FirstOrDefault(a => a.Type == ArgumentType.Usage) != null ? true : false;
         bool unknown = arguments.FirstOrDefault(a => a.Type == ArgumentType.Unknown) != null ? true : false;
         if (showUsage || unknown)
         {
            Console.WriteLine(Arguments.Usage);
            return;
         }
         
         string filePath = "queue.dat";
         var queueFile = arguments.FirstOrDefault(a => a.Type == ArgumentType.QueueFile);
         if (queueFile != null)
         {
            filePath = queueFile.Data;
         }

         if (!(File.Exists(filePath)))
         {
            Console.WriteLine();
            Console.WriteLine("File '{0}' does not exist.", filePath);
            return;
         }

         try
         {
            var q = QueueReader.ReadQueue(filePath);
            QueueDisplay.Write(q, arguments);
         }
         catch (Exception ex)
         {
            Console.WriteLine();
            Console.WriteLine(ex.Message);
         }
      }
   }
}
