/*
 * Class based primarily on code from: http://www.codeproject.com/KB/threads/SingleInstancingWithIpc.aspx
 */

using System;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace HFM
{
    internal static class SingleInstanceHelper
    {
        // ReSharper disable once NotAccessedField.Local
#pragma warning disable IDE0052 // Remove unread private members
        private static Mutex _Mutex;
#pragma warning restore IDE0052 // Remove unread private members

        private static readonly string _AssemblyGuid = GetAssemblyGuid();
        private static readonly string _MutexName = String.Format(CultureInfo.InvariantCulture, "Global\\hfm-{0}-{1}", Environment.UserName, _AssemblyGuid);

        public static bool Start()
        {
            _Mutex = new Mutex(true, _MutexName, out bool onlyInstance);
            return onlyInstance;
        }

        private static string GetAssemblyGuid()
        {
            object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false);
            if (attributes.Length == 0)
            {
                return String.Empty;
            }
            return ((System.Runtime.InteropServices.GuidAttribute)attributes[0]).Value;
        }
    }
}
