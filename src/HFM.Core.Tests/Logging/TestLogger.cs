namespace HFM.Core.Logging
{
    public static class TestLogger
    {
#if DEBUG
        public static ILogger Instance { get; } = ConsoleLogger.Instance;
#else
        public static ILogger Instance { get; } = NullLogger.Instance;
#endif
    }
}
