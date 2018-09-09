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

/*
 * Based on code by Bryan Watts
 * http://stackoverflow.com/questions/353126/c-multiple-generic-types-in-one-list/1351071
 */

using System;
using System.Linq.Expressions;

using HFM.Preferences.Data;

namespace HFM.Preferences.Internal
{
   internal interface IMetadata
   {
      Type DataType { get; }

      object Data { get; set; }
   }

   internal interface IMetadata<T> : IMetadata
   {
      new T Data { get; set; }
   }

   // Original Metadata<T> class
   //public class Metadata<T> : IMetadata<T>
   //{
   //   public Metadata()
   //   {
   //      Data = default(T);
   //   }
   //
   //   public Metadata(T data)
   //   {
   //      Data = data;
   //   }
   //
   //   public Type DataType
   //   {
   //      get { return typeof(T); }
   //   }
   //
   //   object IMetadata.Data
   //   {
   //      get { return Data; }
   //   }
   //
   //   public T Data { get; set; }
   //}

   internal class ExpressionMetadata<T> : IMetadata<T>
   {
      private readonly PreferenceData _data;
      private readonly Func<PreferenceData, T> _getter;
      private readonly Action<PreferenceData, T> _setter;

      public ExpressionMetadata(PreferenceData data, Expression<Func<PreferenceData, T>> propertyExpression)
         : this(data, propertyExpression, false)
      {

      }

      public ExpressionMetadata(PreferenceData data, Expression<Func<PreferenceData, T>> propertyExpression, bool readOnly)
      {
         _data = data;
         _getter = propertyExpression.Compile();
         if (!readOnly)
         {
            _setter = propertyExpression.ToSetter();
         }
      }

      public Type DataType
      {
         get { return typeof(T); }
      }

      object IMetadata.Data
      {
         get { return Data; }
         set { Data = (T)value; }
      }

      public virtual T Data
      {
         get { return _getter(_data); }
         set
         {
            if (_setter == null)
            {
               throw new InvalidOperationException("Data is read-only.");
            }
            _setter(_data, value);
         }
      }
   }

   internal class EncryptedExpressionMetadata : ExpressionMetadata<string>
   {
      public EncryptedExpressionMetadata(PreferenceData data, Expression<Func<PreferenceData, string>> propertyExpression)
         : base(data, propertyExpression)
      {

      }

      public override string Data
      {
         get { return Cryptography.DecryptValue(base.Data); }
         set { base.Data = Cryptography.EncryptValue(value); }
      }
   }
}
