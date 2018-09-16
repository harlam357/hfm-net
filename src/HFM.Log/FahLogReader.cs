
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HFM.Log
{
   public abstract class FahLogReader : IDisposable
   {
      private readonly IFahLogLineReader _lineReader;
      private readonly ILogLineTypeIdentifier _logLineTypeIdentifier;
      private readonly ILogLineParserDictionary _parserDictionary;

      internal FahLogReader(IFahLogLineReader lineReader, ILogLineTypeIdentifier logLineTypeIdentifier, ILogLineParserDictionary parserDictionary)
      {
         _lineReader = lineReader;
         _logLineTypeIdentifier = logLineTypeIdentifier;
         _parserDictionary = parserDictionary;
      }

      private int _lineIndex;

      /// <summary>
      /// Reads a line of characters from the log and returns the data as a LogLine.
      /// </summary>
      /// <returns>The next line from the reader, or null if all lines have been read.</returns>
      public LogLine ReadLine()
      {
         string line = _lineReader.ReadLine();
         return CreateLine(line, _lineIndex++);
      }

      /// <summary>
      /// Reads a line of characters asynchronously and returns the data as a LogLine.
      /// </summary>
      /// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter contains the next line from the reader, or is null if all of the lines have been read.</returns>
      public async Task<LogLine> ReadLineAsync()
      {
         string line = await _lineReader.ReadLineAsync().ConfigureAwait(false);
         return CreateLine(line, _lineIndex++);
      }

      private LogLine CreateLine(string line, int index)
      {
         if (line == null) return null;
         var lineType = _logLineTypeIdentifier.DetermineLineType(line);
         Func<LogLine, object> parser;
         _parserDictionary.TryGetValue(lineType, out parser);
         var logLine = new LogLine { LineRaw = line, LineType = lineType, LineIndex = index };
         if (parser != null) logLine.SetParser(parser);
         return logLine;
      }

      public virtual void Close()
      {
         _lineReader.Close();
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
         private FahClientLogReader(IFahLogLineReader lineReader, ILogLineTypeIdentifier logLineTypeIdentifier, ILogLineParserDictionary parserDictionary)
            : base(lineReader, logLineTypeIdentifier, parserDictionary)
         {

         }

         public static FahLogReader Create(TextReader textReader)
         {
            return new FahClientLogReader(new FahTextReaderLogLineReader(textReader), new FahClientLogLineTypeIdentifier(), new FahClientLogLineParserDictionary());
         }

         public static FahLogReader Create(IEnumerable<string> lines)
         {
            return new FahClientLogReader(new FahStringEnumerationLogLineReader(lines), new FahClientLogLineTypeIdentifier(), new FahClientLogLineParserDictionary());
         }
      }
   }

   namespace Legacy
   {
      public class LegacyLogReader : FahLogReader
      {
         private LegacyLogReader(IFahLogLineReader lineReader, ILogLineTypeIdentifier logLineTypeIdentifier, ILogLineParserDictionary parserDictionary)
            : base(lineReader, logLineTypeIdentifier, parserDictionary)
         {

         }

         public static FahLogReader Create(TextReader textReader)
         {
            return new LegacyLogReader(new FahTextReaderLogLineReader(textReader), new LegacyLogLineTypeIdentifier(), new LegacyLogLineParserDictionary());
         }

         public static FahLogReader Create(IEnumerable<string> lines)
         {
            return new LegacyLogReader(new FahStringEnumerationLogLineReader(lines), new LegacyLogLineTypeIdentifier(), new LegacyLogLineParserDictionary());
         }
      }
   }

   internal interface IFahLogLineReader
   {
      string ReadLine();

      Task<string> ReadLineAsync();

      void Close();
   }

   internal sealed class FahTextReaderLogLineReader : IFahLogLineReader
   {
      private readonly TextReader _textReader;

      public FahTextReaderLogLineReader(TextReader textReader)
      {
         _textReader = textReader;
      }

      public string ReadLine()
      {
         return _textReader.ReadLine();
      }

      public async Task<string> ReadLineAsync()
      {
         return await _textReader.ReadLineAsync().ConfigureAwait(false);
      }

      public void Close()
      {
         _textReader.Close();
      }
   }

   internal sealed class FahStringEnumerationLogLineReader : IFahLogLineReader
   {
      private readonly IEnumerator<string> _lineEnumerator;

      public FahStringEnumerationLogLineReader(IEnumerable<string> lines)
      {
         _lineEnumerator = lines.GetEnumerator();
      }

      public string ReadLine()
      {
         if (_lineEnumerator.MoveNext())
         {
            return _lineEnumerator.Current;
         }
         return null;
      }

      public Task<string> ReadLineAsync()
      {
         return Task.FromResult(ReadLine());
      }

      public void Close()
      {
         
      }
   }
}
