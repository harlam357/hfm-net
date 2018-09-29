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
   public class LogLine
   {
      /// <summary>
      /// Gets or sets the raw log line string.
      /// </summary>
      public virtual string Raw { get; set; }

      /// <summary>
      /// Gets or sets the index of the line in the log file.
      /// </summary>
      public virtual int Index { get; set; }

      /// <summary>
      /// Gets or sets the type of log line.
      /// </summary>
      public virtual LogLineType LineType { get; set; }

      /// <summary>
      /// Gets or sets the time stamp of the log line if the log line contains time information.
      /// </summary>
      public virtual TimeSpan? TimeStamp { get; set; }

      /// <summary>
      /// Gets or sets the object that contains data about the log line.
      /// </summary>
      public virtual object Data { get; set; }

      public override string ToString()
      {
         return Raw;
      }

      public static LogLine Create(string raw, int index, LogLineType lineType, TimeSpan? timeStamp, object data)
      {
         return new LogLine { Raw = raw, Index = index, LineType = lineType, TimeStamp = timeStamp, Data = data };
      }

      public static LogLine Create(string raw, int index, LogLineType lineType, LogLineTimeStampParserDelegate timeStampParser, LogLineDataParserDelegate dataParser)
      {
         return new Internal.LazyLogLine(timeStampParser, dataParser) { Raw = raw, Index = index, LineType = lineType };
      }
   }

   namespace Internal
   {
      internal class LazyLogLine : LogLine
      {
         private Lazy<TimeSpan?> _lazyTimeStampParser;
         private Lazy<object> _lazyDataParser;

         internal LazyLogLine(LogLineTimeStampParserDelegate timeStampParser, LogLineDataParserDelegate dataParser)
         {
            _lazyTimeStampParser = new Lazy<TimeSpan?>(() => timeStampParser(this));
            _lazyDataParser = new Lazy<object>(() => dataParser(this));
         }

         private TimeSpan? _timeStamp;

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
                           LineType = LogLineType.Error;
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
}
