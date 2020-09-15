using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using HFM.Log;

namespace HFM.Forms.Controls
{
    public partial class LogFileViewer : RichTextBox
    {
        private ICollection<LogLine> _logLines;

        public object Owner { get; private set; }

        public LogFileViewer()
        {
            InitializeComponent();
        }

        private const int MaxDisplayableLogLines = 500;

        private bool _colorLogFile;

        public bool ColorLogFile
        {
            get => _colorLogFile;
            set
            {
                if (_colorLogFile != value)
                {
                    _colorLogFile = value;
                    ApplyColorLogFile();
                }
            }
        }

        public void SetLogLines(object owner, ICollection<LogLine> lines)
        {
            Owner = owner;

            // limit the maximum number of log lines
            int lineOffset = lines.Count - MaxDisplayableLogLines;
            if (lineOffset > 0)
            {
                lines = lines.Where((x, i) => i > lineOffset).ToList();
            }

            _logLines = lines;
            ApplyColorLogFile();
        }

        private void ApplyColorLogFile()
        {
            if (_logLines is null) return;

            if (ColorLogFile)
            {
                Rtf = BuildRtfString(_logLines);
            }
            else
            {
                Rtf = null;
                Lines = _logLines.Select(line => line.Raw.Replace("\r", String.Empty)).ToArray();
            }
        }

        private static string BuildRtfString(ICollection<LogLine> logLines)
        {
            // cf1 - Dark Green
            // cf2 - Dark Red
            // cf3 - Dark Orange
            // cf4 - Blue
            // cf5 - Slate Gray

            var sb = new StringBuilder();
            sb.Append(@"{\rtf1\ansi\deff0{\colortbl;\red0\green150\blue0;\red139\green0\blue0;\red255\green140\blue0;\red0\green0\blue255;\red120\green120\blue120;}");
            foreach (var line in logLines)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, @"{0}{1}\line", GetLineColor(line), line);
            }
            return sb.ToString();
        }

        private static string GetLineColor(LogLine line)
        {
            if (line.LineType == LogLineType.None)
            {
                return @"\cf5 ";
            }

            if (line.Data is LogLineDataParserError)
            {
                return @"\cf3 ";
            }

            if (line.LineType == LogLineType.WorkUnitFrame)
            {
                return @"\cf1 ";
            }

            if (line.LineType == LogLineType.ClientShutdown ||
                line.LineType == LogLineType.ClientCoreCommunicationsError ||
                line.LineType == LogLineType.ClientCoreCommunicationsErrorShutdown ||
                line.LineType == LogLineType.ClientEuePauseState ||
                line.LineType == LogLineType.WorkUnitCoreShutdown ||
                line.LineType == LogLineType.WorkUnitCoreReturn)
            {
                return @"\cf2 ";
            }

            return @"\cf4 ";
        }

        public void SetNoLogLines()
        {
            _logLines = null;

            Rtf = Core.Application.IsRunningOnMono ? String.Empty : null;
            Text = "No Log Available";
        }

        public void ScrollToBottom()
        {
            SelectionStart = TextLength;

            if (Core.Application.IsRunningOnMono)
            {
                ScrollToCaret();
            }
            else
            {
                Internal.NativeMethods.SendMessage(Handle, Internal.NativeMethods.WM_VSCROLL, new IntPtr(Internal.NativeMethods.SB_BOTTOM), new IntPtr(0));
            }
        }
    }
}
