
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace HFM.Client
{
   public class Options : Dictionary<string, string>
   {
      public const string AssignmentServers = "assignment-servers";
      public const string CaptureDirectory = "capture-directory";
      public const string CaptureSockets = "capture-sockets";
      public const string Checkpoint = "checkpoint";
      public const string Child = "child";
      public const string ClientSubtype = "client-subtype";
      public const string ClientType = "client-type";
      public const string CommandAddress = "command-address";
      public const string CommandAllow = "command-allow";
      public const string CommandAllowNoPass = "command-allow-no-pass";
      public const string CommandDeny = "command-deny";
      public const string CommandDenyNoPass = "command-deny-no-pass";
      public const string CommandPort = "command-port";
      public const string ConfigRotate = "config-rotate";
      public const string ConfigRotateDir = "config-rotate-dir";
      public const string ConfigRotateMax = "config-rotate-max";
      public const string CoreDir = "core-dir";
      public const string CoreKey = "core-key";
      public const string CorePrep = "core-prep";
      public const string CorePriority = "core-priority";
      public const string CoreServer = "core-server";
      public const string CpuAffinity = "cpu-affinity";
      public const string CpuSpecies = "cpu-species";
      public const string CpuType = "cpu-type";
      public const string CpuUsage = "cpu-usage";
      public const string Cpus = "cpus";
      public const string CycleRate = "cycle-rate";
      public const string Cycles = "cycles";
      public const string Daemon = "daemon";
      public const string DataDirectory = "data-directory";
      public const string DebugSockets = "debug-sockets";
      public const string DumpAfterDeadline = "dump-after-deadline";
      public const string Eval = "eval";
      public const string ExceptionLocations = "exception-locations";
      public const string ExecDirectory = "exec-directory";
      public const string ExitWhenDone = "exit-when-done";
      public const string ExtraCoreArgs = "extra-core-args";
      public const string ForceWs = "force-ws";
      public const string Gpu = "gpu";
      public const string GpuAssignmentServers = "gpu-assignment-servers";
      public const string GpuDeviceId = "gpu-device-id";
      public const string GpuId = "gpu-id";
      public const string GpuIndex = "gpu-index";
      public const string GpuVendorId = "gpu-vendor-id";
      public const string Log = "log";
      public const string LogColor = "log-color";
      public const string LogCrlf = "log-crlf";
      public const string LogDate = "log-date";
      public const string LogDebug = "log-debug";
      public const string LogDomain = "log-domain";
      public const string LogDomainLevels = "log-domain-levels";
      public const string LogHeader = "log-header";
      public const string LogLevel = "log-level";
      public const string LogNoInfoHeader = "log-no-info-header";
      public const string LogRedirect = "log-redirect";
      public const string LogRotate = "log-rotate";
      public const string LogRotateDir = "log-rotate-dir";
      public const string LogRotateMax = "log-rotate-max";
      public const string LogShortLevel = "log-short-level";
      public const string LogSimpleDomains = "log-simple-domains";
      public const string LogThreadId = "log-thread-id";
      public const string LogTime = "log-time";
      public const string LogToScreen = "log-to-screen";
      public const string LogTruncate = "log-truncate";
      public const string MachineId = "machine-id";
      public const string MaxDelay = "max-delay";
      public const string MaxPacketSize = "max-packet-size";
      public const string MaxQueue = "max-queue";
      public const string MaxShutdownWait = "max-shutdown-wait";
      public const string MaxSlotErrors = "max-slot-errors";
      public const string MaxUnitErrors = "max-unit-errors";
      public const string MaxUnits = "max-units";
      public const string Memory = "memory";
      public const string MinDelay = "min-delay";
      public const string NextUnitPercentage = "next-unit-percentage";
      public const string Priority = "priority";
      public const string NoAssembly = "no-assembly";
      public const string OsSpecies = "os-species";
      public const string OsType = "os-type";
      public const string Passkey = "passkey";
      public const string Password = "password";
      public const string PauseOnBattery = "pause-on-battery";
      public const string PauseOnStart = "pause-on-start";
      public const string Pid = "pid";
      public const string PidFile = "pid-file";
      public const string ProjectKey = "project-key";
      public const string Respawn = "respawn";
      public const string Script = "script";
      public const string Service = "service";
      public const string ServiceDescription = "service-description";
      public const string ServiceRestart = "service-restart";
      public const string ServiceRestartDelay = "service-restart-delay";
      public const string Smp = "smp";
      public const string StackTraces = "stack-traces";
      public const string Team = "team";
      public const string Threads = "threads";
      public const string User = "user";
      public const string Verbosity = "verbosity";

      public static Options Parse(string value)
      {
         var o = JObject.Parse(value);
         var options = new Options();
         foreach (var prop in o.Properties())
         {
            options.Add(prop.Name, (string)prop);
         }
         return options;
      }
   }
}
