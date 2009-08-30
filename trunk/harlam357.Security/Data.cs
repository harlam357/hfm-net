
// A simple, string-oriented wrapper class for encryption functions, including 
// Hashing, Symmetric Encryption, and Asymmetric Encryption.
//
//  Jeff Atwood
//   http://www.codinghorror.com/
//   http://www.codeproject.com/KB/security/SimpleEncryption.aspx

using System;
using System.Collections.Generic;
using System.Text;

namespace harlam357.Security
{
   /// <summary>
   /// represents Hex, Byte, Base64, or String data to encrypt/decrypt;
   /// use the .Text property to set/get a string representation 
   /// use the .Hex property to set/get a string-based Hexadecimal representation 
   /// use the .Base64 to set/get a string-based Base64 representation 
   /// </summary>
   public class Data
   {
      private byte[] _b;
      private int _MaxBytes = 0;
      private int _MinBytes = 0;
      private int _StepBytes = 0;

      /// <summary>
      /// Determines the default text encoding across ALL Data instances
      /// </summary>
      public static Encoding DefaultEncoding = System.Text.Encoding.GetEncoding("Windows-1252");

      /// <summary>
      /// Determines the default text encoding for this Data instance
      /// </summary>
      public Encoding Encoding = DefaultEncoding;

      /// <summary>
      /// Creates new, empty encryption data
      /// </summary>
      public Data()
      {
      }

      /// <summary>
      /// Creates new encryption data with the specified byte array
      /// </summary>
      public Data(byte[] b)
      {
         _b = b;
      }

      /// <summary>
      /// Creates new encryption data with the specified string; 
      /// will be converted to byte array using default encoding
      /// </summary>
      public Data(string s)
      {
         this.Text = s;
      }

      /// <summary>
      /// Creates new encryption data using the specified string and the 
      /// specified encoding to convert the string to a byte array.
      /// </summary>
      public Data(string s, System.Text.Encoding encoding)
      {
         this.Encoding = encoding;
         this.Text = s;
      }

      /// <summary>
      /// returns true if no data is present
      /// </summary>
      public bool IsEmpty
      {
         get
         {
            if (_b == null)
            {
               return true;
            }
            if (_b.Length == 0)
            {
               return true;
            }
            return false;
         }
      }

      /// <summary>
      /// allowed step interval, in bytes, for this data; if 0, no limit
      /// </summary>
      public int StepBytes
      {
         get { return _StepBytes; }
         set { _StepBytes = value; }
      }

      /// <summary>
      /// allowed step interval, in bits, for this data; if 0, no limit
      /// </summary>
      public int StepBits
      {
         get { return _StepBytes * 8; }
         set { _StepBytes = value / 8; }
      }

      /// <summary>
      /// minimum number of bytes allowed for this data; if 0, no limit
      /// </summary>
      public int MinBytes
      {
         get { return _MinBytes; }
         set { _MinBytes = value; }
      }

      /// <summary>
      /// minimum number of bits allowed for this data; if 0, no limit
      /// </summary>
      public int MinBits
      {
         get { return _MinBytes * 8; }
         set { _MinBytes = value / 8; }
      }

      /// <summary>
      /// maximum number of bytes allowed for this data; if 0, no limit
      /// </summary>
      public int MaxBytes
      {
         get { return _MaxBytes; }
         set { _MaxBytes = value; }
      }

      /// <summary>
      /// maximum number of bits allowed for this data; if 0, no limit
      /// </summary>
      public int MaxBits
      {
         get { return _MaxBytes * 8; }
         set { _MaxBytes = value / 8; }
      }

      /// <summary>
      /// Returns the byte representation of the data; 
      /// This will be padded to MinBytes and trimmed to MaxBytes as necessary!
      /// </summary>
      public byte[] Bytes
      {
         get
         {
            if (_MaxBytes > 0)
            {
               if (_b.Length > _MaxBytes)
               {
                  byte[] b = new byte[_MaxBytes];
                  Array.Copy(_b, b, b.Length);
                  _b = b;
               }
            }
            if (_MinBytes > 0)
            {
               if (_b.Length < _MinBytes)
               {
                  byte[] b = new byte[_MinBytes];
                  Array.Copy(_b, b, _b.Length);
                  _b = b;
               }
            }
            return _b;
         }
         set { _b = value; }
      }

      /// <summary>
      /// Sets or returns text representation of bytes using the default text encoding
      /// </summary>
      public string Text
      {
         get
         {
            if (_b == null)
            {
               return "";
            }
            else
            {
               //-- need to handle nulls here; oddly, C# will happily convert
               //-- nulls into the string whereas VB stops converting at the
               //-- first null!
               int i = Array.IndexOf(_b, (byte)0);
               if (i >= 0)
               {
                  return this.Encoding.GetString(_b, 0, i);
               }
               else
               {
                  return this.Encoding.GetString(_b);
               }
            }
         }
         set { _b = this.Encoding.GetBytes(value); }
      }

      /// <summary>
      /// Sets or returns Hex string representation of this data
      /// </summary>
      public string Hex
      {
         get { return Utils.ToHex(_b); }
         set { _b = Utils.FromHex(value); }
      }

      /// <summary>
      /// Sets or returns Base64 string representation of this data
      /// </summary>
      public string Base64
      {
         get { return Utils.ToBase64(_b); }
         set { _b = Utils.FromBase64(value); }
      }

      /// <summary>
      /// Returns text representation of bytes using the default text encoding
      /// </summary>
      public new string ToString()
      {
         return this.Text;
      }

      /// <summary>
      /// returns Base64 string representation of this data
      /// </summary>
      public string ToBase64()
      {
         return this.Base64;
      }

      /// <summary>
      /// returns Hex string representation of this data
      /// </summary>
      public string ToHex()
      {
         return this.Hex;
      }
   }
}
