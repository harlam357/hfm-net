using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace HFM.Core;

public static class Application
{
    public static string Path { get; private set; }

    public static string DataFolderPath { get; private set; }

    public static void SetPaths(string path, string dataFolderPath)
    {
        Path = path;
        DataFolderPath = dataFolderPath;
    }

    public const string Name = "HFM";

    public const string CssFolderName = "CSS";
    public const string XsltFolderName = "XSL";

    public const string ProjectSiteUrl = "https://github.com/harlam357/hfm-net";
    public const string SupportForumUrl = "https://groups.google.com/g/hfm-net";

    /// <summary>
    /// Gets a string in the format Name vMajor.Minor.Build.
    /// </summary>
    public static string NameAndVersion => $"{Name} v{Version}";

    private static string _Version;
    /// <summary>
    /// Gets a string in the format Major.Minor.Build.
    /// </summary>
    public static string Version => _Version ??= CreateVersionString("{0}.{1}.{2}");

    private static string CreateVersionString(string format)
    {
        var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        return String.Format(CultureInfo.InvariantCulture, format, fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
    }

    public static Version VersionNumber
    {
        get
        {
            var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            return new Version(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart);
        }
    }
}
