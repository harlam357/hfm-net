
// A simple, string-oriented wrapper class for encryption functions, including 
// Hashing, Symmetric Encryption, and Asymmetric Encryption.
//
//  Jeff Atwood
//   http://www.codinghorror.com/
//   http://www.codeproject.com/KB/security/SimpleEncryption.aspx

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace harlam357.Security.Encryption
{
   /// <summary>
   /// Symmetric encryption uses a single key to encrypt and decrypt. 
   /// Both parties (encryptor and decryptor) must share the same secret key.
   /// </summary>
   public class Symmetric
   {
      private const string _DefaultIntializationVector = "%2Bz=+@xT";
      private const int _BufferSize = 2048;

      public enum Provider
      {
         /// <summary>
         /// The Data Encryption Standard provider supports a 64 bit key only
         /// </summary>
         DES,
         /// <summary>
         /// The Rivest Cipher 2 provider supports keys ranging from 40 to 128 bits, default is 128 bits
         /// </summary>
         RC2,
         /// <summary>
         /// The Rijndael (also known as AES) provider supports keys of 128, 192, or 256 bits with a default of 256 bits
         /// </summary>
         Rijndael,
         /// <summary>
         /// The TripleDES provider (also known as 3DES) supports keys of 128 or 192 bits with a default of 192 bits
         /// </summary>
         TripleDES
      }

      private Data _key;
      private Data _iv;
      private SymmetricAlgorithm _crypto;

      private Symmetric()
      {
      }

      public Symmetric(Provider provider)
         : this(provider, true)
      {
      }

      /// <summary>
      /// Instantiates a new symmetric encryption object using the specified provider.
      /// </summary>
      public Symmetric(
          Provider provider,
          bool useDefaultInitializationVector)
      {
         switch (provider)
         {
            case Provider.DES:
               _crypto = new DESCryptoServiceProvider();
               break;
            case Provider.RC2:
               _crypto = new RC2CryptoServiceProvider();
               break;
            case Provider.Rijndael:
               _crypto = new RijndaelManaged();
               break;
            case Provider.TripleDES:
               _crypto = new TripleDESCryptoServiceProvider();
               break;
         }

         //-- make sure key and IV are always set, no matter what
         this.Key = RandomKey();
         if (useDefaultInitializationVector)
         {
            this.IntializationVector = new Data(_DefaultIntializationVector);
         }
         else
         {
            this.IntializationVector = RandomInitializationVector();
         }
      }

      /// <summary>
      /// Key size in bytes. We use the default key size for any given provider; if you 
      /// want to force a specific key size, set this property
      /// </summary>
      public int KeySizeBytes
      {
         get { return _crypto.KeySize / 8; }
         set
         {
            _crypto.KeySize = value * 8;
            _key.MaxBytes = value;
         }
      }

      /// <summary>
      /// Key size in bits. We use the default key size for any given provider; if you 
      /// want to force a specific key size, set this property
      /// </summary>
      public int KeySizeBits
      {
         get { return _crypto.KeySize; }
         set
         {
            _crypto.KeySize = value;
            _key.MaxBits = value;
         }
      }

      /// <summary>
      /// The key used to encrypt/decrypt data
      /// </summary>
      public Data Key
      {
         get { return _key; }
         set
         {
            _key = value;
            _key.MaxBytes = _crypto.LegalKeySizes[0].MaxSize / 8;
            _key.MinBytes = _crypto.LegalKeySizes[0].MinSize / 8;
            _key.StepBytes = _crypto.LegalKeySizes[0].SkipSize / 8;
         }
      }

      /// <summary>
      /// Using the default Cipher Block Chaining (CBC) mode, all data blocks are processed using
      /// the value derived from the previous block; the first data block has no previous data block
      /// to use, so it needs an InitializationVector to feed the first block
      /// </summary>
      public Data IntializationVector
      {
         get { return _iv; }
         set
         {
            _iv = value;
            _iv.MaxBytes = _crypto.BlockSize / 8;
            _iv.MinBytes = _crypto.BlockSize / 8;
         }
      }

      /// <summary>
      /// generates a random Initialization Vector, if one was not provided
      /// </summary>
      public Data RandomInitializationVector()
      {
         _crypto.GenerateIV();
         Data d = new Data(_crypto.IV);
         return d;
      }

      /// <summary>
      /// generates a random Key, if one was not provided
      /// </summary>
      public Data RandomKey()
      {
         _crypto.GenerateKey();
         Data d = new Data(_crypto.Key);
         return d;
      }

      /// <summary>
      /// Ensures that _crypto object has valid Key and IV
      /// prior to any attempt to encrypt/decrypt anything
      /// </summary>
      private void ValidateKeyAndIv(bool isEncrypting)
      {
         if (_key.IsEmpty)
         {
            if (isEncrypting)
            {
               _key = RandomKey();
            }
            else
            {
               throw new CryptographicException("No key was provided for the decryption operation!");
            }
         }
         if (_iv.IsEmpty)
         {
            if (isEncrypting)
            {
               _iv = RandomInitializationVector();
            }
            else
            {
               throw new CryptographicException("No initialization vector was provided for the decryption operation!");
            }
         }
         _crypto.Key = _key.Bytes;
         _crypto.IV = _iv.Bytes;
      }

      /// <summary>
      /// Encrypts the specified Data using provided key
      /// </summary>
      public Data Encrypt(Data d, Data key)
      {
         this.Key = key;
         return Encrypt(d);
      }

      /// <summary>
      /// Encrypts the specified Data using preset key and preset initialization vector
      /// </summary>
      public Data Encrypt(Data d)
      {
         MemoryStream ms = new MemoryStream();

         ValidateKeyAndIv(true);

         CryptoStream cs = new CryptoStream(ms, _crypto.CreateEncryptor(), CryptoStreamMode.Write);
         cs.Write(d.Bytes, 0, d.Bytes.Length);
         cs.Close();
         ms.Close();

         return new Data(ms.ToArray());
      }

      /// <summary>
      /// Encrypts the stream to memory using provided key and provided initialization vector
      /// </summary>
      public Data Encrypt(Stream s, Data key, Data iv)
      {
         this.IntializationVector = iv;
         this.Key = key;
         return Encrypt(s);
      }

      /// <summary>
      /// Encrypts the stream to memory using specified key
      /// </summary>
      public Data Encrypt(Stream s, Data key)
      {
         this.Key = key;
         return Encrypt(s);
      }

      /// <summary>
      /// Encrypts the specified stream to memory using preset key and preset initialization vector
      /// </summary>
      public Data Encrypt(Stream s)
      {
         MemoryStream ms = new MemoryStream();
         byte[] b = new byte[_BufferSize + 1];
         int i = 0;

         ValidateKeyAndIv(true);

         CryptoStream cs = new CryptoStream(ms, _crypto.CreateEncryptor(), CryptoStreamMode.Write);
         i = s.Read(b, 0, _BufferSize);
         while (i > 0)
         {
            cs.Write(b, 0, i);
            i = s.Read(b, 0, _BufferSize);
         }

         cs.Close();
         ms.Close();

         return new Data(ms.ToArray());
      }

      /// <summary>
      /// Decrypts the specified data using provided key and preset initialization vector
      /// </summary>
      public Data Decrypt(Data encryptedData, Data key)
      {
         this.Key = key;
         return Decrypt(encryptedData);
      }

      /// <summary>
      /// Decrypts the specified stream using provided key and preset initialization vector
      /// </summary>
      public Data Decrypt(Stream encryptedStream, Data key)
      {
         this.Key = key;
         return Decrypt(encryptedStream);
      }

      /// <summary>
      /// Decrypts the specified stream using preset key and preset initialization vector
      /// </summary>
      public Data Decrypt(Stream encryptedStream)
      {
         System.IO.MemoryStream ms = new System.IO.MemoryStream();
         byte[] b = new byte[_BufferSize + 1];

         ValidateKeyAndIv(false);
         CryptoStream cs = new CryptoStream(encryptedStream, _crypto.CreateDecryptor(), CryptoStreamMode.Read);

         int i = 0;
         i = cs.Read(b, 0, _BufferSize);

         while (i > 0)
         {
            ms.Write(b, 0, i);
            i = cs.Read(b, 0, _BufferSize);
         }
         cs.Close();
         ms.Close();

         return new Data(ms.ToArray());
      }

      /// <summary>
      /// Decrypts the specified data using preset key and preset initialization vector
      /// </summary>
      public Data Decrypt(Data encryptedData)
      {
         System.IO.MemoryStream ms = new System.IO.MemoryStream(encryptedData.Bytes, 0, encryptedData.Bytes.Length);
         byte[] b = new byte[encryptedData.Bytes.Length];

         ValidateKeyAndIv(false);
         CryptoStream cs = new CryptoStream(ms, _crypto.CreateDecryptor(), CryptoStreamMode.Read);

         try
         {
            cs.Read(b, 0, encryptedData.Bytes.Length - 1);
         }
         catch (CryptographicException ex)
         {
            throw new CryptographicException("Unable to decrypt data. The provided key may be invalid.", ex);
         }
         finally
         {
            cs.Close();
         }
         return new Data(b);
      }
   }
}
