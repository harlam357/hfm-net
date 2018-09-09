
using System.Collections.Generic;
using System.Diagnostics;

using NUnit.Framework;

using HFM.Preferences.Data;

namespace HFM.Preferences.Internal
{
   [TestFixture]
   public class ObjectExtensionsTests
   {
      [Test]
      public void ObjectExtensions_Copy_Test()
      {
         short a = 1;
         int b = 2;
         long c = 3;
         float d = 4.0f;
         double e = 5.0;
         string f = "foo";
         var g = new ClientRetrievalTask { Enabled = false };
         var h = new List<string>(new[] { "foo" });
         var i = new Dictionary<string, object> { { "foo", new object() } };

         object objA = (short)1;
         object objB = 2;
         object objC = (long)3;
         object objD = 4.0f;
         object objE = 5.0;
         object objF = "foo";
         object objG = new ClientRetrievalTask { Enabled = false };
         object objH = new List<string>(new[] { "foo" });
         object objI = new Dictionary<string, object> { { "foo", new object() } };

         Assert.AreEqual(1, a.Copy());
         Assert.AreEqual(2, b.Copy());
         Assert.AreEqual(3, c.Copy());
         Assert.AreEqual(4.0f, d.Copy());
         Assert.AreEqual(5.0, e.Copy());
         Assert.AreEqual("foo", f.Copy());
         Assert.AreNotSame(g, g.Copy());
         Assert.AreNotSame(h, h.Copy());
         Assert.AreNotSame(i, i.Copy());

         Assert.AreEqual(1, objA.Copy());
         Assert.AreEqual(2, objB.Copy());
         Assert.AreEqual(3, objC.Copy());
         Assert.AreEqual(4.0f, objD.Copy());
         Assert.AreEqual(5.0, objE.Copy());
         Assert.AreEqual("foo", objF.Copy());
         Assert.AreNotSame(g, objG.Copy());
         Assert.AreNotSame(h, objH.Copy());
         Assert.AreNotSame(i, objI.Copy());
      }

      [Test]
      public void ObjectExtensions_Copy_Benchmark()
      {
         object objNum = null;

         var sw = Stopwatch.StartNew();
         for (int i = 0; i < 1000000; i++)
         {
            object objNumCopy = objNum.Copy(typeof(int));
         }
         sw.Stop();
         Debug.WriteLine("Copy Null ValueType: {0}ms", sw.ElapsedMilliseconds);

         int num = 1;

         sw = Stopwatch.StartNew();
         for (int i = 0; i < 1000000; i++)
         {
            int numCopy = num.Copy();
         }
         sw.Stop();
         Debug.WriteLine("Copy ValueType: {0}ms", sw.ElapsedMilliseconds);

         string str = null;

         sw = Stopwatch.StartNew();
         for (int i = 0; i < 1000000; i++)
         {
            string strCopy = str.Copy();
         }
         sw.Stop();
         Debug.WriteLine("Copy Null String: {0}ms", sw.ElapsedMilliseconds);

         str = "foo";

         sw = Stopwatch.StartNew();
         for (int i = 0; i < 1000000; i++)
         {
            string strCopy = str.Copy();
         }
         sw.Stop();
         Debug.WriteLine("Copy String: {0}ms", sw.ElapsedMilliseconds);

         ClientRetrievalTask task = null;

         sw = Stopwatch.StartNew();
         for (int i = 0; i < 1000000; i++)
         {
            ClientRetrievalTask taskCopy = task.Copy();
         }
         sw.Stop();
         Debug.WriteLine("Copy Null Class: {0}ms", sw.ElapsedMilliseconds);

         task = new ClientRetrievalTask();

         sw = Stopwatch.StartNew();
         for (int i = 0; i < 1000000; i++)
         {
            ClientRetrievalTask taskCopy = task.Copy();
         }
         sw.Stop();
         Debug.WriteLine("Copy Class: {0}ms", sw.ElapsedMilliseconds);

         List<string> list = null;

         sw = Stopwatch.StartNew();
         for (int i = 0; i < 1000000; i++)
         {
            List<string> listCopy = list.Copy();
         }
         sw.Stop();
         Debug.WriteLine("Copy Null List: {0}ms", sw.ElapsedMilliseconds);

         list = new List<string>(new[] { "a", "b", "c" });

         sw = Stopwatch.StartNew();
         for (int i = 0; i < 1000000; i++)
         {
            List<string> listCopy = list.Copy();
         }
         sw.Stop();
         Debug.WriteLine("Copy List: {0}ms", sw.ElapsedMilliseconds);
      }
   }
}
