/*
 * HFM.NET - Generic Metadata Class and Interface
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
 * 
 * Based on code by Bryan Watts
 * http://stackoverflow.com/questions/353126/c-multiple-generic-types-in-one-list/1351071
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

namespace HFM.Framework
{
   public interface IMetadata
   {
      Type DataType { get; }

      object Data { get; }
   }

   public interface IMetadata<T> : IMetadata
   {
      new T Data { get; }
   }

   public class Metadata<T> : IMetadata<T>
   {
      public Metadata()
      {
         Data = default(T);
      }

      public Metadata(T data)
      {
         Data = data;
      }

      public Type DataType
      {
         get { return typeof(T); }
      }

      object IMetadata.Data
      {
         get { return Data; }
      }

      private T _Data;
      public T Data 
      {
         get { return _Data; }
         set { _Data = value; }
      }
   }
}
