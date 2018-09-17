
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HFM.Log
{
   public abstract class FahLogReader : IDisposable
   {
      private readonly TextReader _textReader;
      private readonly ILogLineTypeIdentifier _typeIdentifier;
      private readonly ILogLineParserDictionary _parserDictionary;

      protected FahLogReader(TextReader textReader, ILogLineTypeIdentifier typeIdentifier, ILogLineParserDictionary parserDictionary)
      {
         _textReader = textReader;
         _typeIdentifier = typeIdentifier;
         _parserDictionary = parserDictionary;
      }

      private int _lineIndex;

      /// <summary>
      /// Reads a line of characters from the log and returns the data as a LogLine.
      /// </summary>
      /// <returns>The next line from the reader, or null if all lines have been read.</returns>
      public LogLine ReadLine()
      {
         string line = _textReader.ReadLine();
         return CreateLine(line, _lineIndex++);
      }

      /// <summary>
      /// Reads a line of characters asynchronously and returns the data as a LogLine.
      /// </summary>
      /// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter contains the next line from the reader, or is null if all of the lines have been read.</returns>
      public async Task<LogLine> ReadLineAsync()
      {
         string line = await _textReader.ReadLineAsync().ConfigureAwait(false);
         return CreateLine(line, _lineIndex++);
      }

      private LogLine CreateLine(string line, int index)
      {
         if (line == null) return null;

         var lineType = _typeIdentifier.DetermineLineType(line);
         LogLineParser parser;
         _parserDictionary.TryGetValue(lineType, out parser);

         var logLine = new LogLine { LineType = lineType, Index = index, Raw = line, Parser = parser };
         return logLine;
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
            : this(textReader, new FahClientLogLineTypeIdentifier(), new FahClientLogLineParserDictionary())
         {
            
         }

         public FahClientLogReader(TextReader textReader, ILogLineTypeIdentifier typeIdentifier, ILogLineParserDictionary parserDictionary)
            : base(textReader, typeIdentifier, parserDictionary)
         {

         }
      }
   }

   namespace Legacy
   {
      public class LegacyLogReader : FahLogReader
      {
         public LegacyLogReader(TextReader textReader)
            : this(textReader, new LegacyLogLineTypeIdentifier(), new LegacyLogLineParserDictionary())
         {
            
         }

         public LegacyLogReader(TextReader textReader, ILogLineTypeIdentifier typeIdentifier, ILogLineParserDictionary parserDictionary)
            : base(textReader, typeIdentifier, parserDictionary)
         {

         }
      }
   }
}
