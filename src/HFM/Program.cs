using System.Reflection;

using LightInject;
using LightInject.Microsoft.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using Application = System.Windows.Forms.Application;

namespace HFM
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
#if DEBUG
            // for manually testing different cultures
            //SetupCulture("de-DE");
#endif
            Core.Application.SetPaths(
                Application.StartupPath,
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HFM"));

            using (var container = new ServiceContainer())
            {
                var bootStrapper = new BootStrapper(args, container);
                try
                {
                    container.RegisterAssembly(Assembly.GetExecutingAssembly());
                    // wires up IServiceProvider and IServiceScopeFactory
                    _ = container.CreateServiceProvider(new EmptyServiceCollection()
                        .AddMemoryCache());
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

        // shim to pass to CreateServiceProvider, using LightInject syntax for registration not Microsoft
        private class EmptyServiceCollection : List<ServiceDescriptor>, IServiceCollection
        {

        }
    }
}
