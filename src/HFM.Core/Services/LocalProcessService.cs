
using System;
using System.Diagnostics;

namespace HFM.Core.Services
{
    public abstract class LocalProcessService
    {
        public static LocalProcessService Default { get; } = DefaultLocalProcessService.Instance;

        // ReSharper disable once EmptyConstructor
        protected LocalProcessService()
        {

        }

        public abstract LocalProcess Start(string fileName);

        public abstract LocalProcess Start(string fileName, string arguments);
    }

    public class DefaultLocalProcessService : LocalProcessService
    {
        public static DefaultLocalProcessService Instance { get; } = new DefaultLocalProcessService();

        protected DefaultLocalProcessService()
        {

        }

        public override LocalProcess Start(string fileName)
        {
            return DefaultLocalProcess.Start(fileName);
        }

        public override LocalProcess Start(string fileName, string arguments)
        {
            return DefaultLocalProcess.Start(fileName, arguments);
        }
    }

    public class NullLocalProcessService : LocalProcessService
    {
        public static NullLocalProcessService Instance { get; } = new NullLocalProcessService();

        protected NullLocalProcessService()
        {

        }

        public override LocalProcess Start(string fileName)
        {
            return NullLocalProcess.Instance;
        }

        public override LocalProcess Start(string fileName, string arguments)
        {
            return NullLocalProcess.Instance;
        }
    }

    public abstract class LocalProcess
    {
        public abstract void WaitForExit();

        public abstract bool WaitForExit(int timeout);

        public abstract int ExitCode { get; }
    }

    public class DefaultLocalProcess : LocalProcess
    {
        private readonly Process _process;

        protected DefaultLocalProcess(Process process)
        {
            _process = process ?? throw new ArgumentNullException(nameof(process));
        }

        public override void WaitForExit()
        {
            _process.WaitForExit();
        }

        public override bool WaitForExit(int timeout)
        {
            return _process.WaitForExit(timeout);
        }

        public override int ExitCode => _process.ExitCode;

        public static DefaultLocalProcess Start(string fileName)
        {
            return new DefaultLocalProcess(Process.Start(fileName));
        }

        public static DefaultLocalProcess Start(string fileName, string arguments)
        {
            return new DefaultLocalProcess(Process.Start(fileName, arguments));
        }
    }

    public class NullLocalProcess : LocalProcess
    {
        public static NullLocalProcess Instance { get; } = new NullLocalProcess();

        protected NullLocalProcess()
        {

        }

        public override void WaitForExit()
        {

        }

        public override bool WaitForExit(int timeout)
        {
            return true;
        }

        public override int ExitCode => 0;
    }
}
