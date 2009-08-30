
// A simple, string-oriented wrapper class for encryption functions, including 
// Hashing, Symmetric Encryption, and Asymmetric Encryption.
//
//  Jeff Atwood
//   http://www.codinghorror.com/
//   http://www.codeproject.com/KB/security/SimpleEncryption.aspx

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;

namespace harlam357.Security
{
   /// <summary>
   /// Friend class for shared utility methods used by multiple Encryption classes
   /// </summary>
   public static class Utils
   {
      /// <summary>
      /// converts an array of bytes to a string Hex representation
      /// </summary>
      static public string ToHex(byte[] ba)
      {
         if (ba == null || ba.Length == 0)
         {
            return "";
         }
         const string HexFormat = "{0:X2}";
         StringBuilder sb = new StringBuilder();
         foreach (byte b in ba)
         {
            sb.Append(string.Format(HexFormat, b));
         }
         return sb.ToString();
      }

      /// <summary>
      /// converts from a string Hex representation to an array of bytes
      /// </summary>
      static public byte[] FromHex(string hexEncoded)
      {
         if (hexEncoded == null || hexEncoded.Length == 0)
         {
            return null;
         }
         try
         {
            int l = Convert.ToInt32(hexEncoded.Length / 2);
            byte[] b = new byte[l];
            for (int i = 0; i <= l - 1; i++)
            {
               b[i] = Convert.ToByte(hexEncoded.Substring(i * 2, 2), 16);
            }
            return b;
         }
         catch (Exception ex)
         {
            throw new System.FormatException("The provided string does not appear to be Hex encoded:" + Environment.NewLine + hexEncoded + Environment.NewLine, ex);
         }
      }

      /// <summary>
      /// converts from a string Base64 representation to an array of bytes
      /// </summary>
      static public byte[] FromBase64(string base64Encoded)
      {
         if (base64Encoded == null || base64Encoded.Length == 0)
         {
            return null;
         }
         try
         {
            return Convert.FromBase64String(base64Encoded);
         }
         catch (System.FormatException ex)
         {
            throw new System.FormatException("The provided string does not appear to be Base64 encoded:" + Environment.NewLine + base64Encoded + Environment.NewLine, ex);
         }
      }

      /// <summary>
      /// converts from an array of bytes to a string Base64 representation
      /// </summary>
      static public string ToBase64(byte[] b)
      {
         if (b == null || b.Length == 0)
         {
            return "";
         }
         return Convert.ToBase64String(b);
      }

      /// <summary>
      /// retrieve an element from an XML string
      /// </summary>
      static internal string GetXmlElement(string xml, string element)
      {
         Match m = default(Match);
         m = Regex.Match(xml, "<" + element + ">(?<Element>[^>]*)</" + element + ">", RegexOptions.IgnoreCase);
         if (m == null)
         {
            throw new Exception("Could not find <" + element + "></" + element + "> in provided Public Key XML.");
         }
         return m.Groups["Element"].ToString();
      }

      static internal string GetConfigString(string key)
      {
         return GetConfigString(key, false);
      }

      /// <summary>
      /// Returns the specified string value from the application .config file
      /// </summary>
      static internal string GetConfigString(
          string key,
          bool isRequired)
      {

         string s = (string)ConfigurationManager.AppSettings.Get(key);
         if (s == null)
         {
            if (isRequired)
            {
               throw new ConfigurationErrorsException("key <" + key + "> is missing from .config file");
            }
            else
            {
               return "";
            }
         }
         else
         {
            return s;
         }
      }

      static internal string WriteConfigKey(string key, string value)
      {
         string s = "<add key=\"{0}\" value=\"{1}\" />" + Environment.NewLine;
         return string.Format(s, key, value);
      }

      static internal string WriteXmlElement(string element, string value)
      {
         string s = "<{0}>{1}</{0}>" + Environment.NewLine;
         return string.Format(s, element, value);
      }

      static internal string WriteXmlNode(string element)
      {
         return WriteXmlNode(element, false);
      }

      static internal string WriteXmlNode(
          string element,
          bool isClosing)
      {
         string s = null;
         if (isClosing)
         {
            s = "</{0}>" + Environment.NewLine;
         }
         else
         {
            s = "<{0}>" + Environment.NewLine;
         }
         return string.Format(s, element);
      }
   }
}
