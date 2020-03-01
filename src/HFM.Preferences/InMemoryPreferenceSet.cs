
namespace HFM.Preferences
{
    public class InMemoryPreferenceSet : PreferenceSetBase
    {
        public InMemoryPreferenceSet() : base(null, null, null)
        {

        }

        public InMemoryPreferenceSet(string applicationPath, string applicationDataFolderPath, string applicationVersion) 
            : base(applicationPath, applicationDataFolderPath, applicationVersion)
        {

        }
    }
}
