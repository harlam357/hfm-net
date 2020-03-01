
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Castle.Core.Logging;

using HFM.Core.Net;
using HFM.Core.Services;
using HFM.Preferences;

namespace HFM.Core.SlotXml
{
    public enum WebDeploymentType
    {
        Path,
        Ftp
    }

    public abstract class WebArtifactDeployment
    {
        public ILogger Logger { get; }
        public IPreferenceSet Preferences { get; }

        protected WebArtifactDeployment(ILogger logger, IPreferenceSet preferences)
        {
            Logger = logger ?? NullLogger.Instance;
            Preferences = preferences;
        }

        public void Deploy(string path)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                OnDeploy(path);
            }
            finally
            {
                Logger.Info($"Web deployment finished in {sw.GetExecTime()}");
            }
        }

        protected abstract void OnDeploy(string path);

        protected IEnumerable<string> EnumerateFiles(string path)
        {
            var copyHtml = Preferences.Get<bool>(Preference.WebGenCopyHtml);
            var copyXml = Preferences.Get<bool>(Preference.WebGenCopyXml);
            var copyLogs = Preferences.Get<bool>(Preference.WebGenCopyFAHlog);

            foreach (var f in Directory.EnumerateFiles(path))
            {
                if ((f.EndsWith(".html") || f.EndsWith(".css")) && copyHtml) yield return f;
                if (f.EndsWith(".xml") && copyXml) yield return f;
                if (f.EndsWith("log.txt") && copyLogs) yield return f;
            }
        }

        public static WebArtifactDeployment Create(WebDeploymentType deploymentType, ILogger logger, IPreferenceSet preferences)
        {
            switch (deploymentType)
            {
                case WebDeploymentType.Path:
                    return new PathDeployment(logger, preferences);
                case WebDeploymentType.Ftp:
                    return new FtpDeployment(logger, preferences);
                default:
                    throw new ArgumentOutOfRangeException(nameof(deploymentType), deploymentType, null);
            }
        }
    }

    public class PathDeployment : WebArtifactDeployment
    {
        public PathDeployment(ILogger logger, IPreferenceSet preferences) : base(logger, preferences)
        {

        }

        protected override void OnDeploy(string path)
        {
            var deployPath = Preferences.Get<string>(Preference.WebDeploymentRoot);
            Directory.CreateDirectory(deployPath);

            foreach (var f in EnumerateFiles(path))
            {
                string destFileName = Path.Combine(deployPath, Path.GetFileName(f));
                File.Copy(f, destFileName, true);
            }
        }
    }

    public class FtpDeployment : WebArtifactDeployment
    {
        public IFtpService FtpService { get; }

        public FtpDeployment(ILogger logger, IPreferenceSet preferences) : this(logger, preferences, new FtpService())
        {

        }

        public FtpDeployment(ILogger logger, IPreferenceSet preferences, IFtpService ftpService) : base(logger, preferences)
        {
            FtpService = ftpService;
        }

        protected override void OnDeploy(string path)
        {
            var host = Preferences.Get<string>(Preference.WebGenServer);
            var port = Preferences.Get<int>(Preference.WebGenPort);
            var ftpPath = Preferences.Get<string>(Preference.WebDeploymentRoot);
            var username = Preferences.Get<string>(Preference.WebGenUsername);
            var password = Preferences.Get<string>(Preference.WebGenPassword);
            var ftpMode = Preferences.Get<FtpMode>(Preference.WebGenFtpMode);

            foreach (var f in EnumerateFiles(path))
            {
                FtpService.Upload(host, port, ftpPath, f, username, password, ftpMode);
            }
        }
    }
}
