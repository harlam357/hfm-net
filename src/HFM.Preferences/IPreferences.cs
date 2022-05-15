namespace HFM.Preferences;

public interface IPreferences
{
    /// <summary>
    /// Resets the preferences to default values.
    /// </summary>
    void Reset();

    /// <summary>
    /// Loads the preferences from the last saved values.
    /// </summary>
    void Load();

    /// <summary>
    /// Saves the preferences.
    /// </summary>
    void Save();

    /// <summary>
    /// Gets a preference value.
    /// </summary>
    /// <typeparam name="T">The type of the preference value.</typeparam>
    /// <param name="key">The preference key.</param>
    T Get<T>(Preference key);

    /// <summary>
    /// Sets a preference value.
    /// </summary>
    /// <typeparam name="T">The type of the preference value.</typeparam>
    /// <param name="key">The preference key.</param>
    /// <param name="value">The preference value.</param>
    void Set<T>(Preference key, T value);

    /// <summary>
    /// Raised when a preference value is changed.
    /// </summary>
    event EventHandler<PreferenceChangedEventArgs> PreferenceChanged;
}

public class PreferenceChangedEventArgs : EventArgs
{
    public Preference Preference { get; }

    public PreferenceChangedEventArgs(Preference preference)
    {
        Preference = preference;
    }
}
