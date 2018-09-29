
using System;
using System.IO;
using System.Threading.Tasks;

namespace HFM.Log
{
   public abstract class FahLogReader : IDisposable
   {
      private readonly TextReader _textReader;

      protected FahLogReader(TextReader textReader)
      {
         _textReader = textReader ?? throw new ArgumentNullException(nameof(textReader));
      }

      private int _lineIndex;

      /// <summary>
      /// Reads a line of characters from the log and returns the data as a LogLine.
      /// </summary>
      /// <returns>The next line from the reader, or null if all lines have been read.</returns>
      public LogLine ReadLine()
      {
         string line = _textReader.ReadLine();
         return CreateLogLine(line, _lineIndex++);
      }

      /// <summary>
      /// Reads a line of characters asynchronously and returns the data as a LogLine.
      /// </summary>
      /// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter contains the next line from the reader, or is null if all of the lines have been read.</returns>
      public async Task<LogLine> ReadLineAsync()
      {
         string line = await _textReader.ReadLineAsync().ConfigureAwait(false);
         return CreateLogLine(line, _lineIndex++);
      }

      private LogLine CreateLogLine(string line, int index)
      {
         if (line == null) return null;
         return OnCreateLogLine(line, index);
      }

      /// <summary>
      /// Implement this method in a derived type and return a <see cref="LogLine"/> value based on the contents of the string line and line index.
      /// </summary>
      protected abstract LogLine OnCreateLogLine(string line, int index);

      public virtual void Close()
      {
         _textReader.Close();
      }

      protected virtual void Dispose(bool disposing)
      {
         if (disposing)
         {
            Close();
         }
      }

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }
   }

   namespace FahClient
   {
      public class FahClientLogReader : FahLogReader
      {
         protected ILogLineTypeResolver LogLineTypeResolver { get; }

         protected ILogLineTimeStampParser LogLineTimeStampParser { get; }

         protected ILogLineDataParserDictionary LogLineDataParserDictionary { get; }

         public FahClientLogReader(TextReader textReader)
            : this(textReader, FahClientLogLineTypeResolver.Instance, FahClientLogLineTimeStampParser.Instance, FahClientLogLineDataParserDictionary.Instance)
         {

         }

         protected FahClientLogReader(TextReader textReader, 
                                      ILogLineTypeResolver logLineTypeResolver, 
                                      ILogLineTimeStampParser logLineTimeStampParser, 
                                      ILogLineDataParserDictionary logLineDataParserDictionary)
            : base(textReader)
         {
            LogLineTypeResolver = logLineTypeResolver;
            LogLineTimeStampParser = logLineTimeStampParser;
            LogLineDataParserDictionary = logLineDataParserDictionary;
         }

         protected override LogLine OnCreateLogLine(string line, int index)
         {
            LogLineType lineType = LogLineTypeResolver.Resolve(line);
            LogLineTimeStampParserDelegate timeStampParser = LogLineTimeStampParser.ParseTimeStamp;
            LogLineDataParserDictionary.TryGetValue(lineType, out LogLineDataParserDelegate dataParser);
            return LogLine.Create(line, index, lineType, timeStampParser, dataParser);
         }
      }
   }

   namespace Legacy
   {
      public class LegacyLogReader : FahLogReader
      {
         protected ILogLineTypeResolver LogLineTypeResolver { get; }

         protected ILogLineTimeStampParser LogLineTimeStampParser { get; }

         protected ILogLineDataParserDictionary LogLineDataParserDictionary { get; }

         public LegacyLogReader(TextReader textReader)
            : this(textReader, LegacyLogLineTypeResolver.Instance, LegacyLogLineTimeStampParser.Instance, LegacyLogLineDataParserDictionary.Instance)
         {
            
         }

         protected LegacyLogReader(TextReader textReader, 
                                   ILogLineTypeResolver logLineTypeResolver, 
                                   ILogLineTimeStampParser logLineTimeStampParser,
                                   ILogLineDataParserDictionary logLineDataParserDictionary)
            : base(textReader)
         {
            LogLineTypeResolver = logLineTypeResolver;
            LogLineTimeStampParser = logLineTimeStampParser;
            LogLineDataParserDictionary = logLineDataParserDictionary;
         }

         protected override LogLine OnCreateLogLine(string line, int index)
         {
            LogLineType lineType = LogLineTypeResolver.Resolve(line);
            LogLineTimeStampParserDelegate timeStampParser = LogLineTimeStampParser.ParseTimeStamp;
            LogLineDataParserDictionary.TryGetValue(lineType, out LogLineDataParserDelegate dataParser);
            return LogLine.Create(line, index, lineType, timeStampParser, dataParser);
         }
      }
   }
}
