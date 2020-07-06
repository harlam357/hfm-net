
using System;

using Microsoft.Win32;

using HFM.Core.Logging;

namespace HFM.Forms
{
    public class RegistryAutoRunConfiguration : IAutoRunConfiguration
    {
        private const string AutoRunKeyName = "HFM.NET";
        private const string HkCuAutoRunSubKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

        private readonly ILogger _logger;

        public RegistryAutoRunConfiguration(ILogger logger)
        {
            _logger = logger ?? NullLogger.Instance;
        }

        /// <summary>
        /// Is auto run enabled?
        /// </summary>
        public bool IsEnabled()
        {
            try
            {
                object currentHfmAutoRunValue = null;

                using (RegistryKey regHkCuRun = GetHkCuAutoRunKey())
                {
                    if (regHkCuRun != null)
                    {
                        currentHfmAutoRunValue = regHkCuRun.GetValue(AutoRunKeyName);
                    }
                }

                return currentHfmAutoRunValue != null && currentHfmAutoRunValue.ToString().Length > 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return false;
        }

        /// <summary>
        /// Sets the HFM.NET auto run value.
        /// </summary>
        /// <param name="filePath">The file path to HFM.exe executable.</param>
        /// <exception cref="InvalidOperationException">Auto run value cannot be set.</exception>
        public void SetFilePath(string filePath)
        {
            try
            {
                using (RegistryKey regHkCuRun = GetHkCuAutoRunKey(true))
                {
                    if (String.IsNullOrEmpty(filePath))
                    {
                        regHkCuRun.DeleteValue(AutoRunKeyName, false);
                    }
                    else
                    {
                        regHkCuRun.SetValue(AutoRunKeyName, WrapInQuotes(filePath), RegistryValueKind.String);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("HFM.NET auto run value could not be set.", ex);
            }
        }

        private static RegistryKey GetHkCuAutoRunKey(bool create = false)
        {
            RegistryKey regHkCuRun = Registry.CurrentUser.OpenSubKey(HkCuAutoRunSubKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (regHkCuRun != null)
            {
                return regHkCuRun;
            }
            return create ? Registry.CurrentUser.CreateSubKey(HkCuAutoRunSubKey, RegistryKeyPermissionCheck.ReadWriteSubTree) : null;
        }

        private static string WrapInQuotes(string value)
        {
            return String.Concat("\"", value, "\"");
        }
    }
}
