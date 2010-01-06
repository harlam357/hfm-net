
/// <summary>
/// unit tests for the Encryption class to verify correct operation
/// see http://www.nunit.org/index.html
/// </summary>
/// <remarks>
///   Jeff Atwood
///   http://www.codinghorror.com/
/// </remarks>

using System.IO;
using System.Text;

using NUnit.Framework;

using harlam357.Security;

namespace harlam357.Security.Tests
{
   [TestFixture()]
   public class SecurityTests
   {
      private string _TargetString;
      private string _TargetString2;

      [TestFixtureSetUp()]
      public void Setup()
      {
         _TargetString = "The instinct of nearly all societies is to lock up anybody who is truly free. " + 
            "First, society begins by trying to beat you up. If this fails, they try to poison you. " + 
            "If this fails too, they finish by loading honors on your head." +
            " - Jean Cocteau (1889-1963)";

         _TargetString2 = "Everything should be made as simple as possible, but not simpler. - Albert Einstein";
      }

      #region Hash Tests
      [Test(), Category("Hash")]
      public void SaltedHashes()
      {
         Assert.AreEqual("6CD9DD96", DoSaltedHash(Hash.Provider.CRC32, new Data("Shazam!")).ToHex());
         Assert.AreEqual("4F7FA9C182C5FA60F9197F4830296685", DoSaltedHash(Hash.Provider.MD5, new Data("SnapCracklePop")).ToHex());
         Assert.AreEqual("3DC330B4E4E61C8DF039EAE93EC16412E22425FB", DoSaltedHash(Hash.Provider.SHA1, new Data("全球最大的華文新聞網站", Encoding.Unicode)).ToHex());
         Assert.AreEqual("EFAE307AEE511D6078FDF0D4372F4D0C8135170C5F7626CB19B04BFDBABBBDB2", DoSaltedHash(Hash.Provider.SHA256, new Data("!@#$%^&*()_-+=", Encoding.ASCII)).ToHex());
         Assert.AreEqual("582B31C13EF16D706EC2514FDA08316A369DF1F130D34A0A2A16B065D82662A1101EA01110AB7C8F9022A1CEA76FD6B9", DoSaltedHash(Hash.Provider.SHA384, new Data("supercalifragilisticexpialidocious", Encoding.ASCII)).ToHex());
         Assert.AreEqual("44FAA06E8E80666408304E3458621769699A76B591C6389F958C0DDA1D80A82965D169E8AA7D3C1A0637BCB7B0F45D420389C629D19E255D64A923F6C4F87FD8", DoSaltedHash(Hash.Provider.SHA512, new Data("42", Encoding.ASCII)).ToHex());
      }

      [Test(), Category("Hash")]
      public void HashFile()
      {
         string hashHex;

         Hash h1 = new Hash(Hash.Provider.CRC32);
         using (StreamReader sr = new StreamReader("gettysburg.txt"))
         {
            Stream baseStream = sr.BaseStream;
            hashHex = h1.Calculate(ref baseStream).ToHex();
         }
         Assert.AreEqual(hashHex, "E37F6423");

         Hash h2 = new Hash(Hash.Provider.MD5);
         using (StreamReader sr = new StreamReader("sample.doc"))
         {
            Stream baseStream = sr.BaseStream;
            hashHex = h2.Calculate(ref baseStream).ToHex();
         }
         Assert.AreEqual(hashHex, "4F32AB797F0FCC782AAC0B4F4E5B1693");
      }

      [Test(), Category("Hash")]
      public void Hashes()
      {
         Assert.AreEqual("AA692113", DoHash(Hash.Provider.CRC32).ToHex());
         Assert.AreEqual("44D36517B0CCE797FF57118ABE264FD9", DoHash(Hash.Provider.MD5).ToHex());
         Assert.AreEqual("9E93AB42BCC8F738C7FBB6CCA27A902DC663DBE1", DoHash(Hash.Provider.SHA1).ToHex());
         Assert.AreEqual("40AF07ABFE970590B2C313619983651B1E7B2F8C2D855C6FD4266DAFD7A5E670", DoHash(Hash.Provider.SHA256).ToHex());
         Assert.AreEqual("9FC0AFB3DA61201937C95B133AB397FE62C329D6061A8768DA2B9D09923F07624869D01CD76826E1152DAB7BFAA30915", DoHash(Hash.Provider.SHA384).ToHex());
         Assert.AreEqual("2E7D4B051DD528F3E9339E0927930007426F4968B5A4A08349472784272F17DA5C532EDCFFE14934988503F77DEF4AB58EB05394838C825632D04A10F42A753B", DoHash(Hash.Provider.SHA512).ToHex());
      }

