namespace HFM.Forms.Internal;

internal static class FileSystemPath
{
    /// <summary>
    /// Adds a trailing slash character to the path (Windows or Unix).
    /// </summary>
    internal static string AddTrailingSlash(string path)
    {
        if (path == null) return String.Empty;

        const char backSlash = '\\';
        const char forwardSlash = '/';

        char separatorChar = backSlash;
        if (path.TakeWhile(c => !c.Equals(backSlash)).Any(c => c.Equals(forwardSlash)))
        {
            separatorChar = forwardSlash;
        }

        // if the path is of sufficient length but does not
        // end with the detected directory separator character
        // then append the detected separator character
        if (path.Length > 2 && !path.EndsWith(separatorChar))
        {
            path = String.Concat(path, separatorChar);
        }

        return path;
    }

    /// <summary>
    /// Adds a Unix trailing slash character to the path.
    /// </summary>
    internal static string AddUnixTrailingSlash(string path)
    {
        if (path == null) return String.Empty;

        if (!path.EndsWith('/'))
        {
            path = String.Concat(path, "/");
        }

        return path;
    }
}
