namespace HFM.Preferences;

public partial class XmlPreferencesProvider
{
    protected override IEnumerable<PreferenceUpgrade> OnEnumerateUpgrades()
    {
        yield return new PreferenceUpgrade
        {
            Version = new Version(0, 9, 13),
            Action = data => data.ApplicationSettings.ProjectDownloadUrl = Data.ApplicationSettings.DefaultProjectDownloadUrl
        };

        yield return new PreferenceUpgrade
        {
            Version = new Version(9, 25),
            Action = data => data.MainWindowGrid.Columns = InsertGridColumn(data.MainWindowGrid.Columns, 4)
        };

        yield return new PreferenceUpgrade
        {
            Version = new Version(9, 26),
            Action = data => data.MainWindowState.QueueSplitterLocation = Data.MainWindowState.DefaultQueueSplitterLocation
        };

        yield return new PreferenceUpgrade
        {
            Version = new Version(10, 1),
            Action = data => data.MainWindowGrid.Columns = DeleteGridColumn(data.MainWindowGrid.Columns, 9)
        };
    }

    private static List<string> InsertGridColumn(IEnumerable<string> columns, int insertIndex)
    {
        if (columns is null) return null;

        var parsed = columns
            .Select(FormColumnPreference.Parse)
            .Where(x => x.HasValue)
            .Select(x => x.Value);

        var result = new List<string>();

        foreach (var c in parsed)
        {
            int displayIndex = c.DisplayIndex;
            int index = c.Index;

            if (displayIndex >= insertIndex)
            {
                displayIndex += 1;
            }

            if (index >= insertIndex)
            {
                index += 1;
            }

            result.Add(FormColumnPreference.Format(displayIndex, c.Width, c.Visible, index));
        }

        return result;
    }

    private static List<string> DeleteGridColumn(IEnumerable<string> columns, int deleteIndex)
    {
        if (columns is null) return null;

        var parsed = columns
            .Select(FormColumnPreference.Parse)
            .Where(x => x.HasValue)
            .Select(x => x.Value);

        var result = new List<string>();

        foreach (var c in parsed)
        {
            int displayIndex = c.DisplayIndex;
            int index = c.Index;

            if (index != deleteIndex)
            {
                if (displayIndex > deleteIndex)
                {
                    displayIndex -= 1;
                }

                if (index > deleteIndex)
                {
                    index -= 1;
                }

                result.Add(FormColumnPreference.Format(displayIndex, c.Width, c.Visible, index));
            }
        }

        return result;
    }
}
