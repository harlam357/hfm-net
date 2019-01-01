/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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

namespace HFM.Core.Data
{
   public abstract class DataContainer<T> where T : class, new()
   {
      private T _data;

      protected internal T Data
      {
         get { return _data; }
         set { _data = value ?? new T(); }
      }

      private ILogger _logger;

      public ILogger Logger
      {
         get { return _logger ?? (_logger = NullLogger.Instance); }
         set { _logger = value; }
      }

      public string FileName { get; set; }

      public abstract Serializers.IFileSerializer<T> DefaultSerializer { get; }

      protected DataContainer()
      {
         Data = new T();
      }

      #region Serialization Support

      private readonly object _serializeLock = new object();

      /// <summary>
      /// Read data file.
      /// </summary>
      public virtual void Read()
      {
         T data = null;

         lock (_serializeLock)
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

         Data = data ?? new T();
      }

      /// <summary>
      /// Write data file.
      /// </summary>
      public virtual void Write()
      {
         lock (_serializeLock)
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
      }

      #endregion
   }
}
