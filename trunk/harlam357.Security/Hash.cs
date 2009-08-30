
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

namespace harlam357.Security
{
   /// <summary>
   /// Hash functions are fundamental to modern cryptography. These functions map binary 
   /// strings of an arbitrary length to small binary strings of a fixed length, known as 
   /// hash values. A cryptographic hash function has the property that it is computationally
   /// infeasible to find two distinct inputs that hash to the same value. Hash functions 
   /// are commonly used with digital signatures and for data integrity.
   /// </summary>
   public class Hash
   {
      /// <summary>
      /// Type of hash; some are security oriented, others are fast and simple
      /// </summary>
      public enum Provider
      {
         /// <summary>
         /// Cyclic Redundancy Check provider, 32-bit
         /// </summary>
         CRC32,
         /// <summary>
         /// Secure Hashing Algorithm provider, SHA-1 variant, 160-bit
         /// </summary>
         SHA1,
         /// <summary>
         /// Secure Hashing Algorithm provider, SHA-2 variant, 256-bit
         /// </summary>
         SHA256,
         /// <summary>
         /// Secure Hashing Algorithm provider, SHA-2 variant, 384-bit
         /// </summary>
         SHA384,
         /// <summary>
         /// Secure Hashing Algorithm provider, SHA-2 variant, 512-bit
         /// </summary>
         SHA512,
         /// <summary>
         /// Message Digest algorithm 5, 128-bit
         /// </summary>
         MD5
      }

      private HashAlgorithm _Hash;
      private Data _HashValue = new Data();

      private Hash()
      {
      }

      /// <summary>
      /// Instantiate a new hash of the specified type
      /// </summary>
      public Hash(Provider p)
      {
         switch (p)
         {
            case Provider.CRC32:
               _Hash = new CRC32();
               break;
            case Provider.MD5:
               _Hash = new MD5CryptoServiceProvider();
               break;
            case Provider.SHA1:
               _Hash = new SHA1Managed();
               break;
            case Provider.SHA256:
               _Hash = new SHA256Managed();
               break;
            case Provider.SHA384:
               _Hash = new SHA384Managed();
               break;
            case Provider.SHA512:
               _Hash = new SHA512Managed();
               break;
         }
      }

      /// <summary>
      /// Returns the previously calculated hash
      /// </summary>
      public Data Value
      {
         get { return _HashValue; }
      }

      /// <summary>
      /// Calculates hash on a stream of arbitrary length
      /// </summary>
      public Data Calculate(ref System.IO.Stream s)
      {
         _HashValue.Bytes = _Hash.ComputeHash(s);
         return _HashValue;
      }

      /// <summary>
      /// Calculates hash for fixed length <see cref="Data"/>
      /// </summary>
      public Data Calculate(Data d)
      {
         return CalculatePrivate(d.Bytes);
      }

      /// <summary>
      /// Calculates hash for a string with a prefixed salt value. 
      /// A "salt" is random data prefixed to every hashed value to prevent 
      /// common dictionary attacks.
      /// </summary>
      public Data Calculate(Data d, Data salt)
      {
         byte[] nb = new byte[d.Bytes.Length + salt.Bytes.Length];
         salt.Bytes.CopyTo(nb, 0);
         d.Bytes.CopyTo(nb, salt.Bytes.Length);
         return CalculatePrivate(nb);
      }

      /// <summary>
      /// Calculates hash for an array of bytes
      /// </summary>
      private Data CalculatePrivate(byte[] b)
      {
         _HashValue.Bytes = _Hash.ComputeHash(b);
         return _HashValue;
      }

      #region CRC32 HashAlgorithm
      private class CRC32 : HashAlgorithm
      {
         public const UInt32 DefaultPolynomial = 0xedb88320;
         public const UInt32 DefaultSeed = 0xffffffff;

         private UInt32 hash;
         private UInt32 seed;
         private UInt32[] table;
         private static UInt32[] defaultTable;

         public CRC32()
         {
            table = InitializeTable(DefaultPolynomial);
            seed = DefaultSeed;
            Initialize();
         }

         public CRC32(UInt32 polynomial, UInt32 seed)
         {
            table = InitializeTable(polynomial);
            this.seed = seed;
            Initialize();
         }

         public override void Initialize()
         {
            hash = seed;
         }

         protected override void HashCore(byte[] buffer, int start, int length)
         {
            hash = CalculateHash(table, hash, buffer, start, length);
         }

         protected override byte[] HashFinal()
         {
            byte[] hashBuffer = UInt32ToBigEndianBytes(~hash);
            this.HashValue = hashBuffer;
            return hashBuffer;
         }

         public override int HashSize
         {
            get { return 32; }
         }

         public static UInt32 Compute(byte[] buffer)
         {
            return ~CalculateHash(InitializeTable(DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
         }

         public static UInt32 Compute(UInt32 seed, byte[] buffer)
         {
            return ~CalculateHash(InitializeTable(DefaultPolynomial), seed, buffer, 0, buffer.Length);
         }

         public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
         {
            return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
         }

         private static UInt32[] InitializeTable(UInt32 polynomial)
         {
            if (polynomial == DefaultPolynomial && defaultTable != null)
               return defaultTable;

            UInt32[] createTable = new UInt32[256];
            for (int i = 0; i < 256; i++)
            {
               UInt32 entry = (UInt32)i;
               for (int j = 0; j < 8; j++)
                  if ((entry & 1) == 1)
                     entry = (entry >> 1) ^ polynomial;
                  else
                     entry = entry >> 1;
               createTable[i] = entry;
            }

            if (polynomial == DefaultPolynomial)
               defaultTable = createTable;

            return createTable;
         }

         private static UInt32 CalculateHash(UInt32[] table, UInt32 seed, byte[] buffer, int start, int size)
         {
            UInt32 crc = seed;
            for (int i = start; i < size; i++)
               unchecked
               {
                  crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
               }
            return crc;
         }

         private byte[] UInt32ToBigEndianBytes(UInt32 x)
         {
            return new byte[] {
			      (byte)((x >> 24) & 0xff),
			      (byte)((x >> 16) & 0xff),
			      (byte)((x >> 8) & 0xff),
			      (byte)(x & 0xff)
            };
         }

      }

      #endregion
   }
}
