/*
 * HFM.NET - Core Data Container
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

using Castle.Core.Logging;

namespace HFM.Core
{
   public abstract class DataContainer<T> where T : class, new()
   {
      private T _data;

      protected internal T Data
      {
         get { return _data; }
         set { _data = value ?? new T(); }
      }

      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      public string FileName { get; set; }

      public abstract Plugins.IFileSerializer<T> DefaultSerializer { get; }

      protected DataContainer()
      {
         Data = new T();
      }

      #region Serialization Support

      private static readonly object SerializeLock = new object();

      /// <summary>
      /// Read data file.
      /// </summary>
      public virtual void Read()
      {
         //DateTime start = HfmTrace.ExecStart;

         T data = null;

         lock (SerializeLock)
         {
            try
            {
               data = DefaultSerializer.Deserialize(FileName);
            }
            catch (Exception ex)
            {
               Logger.ErrorFormat(ex, "{0}", ex.Message);
            }
         }

         //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);

         Data = data ?? new T();
      }

      /// <summary>
      /// Read data file.
      /// </summary>
      public virtual T Read(string filePath, Plugins.IFileSerializer<T> serializer)
      {
         //DateTime start = HfmTrace.ExecStart;

         lock (SerializeLock)
         {
            return serializer.Deserialize(filePath);
         }

         //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
      }

      /// <summary>
      /// Write data file.
      /// </summary>
      public virtual void Write()
      {
         //DateTime start = HfmTrace.ExecStart;

         lock (SerializeLock)
         {
            try
            {
               DefaultSerializer.Serialize(FileName, Data);
            }
            catch (Exception ex)
            {
               Logger.ErrorFormat(ex, "{0}", ex.Message);
            }
         }

         //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
      }

      /// <summary>
      /// Write data file.
      /// </summary>
      public virtual void Write(string filePath, Plugins.IFileSerializer<T> serializer)
      {
         //DateTime start = HfmTrace.ExecStart;

         lock (SerializeLock)
         {
            serializer.Serialize(filePath, Data);
         }

         //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
      }

      #endregion
   }
}
