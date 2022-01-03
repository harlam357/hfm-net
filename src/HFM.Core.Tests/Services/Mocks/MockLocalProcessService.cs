using System.Collections.Generic;

namespace HFM.Core.Services.Mocks
{
    public class MockLocalProcessService : LocalProcessService
    {
        public ICollection<MockLocalProcess> Invocations { get; } = new List<MockLocalProcess>();

        public override LocalProcess Start(string fileName)
        {
            var process = MockLocalProcess.Start(fileName);
            Invocations.Add(process);
            return process;
        }

        public override LocalProcess Start(string fileName, string arguments)
        {
            var process = MockLocalProcess.Start(fileName, arguments);
            Invocations.Add(process);
            return process;
        }
    }

    public class MockLocalProcess : NullLocalProcess
    {
        public string FileName { get; }
        public string Arguments { get; }

        public MockLocalProcess(string fileName)
        {
            FileName = fileName;
        }

        public MockLocalProcess(string fileName, string arguments)
        {
            FileName = fileName;
            Arguments = arguments;
        }

        public static MockLocalProcess Start(string fileName)
        {
            return new MockLocalProcess(fileName);
        }

        public static MockLocalProcess Start(string fileName, string arguments)
        {
            return new MockLocalProcess(fileName, arguments);
        }
    }
}