      private Data DoSaltedHash(Hash.Provider p, Data salt)
      {
         Hash h = new Hash(p);
         return h.Calculate(new Data(_TargetString), salt);
      }

      private Data DoHash(Hash.Provider p)
      {
         Hash h = new Hash(p);
         return h.Calculate(new Data(_TargetString));
      } 
      #endregion

      #region Asymmetric Tests
      [Test(), Category("Asymmetric")]
      public void Asymmetric()
      {
         string secret = "Pack my box with five dozen liquor jugs.";
         Assert.AreEqual(secret, AsymmetricNewKey(secret));
         Assert.AreEqual(secret, AsymmetricNewKey(secret, 384));
         Assert.AreEqual(secret, AsymmetricNewKey(secret, 512));
         Assert.AreEqual(secret, AsymmetricNewKey(secret, 1024));
         Assert.AreEqual(secret, AsymmetricConfigKey(secret));
         Assert.AreEqual(secret, AsymmetricXmlKey(secret));
      }

      private string AsymmetricXmlKey(string secret)
      {
         string publicKeyXml = "<RSAKeyValue>" + "<Modulus>0D59Km2Eo9oopcm7Y2wOXx0TRRXQFybl9HHe/ve47Qcf2EoKbs9nkuMmhCJlJzrq6ZJzgQSEbpVyaWn8OHq0I50rQ13dJsALEquhlfwVWw6Hit7qRvveKlOAGfj8xdkaXJLYS1tA06tKHfYxgt6ysMBZd0DIedYoE1fe3VlLZyE=</Modulus>" + "<Exponent>AQAB</Exponent>" + "</RSAKeyValue>";

         string privateKeyXml = "<RSAKeyValue>" + "<Modulus>0D59Km2Eo9oopcm7Y2wOXx0TRRXQFybl9HHe/ve47Qcf2EoKbs9nkuMmhCJlJzrq6ZJzgQSEbpVyaWn8OHq0I50rQ13dJsALEquhlfwVWw6Hit7qRvveKlOAGfj8xdkaXJLYS1tA06tKHfYxgt6ysMBZd0DIedYoE1fe3VlLZyE=</Modulus>" + "<Exponent>AQAB</Exponent>" + "<P>/1cvDks8qlF1IXKNwcXW8tjTlhjidjGtbT9k7FCYug+P6ZBDfqhUqfvjgLFF/+dAkoofNqliv89b8DRy4gS4qQ==</P>" + "<Q>0Mgq7lyvmVPR1r197wnba1bWbJt8W2Ki8ilUN6lX6Lkk04ds9y3A0txy0ESya7dyg9NLscfU3NQMH8RRVnJtuQ==</Q>" + "<DP>+uwfRumyxSDlfSgInFqh/+YKD5+GtGXfKtO4hu4xF+8BGqJ1YXtkL+Njz2zmADOt5hOr1tigPSQ2EhhIqUnAeQ==</DP>" + "<DQ>M5Ofd28SOjCIwCHjwG+Q8v1qzz3CBNljI6uuEGoXO3ixbkggVRfKcMzg2C6AXTfeZE6Ifoy9OyhvLlHTPiXakQ==</DQ>" + "<InverseQ>yQIJMLdL6kU4VK7M5b5PqWS8XzkgxfnaowRs9mhSEDdwwWPtUXO8aQ9G3zuiDUqNq9j5jkdt77+c2stBdV97ew==</InverseQ>" + "<D>HOpQXu/OFyJXuo2EY43BgRt8bX9V4aEZFRQqrqSfHOp8VYASasiJzS+VTYupGAVqUPxw5V1HNkOyG0kIKJ+BG6BpIwLIbVKQn/ROs7E3/vBdg2+QXKhikMz/4gYx2oEsXW2kzN1GMRop2lrrJZJNGE/eG6i4lQ1/inj1Tk/sqQE=</D>" + "</RSAKeyValue>";

         Data encryptedData;
         Data decryptedData;
         Encryption.Asymmetric asym = new Encryption.Asymmetric();
         Encryption.Asymmetric asym2 = new Encryption.Asymmetric();

         encryptedData = asym.Encrypt(new Data(secret), publicKeyXml);
         decryptedData = asym2.Decrypt(encryptedData, privateKeyXml);

         return decryptedData.ToString();
      }

