using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using HFM.Log;

namespace HFM.Forms.Controls
{
    public partial class LogLineRichTextBox : RichTextBox
    {
        public object Owner { get; private set; }

        public ICollection<LogLine> LogLines { get; private set; }

        public LogLineRichTextBox()
        {
            InitializeComponent();
        }

        private bool _colorLogFile;

        public bool ColorLogFile
        {
            get => _colorLogFile;
            set
            {
                if (_colorLogFile != value)
                {
                    _colorLogFile = value;
                    SetTextFromLogLines();
                }
            }
        }

        public void SetLogLines(object owner, ICollection<LogLine> logLines)
        {
            if (owner != null && logLines != null && logLines.Count > 0)
            {
                // different owner
                if (!ReferenceEquals(Owner, owner))
                {
                    SetLogLinesInternal(owner, logLines);
                }
                else if (Lines.Length > 0)
                {
                    // get the last text lines from the control and incoming LogLines collection
                    string lastLine = Lines.Last();
                    string lastLogLineText = logLines.LastOrDefault()?.Raw ?? String.Empty;

                    // don't reload ("flicker") if the log appears the same
                    if (lastLine != lastLogLineText)
                    {
                        SetLogLinesInternal(owner, logLines);
                    }
                }
                else
                {
                    SetLogLinesInternal(owner, logLines);
                }
            }
            else
            {
                SetLogLinesInternal(null, null);
            }
        }

        private void SetLogLinesInternal(object owner, ICollection<LogLine> logLines)
        {
            Owner = owner;
            LogLines = FilterMaxDisplayableLogLines(logLines);
            SetTextFromLogLines();
        }

        private const int MaxDisplayableLogLines = 500;

        private static ICollection<LogLine> FilterMaxDisplayableLogLines(ICollection<LogLine> logLines)
        {
            if (logLines != null)
            {
                // limit the maximum number of log lines
                int lineOffset = logLines.Count - MaxDisplayableLogLines;
                if (lineOffset > 0)
                {
                    logLines = logLines.Where((x, i) => i > lineOffset).ToList();
                }
            }
            return logLines;
        }

        private void SetTextFromLogLines()
        {
            if (LogLines is null)
            {
                Rtf = null;
                Text = "No Log Available";
            }
            else
            {
                if (ColorLogFile)
                {
                    Rtf = LogLineRtf.Build(LogLines);
                }
                else
                {
                    Rtf = null;
                    Lines = LogLines.Select(line => line.Raw.Replace("\r", String.Empty)).ToArray();
                }
            }
        }

        public void ScrollToBottom()
        {
            SelectionStart = TextLength;

            Internal.NativeMethods.SendMessage(Handle, Internal.NativeMethods.WM_VSCROLL, new IntPtr(Internal.NativeMethods.SB_BOTTOM), new IntPtr(0));
        }
    }

    public static class LogLineRtf
    {
        public const string Definition = @"{\rtf1\ansi\deff0{\colortbl;\red0\green150\blue0;\red139\green0\blue0;\red255\green140\blue0;\red0\green0\blue255;\red120\green120\blue120;}";

        public static string Build(LogLine line) => String.Format(CultureInfo.InvariantCulture, @"{0}{1}\line", GetLineColor(line), line);

        public static string Build(IEnumerable<LogLine> logLines)
        {
            // cf1 - Dark Green
            // cf2 - Dark Red
            // cf3 - Dark Orange
            // cf4 - Blue
            // cf5 - Slate Gray

            var sb = new StringBuilder();
            sb.Append(Definition);
            foreach (var line in logLines)
            {
                sb.Append(Build(line));
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
    }
}
