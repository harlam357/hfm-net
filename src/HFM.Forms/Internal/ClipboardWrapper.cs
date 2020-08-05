// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3702 $</version>
// </file>

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HFM.Forms.Internal
{
    /// <summary>
    /// Helper class to access the clipboard without worrying about ExternalExceptions
    /// </summary>
    internal static class ClipboardWrapper
    {
        public static bool ContainsText
        {
            get
            {
                try
                {
                    return Clipboard.ContainsText();
                }
                catch (ExternalException)
                {
                    return false;
                }
            }
        }

        public static string GetText()
        {
            // retry 2 times should be enough for read access
            try
            {
                return Clipboard.GetText();
            }
            catch (ExternalException)
            {
                return Clipboard.GetText();
            }
        }

        public static void SetText(string text)
        {
            var data = new DataObject();
            data.SetData(DataFormats.UnicodeText, true, text);
            SetDataObject(data);
        }

        /// <summary>
        /// Gets the current clipboard content.
        /// Can return null!
        /// </summary>
        public static IDataObject GetDataObject()
        {
            // retry 2 times should be enough for read access
            try
            {
                return Clipboard.GetDataObject();
            }
            catch (ExternalException)
            {
                try
                {
                    return Clipboard.GetDataObject();
                }
                catch (ExternalException)
                {
                    return null;
                }
            }
        }

        public static void SetDataObject(object data)
        {
            SafeSetClipboard(data);
        }

        // Code duplication: TextAreaClipboardHandler.cs also has SafeSetClipboard
        [ThreadStatic] private static int _SafeSetClipboardDataVersion;

        private static void SafeSetClipboard(object dataObject)
        {
            // Work around ExternalException bug. (SD2-426)
            // Best reproducible inside Virtual PC.
            int version = unchecked(++_SafeSetClipboardDataVersion);
            try
            {
                Clipboard.SetDataObject(dataObject, true);
            }
            catch (ExternalException)
            {
                var timer = new Timer();
                timer.Interval = 100;
                timer.Tick += delegate
                {
                    timer.Stop();
                    timer.Dispose();
                    if (_SafeSetClipboardDataVersion == version)
                    {
                        try
                        {
                            Clipboard.SetDataObject(dataObject, true, 10, 50);
                        }
                        catch (ExternalException) { }
                    }
                };
                timer.Start();
            }
        }
    }
}