      private string AsymmetricConfigKey(string secret)
      {
         Data encryptedData;
         Data decryptedData;
         Encryption.Asymmetric asym = new Encryption.Asymmetric();
         Encryption.Asymmetric asym2 = new Encryption.Asymmetric();

         encryptedData = asym.Encrypt(new Data(secret));
         decryptedData = asym2.Decrypt(encryptedData);

         return decryptedData.ToString();
      }

      private string AsymmetricNewKey(string secret)
      {
         return AsymmetricNewKey(secret, 0);
      }

      private string AsymmetricNewKey(string secret, int keysize)
      {
         Encryption.Asymmetric.PublicKey pubkey = new Encryption.Asymmetric.PublicKey();
         Encryption.Asymmetric.PrivateKey privkey = new Encryption.Asymmetric.PrivateKey();
         Data encryptedData;
         Data decryptedData;
         Encryption.Asymmetric asym = new Encryption.Asymmetric();
         Encryption.Asymmetric asym2 = new Encryption.Asymmetric();

         if (keysize == 0)
         {
            asym = new Encryption.Asymmetric();
            asym2 = new Encryption.Asymmetric();
         }
         else
         {
            asym = new Encryption.Asymmetric(keysize);
            asym2 = new Encryption.Asymmetric(keysize);
         }
         asym.GenerateNewKeyset(ref pubkey, ref privkey);

         encryptedData = asym.Encrypt(new Data(secret), pubkey);
         decryptedData = asym2.Decrypt(encryptedData, privkey);

         return decryptedData.ToString();
      } 
      #endregion

      #region Symmetric Tests
      [Test(), Category("Symmetric")]
      public void Symmetric()
      {
         Assert.AreEqual(_TargetString, SymmetricLoopback(Encryption.Symmetric.Provider.DES, _TargetString));
         Assert.AreEqual(_TargetString, SymmetricWithKey(Encryption.Symmetric.Provider.DES, _TargetString));
         Assert.AreEqual(_TargetString, SymmetricLoopback(Encryption.Symmetric.Provider.RC2, _TargetString));
         Assert.AreEqual(_TargetString, SymmetricWithKey(Encryption.Symmetric.Provider.RC2, _TargetString));
         Assert.AreEqual(_TargetString, SymmetricLoopback(Encryption.Symmetric.Provider.Rijndael, _TargetString));
         Assert.AreEqual(_TargetString, SymmetricWithKey(Encryption.Symmetric.Provider.Rijndael, _TargetString));
         Assert.AreEqual(_TargetString, SymmetricLoopback(Encryption.Symmetric.Provider.TripleDES, _TargetString));
         Assert.AreEqual(_TargetString, SymmetricWithKey(Encryption.Symmetric.Provider.TripleDES, _TargetString));

         Assert.AreEqual(_TargetString2, SymmetricLoopback(Encryption.Symmetric.Provider.DES, _TargetString2));
         Assert.AreEqual(_TargetString2, SymmetricWithKey(Encryption.Symmetric.Provider.DES, _TargetString2));
         Assert.AreEqual(_TargetString2, SymmetricLoopback(Encryption.Symmetric.Provider.RC2, _TargetString2));
         Assert.AreEqual(_TargetString2, SymmetricWithKey(Encryption.Symmetric.Provider.RC2, _TargetString2));
         Assert.AreEqual(_TargetString2, SymmetricLoopback(Encryption.Symmetric.Provider.Rijndael, _TargetString2));
         Assert.AreEqual(_TargetString2, SymmetricWithKey(Encryption.Symmetric.Provider.Rijndael, _TargetString2));
         Assert.AreEqual(_TargetString2, SymmetricLoopback(Encryption.Symmetric.Provider.TripleDES, _TargetString2));
         Assert.AreEqual(_TargetString2, SymmetricWithKey(Encryption.Symmetric.Provider.TripleDES, _TargetString2));
      }

