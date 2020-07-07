
using System;
using System.Windows.Forms;
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
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
#if DEBUG
            // for manually testing different cultures
            //SetupCulture("de-DE");
#endif
            Core.Net.SecurityProtocol.Setup();
            Core.Application.SetPaths(
                Application.StartupPath,
                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HFM"));

            using (var container = new WindsorContainer())
            {
                var bootStrapper = new BootStrapper(args, container);
                try
                {
                    container.AddFacility<TypedFactoryFacility>();
                    container.Install(new Core.Configuration.ContainerInstaller(), new Forms.Configuration.ContainerInstaller());

                    Forms.Configuration.TypeDescriptionProviderSetup.Execute();

                    bootStrapper.Execute();
                }
                catch (Exception ex)
                {
                    bootStrapper.ShowStartupException(ex);
                    return;
                }

                if (bootStrapper.MainForm != null)
                {
                    Application.Run(bootStrapper.MainForm);
                }
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
