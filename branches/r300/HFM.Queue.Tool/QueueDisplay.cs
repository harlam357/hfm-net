
using System;
using System.Linq;
using System.Collections.Generic;

namespace HFM.Queue.Tool
{
   internal static class QueueDisplay
   {
      public static void Write(QueueData q, IEnumerable<Argument> arguments)
      {
         Console.WriteLine("Queue version {0:0.00}", q.Version / 100);
         Console.WriteLine("Current index: {0}", q.CurrentIndex);

         bool currentOnly = arguments.FirstOrDefault(a => a.Type == ArgumentType.CurrentOnly) != null ? true : false;
         if (currentOnly)
         {
            var entry = q.GetQueueEntry(q.CurrentIndex);
            WriteEntry(entry, arguments);
         }
         else
         {
            uint i = IncrementIndex(q.CurrentIndex);
            while (true)
            {
               var entry = q.GetQueueEntry(i);
               WriteEntry(entry, arguments);

               if (i == q.CurrentIndex)
               {
                  break;
               }

               i = IncrementIndex(i);
            }
         }

         if (q.ResultsSentUtc.Equals(QueueData.Epoch2000) == false)
         {
            Console.WriteLine("Results successfully sent: {0:ddd MMM dd HH:mm:ss yyyy}", q.ResultsSentLocal);
         }
         Console.WriteLine("Average download rate {0:0.000} KB/s (u={1}); upload rate {2:0.000} KB/s (u={3})",
                           q.DownloadRateAverage, q.DownloadRateUnitWeight, q.UploadRateAverage, q.UploadRateUnitWeight);
         Console.WriteLine("Performance fraction {0:0.000000} (u={1})", q.PerformanceFraction, q.PerformanceFractionUnitWeight);
      }
      
      private static uint IncrementIndex(uint i)
      {
         if (i == 9)
         {
            return 0;
         }
         return i+1;
      }

      private static void WriteEntry(QueueEntry e, IEnumerable<Argument> arguments)
      {
         bool printProjectString = arguments.FirstOrDefault(a => a.Type == ArgumentType.PrintProjectString) != null ? true : false;
         bool showAll = arguments.FirstOrDefault(a => a.Type == ArgumentType.ShowAll) != null ? true : false;
      
         string statusString = QueueEntry.EntryStatusStrings[e.EntryStatus];
         Console.Write(" Index {0}: {1}", e.Index, statusString);
         if (e.SpeedFactor == 0)
         {
            Console.WriteLine();
         }
         else
         {
            Console.WriteLine(" {0} X min speed", e.SpeedFactor);
         }

         Console.WriteLine("  server: {0}:{1}; project: {2}", e.ServerIP, e.ServerPort, e.ProjectID);
         string misc4aEndian = e.Misc4aBigEndian ? "be" : "le";
         Console.WriteLine("  Folding: run {0}, clone {1}, generation {2}; benchmark {3}; misc: {4}, {5}, {6} ({7})",
                           e.ProjectRun, e.ProjectClone, e.ProjectGen, e.Benchmark, e.Misc1a, e.Misc1b, e.Misc4a, misc4aEndian);
         if (printProjectString)
         {
            Console.WriteLine("  Project: {0} (Run {1}, Clone {2}, Gen {3})", e.ProjectID, e.ProjectRun, e.ProjectClone, e.ProjectGen);
         }
         Console.WriteLine("  issue: {0:ddd MMM dd HH:mm:ss yyyy}; begin: {1:ddd MMM dd HH:mm:ss yyyy}", e.ProjectIssuedLocal, e.BeginTimeLocal);
         Console.Write("  ");
         if (e.EntryStatus == 3 || e.EntryStatus == 7)
         {
            Console.Write("end: {0:ddd MMM dd HH:mm:ss yyyy}; ", e.EndTimeLocal);
         }
         TimeSpan preferred = e.DueTimeLocal.Subtract(e.BeginTimeLocal);
         Console.WriteLine("due: {0:ddd MMM dd HH:mm:ss yyyy} ({1} days)", e.DueTimeLocal, Math.Ceiling(preferred.TotalDays));
         Console.WriteLine("  core URL: {0}", e.CoreDownloadUrl);
         Console.Write("  core number: 0x{0}", e.CoreNumber);
         if (QueueEntry.CoreNumberStrings.ContainsKey(e.CoreNumber))
         {
            Console.WriteLine("; core name: {0}", QueueEntry.CoreNumberStrings[e.CoreNumber]);
         }
         else
         {
            Console.WriteLine();
         }
         Console.WriteLine("  CPU: {0},{1} {2}; OS: {3},{4} {5}", e.CpuType, e.CpuSpecies, e.CpuString, e.OsType, e.OsSpecies, e.OsString);
         Console.WriteLine("  smp cores: {0}; cores to use: {1}", e.NumberOfSmpCores, e.UseCores);
         Console.WriteLine("  tag: {0}", e.WorkUnitTag);
         if (e.Passkey.Length != 0 && showAll)
         {
            Console.WriteLine("  passkey: {0}", e.Passkey);
         }
         Console.WriteLine("  flops: {0} ({1:0.000000} megaflops)", e.Flops, e.MegaFlops);
         Console.WriteLine("  memory: {0} MB", e.Memory);
         Console.WriteLine("  client type: {0} {1}", e.RequiredClientType, QueueEntry.RequiredClientTypeStrings[e.RequiredClientType]);
         string assignmentInfoEndian = e.AssignmentInfoBigEndian ? "be" : "le";
         if (e.AssignmentInfoPresent)
         {
            Console.WriteLine("  assignment info ({0}): {1:ddd MMM dd HH:mm:ss yyyy}; {2}", assignmentInfoEndian, e.AssignmentTimeStampLocal, e.AssignmentInfoChecksum);
         }
         Console.Write("  CS: {0}; ", e.CollectionServerIP);
         if (e.NumberOfUploadFailures != 0)
         {
            Console.Write("upload failures: {0}; ", e.NumberOfUploadFailures);
         }
         Console.WriteLine("P limit: {0}", e.PacketSizeLimit);
         Console.WriteLine("  user: {0}; team: {1}; ID: {2}; mach ID: {3}", e.FoldingID, e.Team, e.ID, e.MachineID);
         Console.WriteLine("  work/wudata_{0:00}.dat file size: {1}; WU type: {2}", e.Index, e.WuDataFileSize, e.WorkUnitType);
      }
   }
}