      [Test(), Category("Symmetric")]
      public void SymmetricFile()
      {
         //-- compare the hash of the decrypted file to what it should be after encryption/decryption
         //-- using pure file streams
         Assert.AreEqual("AC27F132E6728E4F8FA3B054013D3456", SymmetricFilePrivate(Encryption.Symmetric.Provider.TripleDES, "gettysburg.txt", "Password, Yo!"));
         Assert.AreEqual("4F32AB797F0FCC782AAC0B4F4E5B1693", SymmetricFilePrivate(Encryption.Symmetric.Provider.RC2, "sample.doc", "0nTheDownLow1"));
      }

      private string SymmetricFilePrivate(Encryption.Symmetric.Provider p, string fileName, string key)
      {
         string EncryptedFilePath = Path.GetFileNameWithoutExtension(fileName) + ".encrypted";
         string DecryptedFilePath = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fileName)) + "-decrypted" + Path.GetExtension(fileName);

         //-- encrypt the file to memory
         Encryption.Symmetric sym = new Encryption.Symmetric(p);
         sym.Key = new Data(key);
         Data encryptedData = new Data();

         StreamReader sr = null;
         using (sr = new StreamReader(fileName))
         {
            encryptedData = sym.Encrypt(sr.BaseStream);
         }

         //-- write encrypted data to a new binary file
         StreamWriter sw = new StreamWriter(EncryptedFilePath);
         BinaryWriter bw = new BinaryWriter(sw.BaseStream);
         bw.Write(encryptedData.Bytes);
         bw.Close();

         //-- decrypt this binary file
         Data decryptedData;
         Encryption.Symmetric sym2 = new Encryption.Symmetric(p);
         sym2.Key = new Data(key);
         try
         {
            sr = new StreamReader(EncryptedFilePath);
            decryptedData = sym.Decrypt(sr.BaseStream);
         }
         finally
         {
            if ((sr != null)) sr.Close();
            //File.Delete(EncryptedFilePath);
         }

         //-- write decrypted data to a new binary file
         sw = new StreamWriter(DecryptedFilePath);
         bw = new BinaryWriter(sw.BaseStream);
         bw.Write(decryptedData.Bytes);
         bw.Close();

         //-- get the MD5 hash of the returned data
         Hash h = new Hash(Hash.Provider.MD5);
         return h.Calculate(decryptedData).ToHex();
      }

      /// <summary>
      /// test using user-provided keys and init vectors
      /// </summary>
      private string SymmetricWithKey(Encryption.Symmetric.Provider p, string TargetString)
      {
         Encryption.Symmetric sym = new Encryption.Symmetric(p, false);
         Encryption.Symmetric sym2 = new Encryption.Symmetric(p, false);
         Data encryptedData;
         Data decryptedData;

         Data keyData = new Data("MySecretPassword");
         Data ivData = new Data("MyInitializationVector");

         sym.IntializationVector = ivData;
         encryptedData = sym.Encrypt(new Data(TargetString), keyData);
         string encryptedString = encryptedData.Base64;
         
         Data newEncryptedData = new Data(Utils.FromBase64(encryptedString));
         sym2.IntializationVector = ivData;
         decryptedData = sym2.Decrypt(newEncryptedData, keyData);

         ////-- the data will be padded to the encryption blocksize, so we need to trim it back down.
         //return decryptedData.ToString().Substring(0, _TargetData.Bytes.Length);

         return decryptedData.ToString();
      }

      /// <summary>
      /// test using auto-generated keys
      /// </summary>
      private string SymmetricLoopback(Encryption.Symmetric.Provider p, string TargetString)
      {
         Encryption.Symmetric sym = new Encryption.Symmetric(p);
         Encryption.Symmetric sym2 = new Encryption.Symmetric(p);
         Data encryptedData;
         Data decryptedData;

         encryptedData = sym.Encrypt(new Data(TargetString));
         decryptedData = sym2.Decrypt(encryptedData, sym.Key);

         ////-- the data will be padded to the encryption blocksize, so we need to trim it back down.
         //return decryptedData.ToString().Substring(0, _TargetData.Bytes.Length);

         return decryptedData.ToString();
      } 
      #endregion
   }
}
