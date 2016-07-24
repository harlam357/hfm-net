
using System;
using System.Security.Cryptography;

using harlam357.Core.Security;
using harlam357.Core.Security.Cryptography;

namespace HFM.Preferences
{
   internal static class Cryptography
   {
      // ReSharper disable once InconsistentNaming
      private static readonly SymmetricKeyData IV = new SymmetricKeyData("3k1vKL=Cz6!wZS`I");
      private static readonly SymmetricKeyData SymmetricKey = new SymmetricKeyData("%`Bb9ega;$.GUDaf");

      internal static string DecryptValue(string value)
      {
         if (String.IsNullOrWhiteSpace(value))
         {
            return String.Empty;
         }

         using (var symmetricProvider = new Symmetric(SymmetricProvider.Rijndael, false))
         {
            string result;

            try
            {
               symmetricProvider.IntializationVector = IV;
               result = symmetricProvider.Decrypt(new Data(value.FromBase64()), SymmetricKey).ToString();
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
      }

      internal static string EncryptValue(string value)
      {
         if (String.IsNullOrWhiteSpace(value))
         {
            return String.Empty;
         }

         using (var symmetricProvider = new Symmetric(SymmetricProvider.Rijndael, false))
         {
            string result;

            try
            {
               symmetricProvider.IntializationVector = IV;
               result = symmetricProvider.Encrypt(new Data(value), SymmetricKey).Bytes.ToBase64();
            }
            catch (CryptographicException)
            {
               // return the value as is
               result = value;
            }

            return result;
         }
      }
   }
}
