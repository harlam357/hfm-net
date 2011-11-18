
using Castle.Core.Logging;

namespace HFM.Core.Logging
{
   public class LoggerFactory : AbstractLoggerFactory
   {
      public override ILogger Create(string name)
      {
         return new Logger(name);
      }

      public override ILogger Create(string name, LoggerLevel level)
      {
         return new Logger(name, level);
      }
   }
}
