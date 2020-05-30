/*
 * Clipboard Helper Class
 * Copyright (C) 2002-2010 by AlphaSierraPapa, Christoph Wille
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

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
    public static class ClipboardWrapper
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
            DataObject data = new DataObject();
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
        [ThreadStatic] static int SafeSetClipboardDataVersion;

        static void SafeSetClipboard(object dataObject)
        {
            // Work around ExternalException bug. (SD2-426)
            // Best reproducable inside Virtual PC.
            int version = unchecked(++SafeSetClipboardDataVersion);
            try
            {
                Clipboard.SetDataObject(dataObject, true);
            }
            catch (ExternalException)
            {
                Timer timer = new Timer();
                timer.Interval = 100;
                timer.Tick += delegate
                {
                    timer.Stop();
                    timer.Dispose();
                    if (SafeSetClipboardDataVersion == version)
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
