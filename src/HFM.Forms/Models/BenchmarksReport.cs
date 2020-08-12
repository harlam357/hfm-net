using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Forms.Models
{
    public interface IBenchmarksReportSource
    {
        SlotIdentifier? SlotIdentifier { get; }

        int? ProjectID { get; }
    }

    public abstract class BenchmarksReport
    {
        public string Key { get; }

        public object Result { get; protected set; }

        protected BenchmarksReport(string key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public abstract void Generate(IBenchmarksReportSource source);

        protected static IEnumerable<string> EnumerateProjectInformation(Protein protein)
        {
            yield return String.Format(CultureInfo.InvariantCulture, " Project ID: {0}", protein.ProjectNumber);
            yield return String.Format(CultureInfo.InvariantCulture, " Core: {0}", protein.Core);
            yield return String.Format(CultureInfo.InvariantCulture, " Credit: {0}", protein.Credit);
            yield return String.Format(CultureInfo.InvariantCulture, " Frames: {0}", protein.Frames);
        }
    }
}
