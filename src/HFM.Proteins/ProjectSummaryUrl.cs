using System.Diagnostics.CodeAnalysis;

namespace HFM.Proteins
{
    /// <summary>
    /// Provides urls for Folding@Home project (protein) information. 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ProjectSummaryUrl
    {
#pragma warning disable 1591
        public static readonly string Html = "https://apps.foldingathome.org/psummary";
        public static readonly string HtmlBeta = "https://apps.foldingathome.org/psummary?visibility=BETA";
        public static readonly string HtmlComplete = "https://apps.foldingathome.org/psummary?visibility=ALL";
        public static readonly string Json = "https://apps.foldingathome.org/psummary.json";
#pragma warning restore 1591
    }
}
