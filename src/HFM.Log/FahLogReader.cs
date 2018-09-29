
using System;
using System.IO;
using System.Threading.Tasks;

namespace HFM.Log
{
   public abstract class FahLogReader : IDisposable
   {
      private readonly TextReader _textReader;

      protected ILogLineTypeResolver LineTypeResolver { get; }

      protected ILogLineDataParserDictionary LogLineDataParserDictionary { get; }

      protected FahLogReader(TextReader textReader, ILogLineTypeResolver logLineTypeResolver, ILogLineDataParserDictionary logLineDataParserDictionary)
      {
         _textReader = textReader ?? throw new ArgumentNullException(nameof(textReader));
         LineTypeResolver = logLineTypeResolver ?? throw new ArgumentNullException(nameof(logLineTypeResolver));
         LogLineDataParserDictionary = logLineDataParserDictionary ?? throw new ArgumentNullException(nameof(logLineDataParserDictionary));
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

      protected virtual LogLine OnCreateLogLine(string line, int index)
      {
         LogLineType lineType = LineTypeResolver.Resolve(line);
         LogLineTimeStampParserDelegate timeStampParser = LogLineTimeStampParser.Default;
         LogLineDataParserDictionary.TryGetValue(lineType, out LogLineDataParserDelegate dataParser);
         return LogLine.Create(line, index, lineType, timeStampParser, dataParser);
      }

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
         public FahClientLogReader(TextReader textReader)
            : this(textReader, FahClientLogLineTypeResolver.Instance, FahClientLogLineDataParserDictionary.Instance)
         {
            
         }

         public FahClientLogReader(TextReader textReader, ILogLineTypeResolver logLineTypeResolver, ILogLineDataParserDictionary logLineDataParserDictionary)
            : base(textReader, logLineTypeResolver, logLineDataParserDictionary)
         {

         }
      }
   }

   namespace Legacy
   {
      public class LegacyLogReader : FahLogReader
      {
         public LegacyLogReader(TextReader textReader)
            : this(textReader, LegacyLogLineTypeResolver.Instance, LegacyLogLineDataParserDictionary.Instance)
         {
            
         }

         public LegacyLogReader(TextReader textReader, ILogLineTypeResolver logLineTypeResolver, ILogLineDataParserDictionary logLineDataParserDictionary)
            : base(textReader, logLineTypeResolver, logLineDataParserDictionary)
         {

         }
      }
   }
}
