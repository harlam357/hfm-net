/*
 * HFM.NET - Query Parameter Container Class
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */
 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ProtoBuf;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances
{
   public class QueryParameterContainer : IQueryParameterContainer
   {
      private const string QueryFilename = "WuHistoryQuery.dat";
   
      private readonly IPreferenceSet _prefs;
      private QueryParameterList _queryList;
      
      public List<QueryParameters> QueryList
      {
         get { return _queryList.QueryList; }
      }
      
      public QueryParameterContainer(IPreferenceSet prefs)
      {
         _prefs = prefs;
         //_queryList = New();
         Read();
      }
      
      private static QueryParameterList New()
      {
         var list = new QueryParameterList();
         list.QueryList.Add(new QueryParameters());
         return list;
      }
   
      #region Serialization Support

      /// <summary>
      /// Read Binary File
      /// </summary>
      public void Read()
      {
         string filePath = Path.Combine(_prefs.ApplicationDataFolderPath, QueryFilename);

         _queryList = Deserialize(filePath);
         if (_queryList == null)
         {
            _queryList = New();
         }
      }

      /// <summary>
      /// Write Binary File
      /// </summary>
      public void Write()
      {
         Serialize(_queryList, Path.Combine(_prefs.ApplicationDataFolderPath, QueryFilename));
      }

      private static readonly object SerializeLock = typeof(QueryParameterList);

      public static void Serialize(QueryParameterList list, string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         lock (SerializeLock)
         {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
               try
               {
                  Serializer.Serialize(fileStream, list);
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(ex);
               }
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
      }

      public static QueryParameterList Deserialize(string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         QueryParameterList list = null;
         try
         {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
               list = Serializer.Deserialize<QueryParameterList>(fileStream);
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);

         return list;
      }

      #endregion
   }

   [ProtoContract]
   public class QueryParameterList
   {
      [ProtoMember(1)]
      private readonly List<QueryParameters> _queryList = new List<QueryParameters>();

      public List<QueryParameters> QueryList
      {
         get { return _queryList; }
      }
   }
}