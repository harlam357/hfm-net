namespace HFM.Core.Logging
{
    public static class TestLogger
    {
        public static ILogger Instance { get; } = ConsoleLogger.Instance;
    }
}
