
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

using HFM.Core.Client;
using HFM.Preferences;

namespace HFM.Core.SlotXml
{
    public struct HtmlBuilderResult
    {
        public HtmlBuilderResult(string cssFile, ICollection<string> slotSummaryFiles, ICollection<string> slotDetailFiles)
        {
            CssFile = cssFile;
            SlotSummaryFiles = slotSummaryFiles;
            SlotDetailFiles = slotDetailFiles;
        }

        public string CssFile { get; set; }

        public ICollection<string> SlotSummaryFiles { get; set; }

        public ICollection<string> SlotDetailFiles { get; set; }
    }

    public class HtmlBuilder
    {
        public IPreferences Preferences { get; }

        public HtmlBuilder(IPreferences preferences)
        {
            Preferences = preferences;
        }

        public HtmlBuilderResult Build(XmlBuilderResult xmlBuilderResult, string path)
        {
            var cssFileName = Preferences.Get<string>(Preference.CssFile);

            var cssFile = CopyCssFile(path, cssFileName);
            var slotSummaryFiles = EnumerateSlotSummaryFiles(xmlBuilderResult.SlotSummaryFile, path, cssFileName).ToList();
            var slotDetailFiles = EnumerateSlotDetailFiles(xmlBuilderResult.SlotDetailFiles, path, cssFileName).ToList();
            return new HtmlBuilderResult(cssFile, slotSummaryFiles, slotDetailFiles);
        }

        private string CopyCssFile(string path, string cssFileName)
        {
            var applicationPath = Preferences.Get<string>(Preference.ApplicationPath);

            string cssFilePath = Path.Combine(applicationPath, Application.CssFolderName, cssFileName);
            if (File.Exists(cssFilePath))
            {
                var destFileName = Path.Combine(path, cssFileName);
                File.Copy(cssFilePath, destFileName, true);
                return destFileName;
            }
            return null;
        }

        private IEnumerable<string> EnumerateSlotSummaryFiles(string slotSummaryFile, string path, string cssFileName)
        {
            // Load the Overview XML
            var summaryXml = new XmlDocument();
            summaryXml.Load(slotSummaryFile);

            StreamWriter sw;
            // Generate the index page
            string filePath = Path.Combine(path, "index.html");
            using (sw = new StreamWriter(filePath, false))
            {
                sw.Write(Transform(summaryXml, GetXsltFileName(Preference.WebOverview), cssFileName));
            }
            yield return filePath;

            // Generate the summary page
            filePath = Path.Combine(path, "summary.html");
            using (sw = new StreamWriter(filePath, false))
            {
                sw.Write(Transform(summaryXml, GetXsltFileName(Preference.WebSummary), cssFileName));
            }
            yield return filePath;
        }

        private IEnumerable<string> EnumerateSlotDetailFiles(ICollection<string> slotDetailFiles, string path, string cssFileName)
        {
            string slotXslt = GetXsltFileName(Preference.WebSlot);
            // Generate a page per slot
            foreach (var f in slotDetailFiles)
            {
                var slotXml = new XmlDocument();
                slotXml.Load(f);

                string filePath = Path.Combine(path, Path.ChangeExtension(f, ".html"));
                using (var sw = new StreamWriter(filePath, false))
                {
                    sw.Write(Transform(slotXml, slotXslt, cssFileName));
                }
                yield return filePath;
            }
        }

        private static string Transform(XmlNode xmlDoc, string xsltFilePath, string cssFileName)
        {
            // Create the XslCompiledTransform and Load the XmlReader
            var xslt = new XslCompiledTransform();
            using (var xmlReader = new XmlTextReader(xsltFilePath))
            {
                xslt.Load(xmlReader, null, new XmlUrlResolver());
            }

            // Transform the XML data to an in memory stream
            using (var ms = new MemoryStream())
            {
                xslt.Transform(xmlDoc, null, ms);

                // Return the transformed XML
                string webPage = Encoding.UTF8.GetString(ms.ToArray());
                webPage = webPage.Replace("$CSSFILE", cssFileName);
                return webPage;
            }
        }

        private string GetXsltFileName(Preference p)
        {
            var xslt = Preferences.Get<string>(p);

            if (Path.IsPathRooted(xslt))
            {
                if (File.Exists(xslt))
                {
                    return xslt;
                }
            }
            else
            {
                string xsltFileName = Path.Combine(Preferences.Get<string>(Preference.ApplicationPath), Application.XsltFolderName, xslt);
                if (File.Exists(xsltFileName))
                {
                    return xsltFileName;
                }
            }

            throw new FileNotFoundException($"XSLT File '{xslt}' cannot be found.");
        }

        internal static Color GetHtmlFontColor(SlotStatus status)
        {
            switch (status)
            {
                case SlotStatus.RunningNoFrameTimes:
                case SlotStatus.Paused:
                case SlotStatus.Finishing:
                case SlotStatus.Offline:
                    return Color.Black;
                default:
                    return Color.White;
            }
        }
    }
}
