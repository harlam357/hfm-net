
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using HFM.Framework.DataTypes;
using ProtoBuf;

using HFM.Framework;

namespace HFM.Instances
{
   public interface IQueryParameterContainer
   {
      List<QueryParameters> QueryList { get; }

      /// <summary>
      /// Read Binary File
      /// </summary>
      void Read();

      /// <summary>
      /// Write Binary File
      /// </summary>
      void Write();
   }

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
         string filePath = Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), QueryFilename);

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
         Serialize(_queryList, Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), QueryFilename));
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