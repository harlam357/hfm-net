
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HFM.Preferences.Support
{
   internal static class Cryptography
   {
      private const string IV = "3k1vKL=Cz6!wZS`I";
      private const string SymmetricKey = "%`Bb9ega;$.GUDaf";

      internal static string DecryptValue(string value)
      {
         if (String.IsNullOrWhiteSpace(value))
         {
            return String.Empty;
         }

         string result;

         try
         {
            result = DecryptInternal(value);
         }
         catch (FormatException)
         {
            // return the value as is
            result = value;
         }
         catch (CryptographicException)
         {
            // return the value as is
            result = value;
         }

         return result;
      }

      internal static string DecryptInternal(string value)
      {
         string plainText;
         using (var algorithm = CreateSymmetricAlgorithm())
         {
            using (var ms = new MemoryStream(Convert.FromBase64String(value)))
            {
               using (var cs = new CryptoStream(ms, algorithm.CreateDecryptor(), CryptoStreamMode.Read))
               {
                  using (var sr = new StreamReader(cs))
                  {
                     plainText = sr.ReadToEnd();
                  }
               }
            }

         }

         return plainText;
      }

      internal static string EncryptValue(string value)
      {
         if (String.IsNullOrWhiteSpace(value))
         {
            return String.Empty;
         }

         string result;

         try
         {
            result = EncryptInternal(value);
         }
         catch (CryptographicException)
         {
            // return the value as is
            result = value;
         }

         return result;
      }

      internal static string EncryptInternal(string value)
      {
         byte[] encrypted;
         using (var algorithm = CreateSymmetricAlgorithm())
         {
            using (var ms = new MemoryStream())
            {
               using (var cs = new CryptoStream(ms, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
               {
                  using (var sw = new StreamWriter(cs))
                  {
                     sw.Write(value);
                  }
                  encrypted = ms.ToArray();
               }
            }
         }

         return Convert.ToBase64String(encrypted);
      }

      private static SymmetricAlgorithm CreateSymmetricAlgorithm()
      {
         var rijndaelManaged = new RijndaelManaged();
         SetKey(rijndaelManaged, Encoding.UTF8.GetBytes(SymmetricKey));
         SetIV(rijndaelManaged, Encoding.UTF8.GetBytes(IV));
         return rijndaelManaged;
      }

      private static void SetKey(SymmetricAlgorithm algorithm, byte[] key)
      {
         int minBytes = algorithm.LegalKeySizes[0].MinSize / 8;
         int maxBytes = algorithm.LegalKeySizes[0].MaxSize / 8;
         algorithm.Key = ResizeByteArray(key, minBytes, maxBytes);
      }

      private static void SetIV(SymmetricAlgorithm algorithm, byte[] iv)
      {
         int minBytes = algorithm.BlockSize / 8;
         int maxBytes = algorithm.BlockSize / 8;
         algorithm.IV = ResizeByteArray(iv, minBytes, maxBytes);
      }

      private static byte[] ResizeByteArray(byte[] bytes, int minBytes, int  maxBytes)
      {
         if (maxBytes > 0)
         {
            if (bytes.Length > maxBytes)
            {
               var b = new byte[maxBytes];
               Array.Copy(bytes, b, b.Length);
               bytes = b;
            }
         }
         if (minBytes > 0)
         {
            if (bytes.Length < minBytes)
            {
               var b = new byte[minBytes];
               Array.Copy(bytes, b, bytes.Length);
               bytes = b;
            }
         }
         return bytes;
      }
   }
}
