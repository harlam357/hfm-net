/*
 * HFM.NET - Log Line Class
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;

namespace HFM.Log
{
   /// <summary>
   /// Represents a line of a Folding@Home client log.
   /// </summary>
   public class LogLine
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="LogLine"/> class.
      /// </summary>
      public LogLine()
      {
         
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="LogLine"/> class.
      /// </summary>
      /// <param name="raw">The raw log line string.</param>
      /// <param name="index">The index of the line in the log.</param>
      /// <param name="lineType">The type of log line.</param>
      /// <param name="timeStamp">The time stamp of the log line if the log line contains time information.</param>
      /// <param name="data">The object that represents the data parsed from the raw log line.</param>
      public LogLine(string raw, int index, LogLineType lineType, TimeSpan? timeStamp, object data)
      {
         _raw = raw;
         _index = index;
         _lineType = lineType;
         _timeStamp = timeStamp;
         _data = data;
      }

      private string _raw;
      /// <summary>
      /// Gets or sets the raw log line string.
      /// </summary>
      public virtual string Raw
      {
         get => _raw;
         set => _raw = value;
      }

      private int _index;
      /// <summary>
      /// Gets or sets the index of the line in the log.
      /// </summary>
      public virtual int Index
      {
         get => _index;
         set => _index = value;
      }

      private LogLineType _lineType;
      /// <summary>
      /// Gets or sets the type of log line.
      /// </summary>
      public virtual LogLineType LineType
      {
         get => _lineType;
         set => _lineType = value;
      }

      private TimeSpan? _timeStamp;
      /// <summary>
      /// Gets or sets the time stamp of the log line if the log line contains time information.
      /// </summary>
      public virtual TimeSpan? TimeStamp
      {
         get => _timeStamp;
         set => _timeStamp = value;
      }

      private object _data;
      /// <summary>
      /// Gets or sets the object that represents the data parsed from the raw log line.
      /// </summary>
      public virtual object Data
      {
         get => _data;
         set => _data = value;
      }

      /// <summary>
      /// Returns a string that represents the current <see cref="LogLine"/> object.
      /// </summary>
      /// <returns>A string that represents the current <see cref="LogLine"/> object.</returns>
      public override string ToString()
      {
         return Raw;
      }
   }

   /// <summary>
   /// Represents a line of a Folding@Home client log where the TimeStamp and Data properties are lazily evaluated.
   /// </summary>
   public class LazyLogLine : LogLine
   {
      private Lazy<TimeSpan?> _lazyTimeStampParser;
      private Lazy<object> _lazyDataParser;

      /// <summary>
      /// Initializes a new instance of the <see cref="LazyLogLine"/> class where the TimeStamp and Data properties are lazily evaluated using the given parsing functions.
      /// </summary>
      /// <param name="raw">The raw log line string.</param>
      /// <param name="index">The index of the line in the log.</param>
      /// <param name="lineType">The type of log line.</param>
      /// <param name="timeStampParser">The <see cref="LogLineTimeStampParserFunction"/> used to lazily parse time stamp information when the TimeStamp property getter is accessed.</param>
      /// <param name="dataParser">The <see cref="LogLineDataParserFunction"/> used to lazily parse log line data when the Data property getter is accessed.</param>
      public LazyLogLine(string raw, int index, LogLineType lineType, LogLineTimeStampParserFunction timeStampParser, LogLineDataParserFunction dataParser)
         : base(raw, index, lineType, null, null)
      {
         if (timeStampParser != null) _lazyTimeStampParser = new Lazy<TimeSpan?>(() => timeStampParser(this));
         if (dataParser != null) _lazyDataParser = new Lazy<object>(() => dataParser(this));
      }

      private TimeSpan? _timeStamp;
      /// <summary>
      /// Gets or sets the time stamp of the log line if the log line contains time information.
      /// </summary>
      public override TimeSpan? TimeStamp
      {
         get
         {
            if (_timeStamp == null)
            {
               if (_lazyTimeStampParser != null)
               {
                  return _lazyTimeStampParser.Value;
               }
            }
            return _timeStamp;
         }
         set
         {
            _lazyTimeStampParser = null;
            _timeStamp = value;
         }
      }

      private object _data;
      /// <summary>
      /// Gets or sets the object that represents the data parsed from the raw log line.
      /// </summary>
      public override object Data
      {
         get
         {
            if (_data == null)
            {
               if (_lazyDataParser != null)
               {
                  if (!_lazyDataParser.IsValueCreated)
                  {
                     object data = _lazyDataParser.Value;
                     if (data == null)
                     {
                        _lazyDataParser = null;
                        _data = new LogLineDataParserError();
                        return _data;
                     }
                  }
                  return _lazyDataParser.Value;
               }
            }
            return _data;
         }
         set
         {
            _lazyDataParser = null;
            _data = value;
         }
      }
   }
}
