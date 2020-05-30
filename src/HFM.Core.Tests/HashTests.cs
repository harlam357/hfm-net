
using System;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace HFM.Core
{
    [TestFixture]
    public class HashTests
    {
        private string _targetString;
        //private string _targetString2;

        [SetUp]
        public void Setup()
        {
            _targetString = "The instinct of nearly all societies is to lock up anybody who is truly free. " +
               "First, society begins by trying to beat you up. If this fails, they try to poison you. " +
               "If this fails too, they finish by loading honors on your head." +
               " - Jean Cocteau (1889-1963)";

            //_targetString2 = "Everything should be made as simple as possible, but not simpler. - Albert Einstein";
        }

        [Test, Category("Hash")]
        public void Hash_File_Test()
        {
            string hashHex;

            using (var h2 = new Hash(HashProvider.MD5))
            using (var stream = File.OpenRead(Path.Combine(Environment.CurrentDirectory, @"..\..\TestFiles\sample.doc")))
            {
                hashHex = h2.Calculate(stream).ToHex();
            }
            Assert.AreEqual(hashHex, "4F32AB797F0FCC782AAC0B4F4E5B1693");
        }

        [Test, Category("Hash")]
        public void Hash_Test()
        {
            Assert.AreEqual("44D36517B0CCE797FF57118ABE264FD9", DoHash(HashProvider.MD5));
            Assert.AreEqual("9E93AB42BCC8F738C7FBB6CCA27A902DC663DBE1", DoHash(HashProvider.SHA1));
            Assert.AreEqual("40AF07ABFE970590B2C313619983651B1E7B2F8C2D855C6FD4266DAFD7A5E670", DoHash(HashProvider.SHA256));
            Assert.AreEqual("9FC0AFB3DA61201937C95B133AB397FE62C329D6061A8768DA2B9D09923F07624869D01CD76826E1152DAB7BFAA30915", DoHash(HashProvider.SHA384));
            Assert.AreEqual("2E7D4B051DD528F3E9339E0927930007426F4968B5A4A08349472784272F17DA5C532EDCFFE14934988503F77DEF4AB58EB05394838C825632D04A10F42A753B", DoHash(HashProvider.SHA512));
        }

        private string DoHash(HashProvider p)
        {
            using (var h = new Hash(p))
            using (var ms = new MemoryStream())
            {
                var bytes = Encoding.Default.GetBytes(_targetString);
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                return h.Calculate(ms).ToHex();
            }
        }

        [Test, Category("Hash")]
        public void Hash_File_WithProgress_Test()
        {
            string hashHex;

            var progressRaised = false;
            var progress = new CurrentThreadProgress<int>(value => progressRaised = true);
            using (var h2 = new Hash(HashProvider.MD5))
            using (var stream = File.OpenRead(Path.Combine(Environment.CurrentDirectory, @"..\..\TestFiles\sample.doc")))
            {
                hashHex = h2.Calculate(stream, progress).ToHex();
            }
            Assert.AreEqual(hashHex, "4F32AB797F0FCC782AAC0B4F4E5B1693");
            Assert.IsTrue(progressRaised);
        }

        private class CurrentThreadProgress<T> : IProgress<T>
        {
            private readonly Action<T> _action;

            public CurrentThreadProgress(Action<T> action)
            {
                _action = action;
            }

            public void Report(T value)
            {
                _action(value);
            }
        }
    }

}
