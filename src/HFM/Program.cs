/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using Application = System.Windows.Forms.Application;

using Castle.Facilities.TypedFactory;
using Castle.Windsor;

namespace HFM
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if DEBUG
            // for manually testing different cultures
            //SetupCulture("de-DE");
#endif
            Core.Net.SecurityProtocol.Setup();

            try
            {
                using (var container = new WindsorContainer())
                {
                    container.AddFacility<TypedFactoryFacility>();
                    container.Install(new Core.Configuration.ContainerInstaller
                        {
                            ApplicationPath = Application.StartupPath,
                            ApplicationDataFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HFM")
                        },
                        new Forms.Configuration.ContainerInstaller());

                    // Setup TypeDescriptor
                    Forms.Configuration.TypeDescriptionProviderSetup.Execute();

                    BootStrapper.Execute(args, container);
                }
            }
            catch (Exception ex)
            {
                BootStrapper.ShowStartupError(ex, null);
            }
        }

#if DEBUG
        private static void SetupCulture(string name)
        {
            // Dutch-Belgium: nl-BE
            // German-Germany: de-DE
            // https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(name);
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo(name);
        }
#endif
    }
}
