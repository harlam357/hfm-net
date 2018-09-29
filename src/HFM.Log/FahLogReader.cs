
using System;
using System.IO;
using System.Threading.Tasks;

namespace HFM.Log
{
   /// <summary>
   /// Represents a reader that provides fast, forward-only access to Folding@Home log line data.
   /// </summary>
   public abstract class FahLogReader : IDisposable
   {
      protected FahLogReader()
      {

      }

      /// <summary>
      /// Reads a line of characters from the log and returns the data as a LogLine.
      /// </summary>
      public abstract LogLine ReadLine();

      /// <summary>
      /// Reads a line of characters asynchronously and returns the data as a LogLine.
      /// </summary>
      public abstract Task<LogLine> ReadLineAsync();

      public virtual void Close()
      {
         
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

   /// <summary>
   /// Represents a reader that provides fast, forward-only access to Folding@Home log line text data.
   /// </summary>
   public abstract class FahLogTextReader : FahLogReader
   {
      private readonly TextReader _textReader;

      protected FahLogTextReader(TextReader textReader)
      {
         _textReader = textReader ?? throw new ArgumentNullException(nameof(textReader));
      }

      private int _lineIndex;

      /// <summary>
      /// Reads a line of characters from the log and returns the data as a LogLine.
      /// </summary>
      /// <returns>The next line from the reader, or null if all lines have been read.</returns>
      public override LogLine ReadLine()
      {
         string line = _textReader.ReadLine();
         return CreateLogLine(line, _lineIndex++);
      }

      /// <summary>
      /// Reads a line of characters asynchronously and returns the data as a LogLine.
      /// </summary>
      /// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter contains the next line from the reader, or is null if all of the lines have been read.</returns>
      public override async Task<LogLine> ReadLineAsync()
      {
         string line = await _textReader.ReadLineAsync().ConfigureAwait(false);
         return CreateLogLine(line, _lineIndex++);
      }

      private LogLine CreateLogLine(string line, int index)
      {
         if (line == null) return null;
         return OnCreateLogLine(line, index);
      }

      protected abstract LogLine OnCreateLogLine(string line, int index);

      public override void Close()
      {
         _textReader.Close();
      }
   }

   namespace FahClient
   {
      public class FahClientLogTextReader : FahLogTextReader
      {
         protected ILogLineTypeResolver LogLineTypeResolver { get; }

         protected ILogLineTimeStampParser LogLineTimeStampParser { get; }

         protected ILogLineDataParserCollection LogLineDataParserCollection { get; }

         public FahClientLogTextReader(TextReader textReader)
            : this(textReader, FahClientLogLineTypeResolver.Instance, FahClientLogLineTimeStampParser.Instance, FahClientLogLineDataParserDictionary.Instance)
         {

         }

         protected FahClientLogTextReader(TextReader textReader, 
                                      ILogLineTypeResolver logLineTypeResolver, 
                                      ILogLineTimeStampParser logLineTimeStampParser, 
                                      ILogLineDataParserCollection logLineDataParserCollection)
            : base(textReader)
         {
            LogLineTypeResolver = logLineTypeResolver;
            LogLineTimeStampParser = logLineTimeStampParser;
            LogLineDataParserCollection = logLineDataParserCollection;
         }

         protected override LogLine OnCreateLogLine(string line, int index)
         {
            LogLineType lineType = LogLineTypeResolver.Resolve(line);
            LogLineTimeStampParserFunction timeStampParser = LogLineTimeStampParser.ParseTimeStamp;
            LogLineDataParserCollection.TryGetValue(lineType, out LogLineDataParserFunction dataParser);
            return LogLine.Create(line, index, lineType, timeStampParser, dataParser);
         }
      }
   }

   namespace Legacy
   {
      public class LegacyLogTextReader : FahLogTextReader
      {
         protected ILogLineTypeResolver LogLineTypeResolver { get; }

         protected ILogLineTimeStampParser LogLineTimeStampParser { get; }

         protected ILogLineDataParserCollection LogLineDataParserCollection { get; }

         public LegacyLogTextReader(TextReader textReader)
            : this(textReader, LegacyLogLineTypeResolver.Instance, LegacyLogLineTimeStampParser.Instance, LegacyLogLineDataParserDictionary.Instance)
         {
            
         }

         protected LegacyLogTextReader(TextReader textReader, 
                                   ILogLineTypeResolver logLineTypeResolver, 
                                   ILogLineTimeStampParser logLineTimeStampParser,
                                   ILogLineDataParserCollection logLineDataParserCollection)
            : base(textReader)
         {
            LogLineTypeResolver = logLineTypeResolver;
            LogLineTimeStampParser = logLineTimeStampParser;
            LogLineDataParserCollection = logLineDataParserCollection;
         }

         protected override LogLine OnCreateLogLine(string line, int index)
         {
            LogLineType lineType = LogLineTypeResolver.Resolve(line);
            LogLineTimeStampParserFunction timeStampParser = LogLineTimeStampParser.ParseTimeStamp;
            LogLineDataParserCollection.TryGetValue(lineType, out LogLineDataParserFunction dataParser);
            return LogLine.Create(line, index, lineType, timeStampParser, dataParser);
         }
      }
   }
}
