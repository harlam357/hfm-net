/*
 * HFM.NET - Log Interpreter Base Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using System.Collections.Generic;
using System.Linq;

using HFM.Framework.DataTypes;

namespace HFM.Log
{
   public abstract class LogInterpreterBase
   {
      #region Fields

      private readonly IList<LogLine> _logLineList;

      protected IList<LogLine> LogLineList 
      { 
         get { return _logLineList; }
      }

      private readonly IList<ClientRun> _clientRunList;

      protected IList<ClientRun> ClientRunList
      {
         get { return _clientRunList; }
      }

      #endregion

      #region Properties

      /// <summary>
      /// Returns any data parsing error messages in the log lines.
      /// </summary>
      public IEnumerable<string> LogLineParsingErrors
      {
         get
         {
            return (from x in _logLineList
                    where x.LineType.Equals(LogLineType.Error) && x.LineData is String
                    select x.LineData).Cast<String>();

         }
      }

      /// <summary>
      /// Returns the most recent Client Run if available, otherwise null.
      /// </summary>
      public ClientRun CurrentClientRun
      {
         get { return _clientRunList.Count > 0 ? _clientRunList[_clientRunList.Count - 1] : null; }
      }

      #endregion

      #region Constructor

      protected LogInterpreterBase(IList<LogLine> logLines, IList<ClientRun> clientRuns)
      {
         if (logLines == null) throw new ArgumentNullException("logLines");
         if (clientRuns == null) throw new ArgumentNullException("clientRuns");

         _logLineList = logLines;
         _clientRunList = clientRuns;
      }

      #endregion
   }
}
