using System;
using System.Collections.Generic;
using System.Globalization;

namespace HFM
{
    public enum ArgumentType
    {
        Unknown,
        Error,
        ResetPrefs,
        OpenFile
    }

    public static class Arguments
    {
        private const string ResetPrefsArg = "/r";
        private const string OpenFileArg = "/f";

        /// <summary>
        /// Parse Raw Arguments.
        /// </summary>
        /// <param name="args">Raw Argument Array</param>
        /// <returns>Argument Collection</returns>
        public static ICollection<Argument> Parse(IList<string> args)
        {
            var arguments = new List<Argument>();

            for (int i = 0; i < args.Count; i++)
            {
                if (args[i] == ResetPrefsArg)
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
                    if (data.Length == 0 || data.StartsWith("/", StringComparison.Ordinal))
                    {
                        arguments.Add(new Argument { Type = ArgumentType.Error, Data = "Missing file name argument." });
                    }
                    else
                    {
                        arguments.Add(new Argument { Type = ArgumentType.OpenFile, Data = data });
                    }
                }
                else
                {
                    arguments.Add(new Argument { Type = ArgumentType.Unknown, Data = String.Format(CultureInfo.InvariantCulture, "Unknown argument: {0}", args[i]) });
                }
            }

            return arguments.AsReadOnly();
        }

        public static string GetUsageMessage()
        {
            return GetUsageMessage(null);
        }

        public static string GetUsageMessage(IEnumerable<Argument> arguments)
        {
            var sb = new System.Text.StringBuilder();
            if (arguments != null)
            {
                foreach (var argument in arguments)
                {
                    sb.AppendLine(argument.Data);
                }
                sb.AppendLine();
            }
            sb.AppendFormat("{0} Arguments", Core.Application.Name);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(" /r - Reset the user preferences.");
            sb.AppendLine(" /f <filepath> - Load HFM client configuration file.");
            return sb.ToString();
        }
    }

    public sealed class Argument
    {
        public ArgumentType Type { get; set; }

        public string Data { get; set; }
    }
}
