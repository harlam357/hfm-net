using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Forms.Views;
using HFM.Preferences;
using HFM.Proteins;

using LightInject;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HFM;

internal class WorkUnitContextMigration
{
    public IServiceFactory Container { get; }
    public ILogger Logger { get; }
    public IPreferences Preferences { get; }

    public WorkUnitContextMigration(IServiceFactory container, ILogger logger, IPreferences preferences)
    {
        Container = container;
        Logger = logger;
        Preferences = preferences;
    }

    public void Migrate()
    {
        string appDataPath = Preferences.Get<string>(Preference.ApplicationDataFolderPath);
        string filePath = Path.Combine(appDataPath, WorkUnitRepository.DefaultFileName);

        ILegacyWorkUnitSource legacyWorkUnitSource = new NullLegacyWorkUnitSource();
        if (File.Exists(filePath))
        {
            var repository = Container.GetInstance<WorkUnitRepository>();
#if DEBUG
            //var repository = new WorkUnitRepository(null, CreateProteinService());
#endif
            repository.Initialize(filePath);
            if (repository.RequiresUpgrade())
            {
                UpgradeWorkUnitRepository(repository);
            }
            legacyWorkUnitSource = repository;
        }

        string databaseVersion = GetDatabaseVersionFromWorkUnitContext();
        if (ShouldMigrateToWorkUnitContext(databaseVersion))
        {
            var toWorkUnitContext = new MigrateToWorkUnitContext(Logger, Container.GetInstance<IServiceScopeFactory>(), legacyWorkUnitSource);
            ExecuteMigration(toWorkUnitContext.ExecuteAsync);
        }

        databaseVersion = GetDatabaseVersionFromWorkUnitContext();
        if (ShouldCleanWorkUnitContextPlatforms(databaseVersion))
        {
            var cleanPlatforms = new CleanWorkUnitContextPlatforms(Logger, Container.GetInstance<IServiceScopeFactory>());
            ExecuteMigration(cleanPlatforms.ExecuteAsync);
        }
    }

    private static void UpgradeWorkUnitRepository(WorkUnitRepository repository)
    {
        using var dialog = new ProgressDialog((progress, _) =>
        {
            repository.Upgrade(progress);
            return Task.CompletedTask;
        }, false);

        dialog.Text = Core.Application.NameAndVersion;
        dialog.StartPosition = FormStartPosition.CenterScreen;
        dialog.ShowDialog();
        if (dialog.Exception != null)
        {
            throw dialog.Exception;
        }
    }

    private string GetDatabaseVersionFromWorkUnitContext()
    {
        using (Container.BeginScope())
        {
            using var context = Container.GetInstance<WorkUnitContext>();
            context.Database.Migrate();
            return context.GetDatabaseVersion();
        }
    }

    private static bool ShouldMigrateToWorkUnitContext(string databaseVersion) =>
        databaseVersion is not null && Version.Parse(databaseVersion) < new Version(10, 0, 0);

    // cleaning up issues with v10 pre-release builds
    private static bool ShouldCleanWorkUnitContextPlatforms(string databaseVersion) =>
        databaseVersion is not null && Version.Parse(databaseVersion) <= new Version(10, 0, 1927);

    private static void ExecuteMigration(Func<IProgress<Core.ProgressInfo>, Task> migration)
    {
        using var dialog = new ProgressDialog(async (progress, _) => await migration(progress).ConfigureAwait(true), false);
        dialog.Text = Core.Application.NameAndVersion;
        dialog.StartPosition = FormStartPosition.CenterScreen;
        dialog.ShowDialog();
        if (dialog.Exception != null)
        {
            throw dialog.Exception;
        }
    }

#if DEBUG
    public static IProteinService CreateProteinService()
    {
        var collection = new List<Protein>();

        var protein = new Protein();
        protein.ProjectNumber = 6600;
        protein.WorkUnitName = "WorkUnitName";
        protein.Core = "GROGPU2";
        protein.Credit = 450;
        protein.KFactor = 0;
        protein.Frames = 100;
        protein.NumberOfAtoms = 5000;
        protein.PreferredDays = 2;
        protein.MaximumDays = 3;
        collection.Add(protein);

        protein = new Protein();
        protein.ProjectNumber = 5797;
        protein.WorkUnitName = "WorkUnitName2";
        protein.Core = "GROGPU2";
        protein.Credit = 675;
        protein.KFactor = 2.3;
        protein.Frames = 100;
        protein.NumberOfAtoms = 7000;
        protein.PreferredDays = 2;
        protein.MaximumDays = 3;
        collection.Add(protein);

        protein = new Protein();
        protein.ProjectNumber = 8011;
        protein.WorkUnitName = "WorkUnitName3";
        protein.Core = "GRO-A4";
        protein.Credit = 106.6;
        protein.KFactor = 0.75;
        protein.Frames = 100;
        protein.NumberOfAtoms = 9000;
        protein.PreferredDays = 2.13;
        protein.MaximumDays = 4.62;
        collection.Add(protein);

        protein = new Protein();
        protein.ProjectNumber = 6903;
        protein.WorkUnitName = "WorkUnitName4";
        protein.Core = "GRO-A5";
        protein.Credit = 22706;
        protein.KFactor = 38.05;
        protein.Frames = 100;
        protein.NumberOfAtoms = 11000;
        protein.PreferredDays = 5;
        protein.MaximumDays = 12;
        collection.Add(protein);

        var dataContainer = new ProteinDataContainer();
        dataContainer.Data = collection;
        return new ProteinService(dataContainer, null, null);
    }
#endif
}
