using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HFM.Core.Client;
using HFM.Core.Client.Mocks;
using HFM.Log;

using NUnit.Framework;

namespace HFM.Core
{
    [TestFixture]
    public class SlotModelTests
    {
        [Test]
        public void SlotModel_CurrentLogLines_IsThreadSafe()
        {
            var slot = new SlotModel(new MockClient());
            var random = new Random();

            Task.Run(() =>
            {
                while (true)
                {
                    slot.CurrentLogLines = Enumerable.Repeat(new LogLine(), random.Next(1, 5)).ToList();
                }
            });

            const int count = 100;

            var tasks = Enumerable.Range(0, count)
                .Select(_ => Task.Run(() =>
                {
                    Thread.Sleep(10);
                    var x = slot.CurrentLogLines.ToList();
                }))
                .ToArray();

            try
            {
                Task.WaitAll(tasks);
            }
            catch (Exception)
            {
                Assert.Fail("Enumeration failed");
            }
        }
    }
}
