
namespace HFM.Preferences
{
    public class InMemoryPreferencesProvider : PreferencesProvider
    {
        public InMemoryPreferencesProvider() : base(null, null, null)
        {

        }

        public InMemoryPreferencesProvider(string applicationPath, string applicationDataFolderPath, string applicationVersion)
            : base(applicationPath, applicationDataFolderPath, applicationVersion)
        {

        }
    }
}
