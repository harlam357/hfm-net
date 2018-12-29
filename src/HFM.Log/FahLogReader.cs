
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
      /// <summary>
      /// Initializes a new instance of the <see cref="FahLogReader"/> class.
      /// </summary>
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

      /// <summary>
      /// Closes the <see cref="FahLogReader"/> and releases any system resources associated with the it.
      /// </summary>
      public virtual void Close()
      {

      }

      /// <summary>
      /// Releases the unmanaged resources used by the <see cref="FahLogReader"/> and optionally releases the managed resources.
      /// </summary>
      /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
      protected virtual void Dispose(bool disposing)
      {
         if (disposing)
         {
            Close();
         }
      }

      /// <summary>
      /// Releases all resources used by the <see cref="FahLogReader"/> object.
      /// </summary>
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
      /// <summary>
      /// Initializes a new instance of the <see cref="FahLogTextReader"/> class.
      /// </summary>
      /// <param name="textReader">The <see cref="TextReader"/> that provides line data as a string.</param>
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
         if (line == null) return null;
         return OnReadLine(line, _lineIndex++);
      }

      /// <summary>
      /// Reads a line of characters asynchronously and returns the data as a LogLine.
      /// </summary>
      /// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter contains the next line from the reader, or is null if all of the lines have been read.</returns>
      public override async Task<LogLine> ReadLineAsync()
      {
         string line = await _textReader.ReadLineAsync().ConfigureAwait(false);
         if (line == null) return null;
         return OnReadLine(line, _lineIndex++);
      }

      /// <summary>
      /// Occurs after a line was read from the <see cref="TextReader"/> and returns a new <see cref="LogLine"/> object.
      /// </summary>
      /// <param name="line">The line read from the <see cref="TextReader"/>.</param>
      /// <param name="index">The index of the line read from the <see cref="TextReader"/>.</param>
      /// <returns>A new <see cref="LogLine"/> object from the string line and line index.</returns>
      protected abstract LogLine OnReadLine(string line, int index);

      /// <summary>
      /// Closes the <see cref="FahLogTextReader"/> and releases any system resources associated with the it.
      /// </summary>
      public override void Close()
      {
         _textReader.Close();
      }
   }

   namespace FahClient
   {
      /// <summary>
      /// Represents a reader that reads Folding@Home log line text data from a v7 or newer client.
      /// </summary>
      public class FahClientLogTextReader : FahLogTextReader
      {
         /// <summary>
         /// Gets the <see cref="LogLineTypeResolver"/> used to resolve the <see cref="LogLineType"/> from a log line.
         /// </summary>
         protected LogLineTypeResolver LogLineTypeResolver { get; }

         /// <summary>
         /// Gets the <see cref="LogLineDataParserDictionary"/> that provides parsing functions for each <see cref="LogLineType"/>.
         /// </summary>
         protected LogLineDataParserDictionary LogLineDataParserDictionary { get; }

         /// <summary>
         /// Gets the <see cref="LogLineTimeStampParser"/> used to parse time stamp information from a log line.
         /// </summary>
         protected LogLineTimeStampParser LogLineTimeStampParser { get; }

         /// <summary>
         /// Initializes a new instance of the <see cref="FahClientLogTextReader"/> class.
         /// </summary>
         /// <param name="textReader">The <see cref="TextReader"/> that provides line data as a string.</param>
         public FahClientLogTextReader(TextReader textReader)
            : this(textReader, null, null, null)
         {

         }

         /// <summary>
         /// Initializes a new instance of the <see cref="FahClientLogTextReader"/> class.
         /// </summary>
         /// <param name="textReader">The <see cref="TextReader"/> that provides line data as a string.</param>
         /// <param name="logLineTypeResolver">The <see cref="LogLineTypeResolver"/> used to resolve the <see cref="LogLineType"/> from a log line.</param>
         /// <param name="logLineDataParserDictionary">The <see cref="LogLineDataParserDictionary"/> that provides parsing functions for each <see cref="LogLineType"/>.</param>
         /// <param name="logLineTimeStampParser">The <see cref="LogLineTimeStampParser"/> used to parse time stamp information from a log line.</param>
         protected FahClientLogTextReader(TextReader textReader,
                                          LogLineTypeResolver logLineTypeResolver,
                                          LogLineDataParserDictionary logLineDataParserDictionary,
                                          LogLineTimeStampParser logLineTimeStampParser)
            : base(textReader)
         {
            LogLineTypeResolver = logLineTypeResolver ?? FahClientLogLineTypeResolver.Instance;
            LogLineDataParserDictionary = logLineDataParserDictionary ?? FahClientLogLineDataParserDictionary.Instance;
            LogLineTimeStampParser = logLineTimeStampParser ?? LogLineTimeStampParser.Instance;
         }

         /// <summary>
         /// Occurs after a line was read from the <see cref="TextReader"/> and returns a new <see cref="LogLine"/> object.
         /// </summary>
         /// <param name="line">The line read from the <see cref="TextReader"/>.</param>
         /// <param name="index">The index of the line read from the <see cref="TextReader"/>.</param>
         /// <returns>A new <see cref="LogLine"/> object from the string line and line index.</returns>
         protected override LogLine OnReadLine(string line, int index)
         {
            LogLineType lineType = LogLineTypeResolver.Resolve(line);
            LogLineTimeStampParserFunction timeStampParser = LogLineTimeStampParser.ParseTimeStamp;
            LogLineDataParserDictionary.TryGetValue(lineType, out LogLineDataParserFunction dataParser);
            return new LazyLogLine(line, index, lineType, timeStampParser, dataParser);
         }
      }
   }

   namespace Legacy
   {
      /// <summary>
      /// Represents a reader that reads Folding@Home log line text data from a v6 or prior client.
      /// </summary>
      public class LegacyLogTextReader : FahLogTextReader
      {
         /// <summary>
         /// Gets the <see cref="LogLineTypeResolver"/> used to resolve the <see cref="LogLineType"/> from a log line.
         /// </summary>
         protected LogLineTypeResolver LogLineTypeResolver { get; }

         /// <summary>
         /// Gets the <see cref="LogLineDataParserDictionary"/> that provides parsing functions for each <see cref="LogLineType"/>.
         /// </summary>
         protected LogLineDataParserDictionary LogLineDataParserDictionary { get; }

         /// <summary>
         /// Gets the <see cref="LogLineTimeStampParser"/> used to parse time stamp information from a log line.
         /// </summary>
         protected LogLineTimeStampParser LogLineTimeStampParser { get; }

         /// <summary>
         /// Initializes a new instance of the <see cref="LegacyLogTextReader"/> class.
         /// </summary>
         /// <param name="textReader">The <see cref="TextReader"/> that provides line data as a string.</param>
         public LegacyLogTextReader(TextReader textReader)
            : this(textReader, null, null, null)
         {

         }

         /// <summary>
         /// Initializes a new instance of the <see cref="LegacyLogTextReader"/> class.
         /// </summary>
         /// <param name="textReader">The <see cref="TextReader"/> that provides line data as a string.</param>
         /// <param name="logLineTypeResolver">The <see cref="LogLineTypeResolver"/> used to resolve the <see cref="LogLineType"/> from a log line.</param>
         /// <param name="logLineDataParserDictionary">The <see cref="LogLineDataParserDictionary"/> that provides parsing functions for each <see cref="LogLineType"/>.</param>
         /// <param name="logLineTimeStampParser">The <see cref="LogLineTimeStampParser"/> used to parse time stamp information from a log line.</param>
         protected LegacyLogTextReader(TextReader textReader,
                                       LogLineTypeResolver logLineTypeResolver,
                                       LogLineDataParserDictionary logLineDataParserDictionary,
                                       LogLineTimeStampParser logLineTimeStampParser)
            : base(textReader)
         {
            LogLineTypeResolver = logLineTypeResolver ?? LegacyLogLineTypeResolver.Instance;
            LogLineDataParserDictionary = logLineDataParserDictionary ?? LegacyLogLineDataParserDictionary.Instance;
            LogLineTimeStampParser = logLineTimeStampParser ?? LogLineTimeStampParser.Instance;
         }

         /// <summary>
         /// Occurs after a line was read from the <see cref="TextReader"/> and returns a new <see cref="LogLine"/> object.
         /// </summary>
         /// <param name="line">The line read from the <see cref="TextReader"/>.</param>
         /// <param name="index">The index of the line read from the <see cref="TextReader"/>.</param>
         /// <returns>A new <see cref="LogLine"/> object from the string line and line index.</returns>
         protected override LogLine OnReadLine(string line, int index)
         {
            LogLineType lineType = LogLineTypeResolver.Resolve(line);
            LogLineTimeStampParserFunction timeStampParser = LogLineTimeStampParser.ParseTimeStamp;
            LogLineDataParserDictionary.TryGetValue(lineType, out LogLineDataParserFunction dataParser);
            return new LazyLogLine(line, index, lineType, timeStampParser, dataParser);
         }
      }
   }
}
