/*
 * HFM.NET - About Form
 * Copyright (C) 2006-2007 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using System.Diagnostics;
using System.Windows.Forms;

using Debug = HFM.Instrumentation.Debug;

namespace HFM.Forms
{
   public partial class frmAbout : Classes.FormWrapper
   {
      #region Constructor
      public frmAbout()
      {
         InitializeComponent();

         FileVersionInfo fileVersionInfo =
         FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
         lblVersion.Text = "Version " + String.Format("{0}.{1}.{2} - Build {3}", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart,
                                                                                 fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
      } 
      #endregion

      #region Event Handlers
      private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         try
         {
            // Issue 28 - wrap process start in try catch
            Process.Start("http://code.google.com/p/fahlogstats-net/");
         }
         catch (Exception ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error,
                                    String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            MessageBox.Show(String.Format(Properties.Resources.ProcessStartError, "FAHLogStats.NET Google Code page"));
         }
      } 
      #endregion
   }
}
