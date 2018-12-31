
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HFM.Log
{
   /// <summary>
   /// Enumerates the <see cref="LogLine"/> objects bound to the source and child objects in the original log order.
   /// </summary>
   public abstract class LogLineEnumerable : IEnumerable<LogLine>
   {
      /// <summary>
      /// Returns an enumerator that iterates through the collection of <see cref="LogLine"/> objects bound to the source, and child, objects in the original log order..
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the collection.</returns>
      public abstract IEnumerator<LogLine> GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      /// <summary>
      /// Creates and returns a <see cref="LogLineEnumerable"/> from an <see cref="FahLog"/> source.
      /// </summary>
      /// <param name="source">The enumeration source.</param>
      /// <returns>A <see cref="LogLineEnumerable"/> from an <see cref="FahLog"/> source.</returns>
      public static LogLineEnumerable Create(FahLog source)
      {
         return new FahLogLogLineEnumerable(source);
      }

      private class FahLogLogLineEnumerable : LogLineEnumerable
      {
         private readonly FahLog _source;

         public FahLogLogLineEnumerable(FahLog source)
         {
            _source = source ?? throw new ArgumentNullException(nameof(source));
         }

         public override IEnumerator<LogLine> GetEnumerator()
         {
            return _source.ClientRuns.SelectMany(Create).GetEnumerator();
         }
      }

      /// <summary>
      /// Creates and returns a <see cref="LogLineEnumerable"/> from an <see cref="ClientRun"/> source.
      /// </summary>
      /// <param name="source">The enumeration source.</param>
      /// <returns>A <see cref="LogLineEnumerable"/> from an <see cref="ClientRun"/> source.</returns>
      public static LogLineEnumerable Create(ClientRun source)
      {
         return new ClientRunLogLineEnumerable(source);
      }

      private class ClientRunLogLineEnumerable : LogLineEnumerable
      {
         private readonly ClientRun _source;

         public ClientRunLogLineEnumerable(ClientRun source)
         {
            _source = source ?? throw new ArgumentNullException(nameof(source));
         }

         public override IEnumerator<LogLine> GetEnumerator()
         {
            return _source.LogLines.Concat(_source.SlotRuns.Values.SelectMany(x => x.UnitRuns).SelectMany(x => x.LogLines)).OrderBy(x => x.Index).GetEnumerator();
         }
      }

      /// <summary>
      /// Creates and returns a <see cref="LogLineEnumerable"/> from an <see cref="SlotRun"/> source.
      /// </summary>
      /// <param name="source">The enumeration source.</param>
      /// <returns>A <see cref="LogLineEnumerable"/> from an <see cref="SlotRun"/> source.</returns>
      public static LogLineEnumerable Create(SlotRun source)
      {
         return new SlotRunLogLineEnumerable(source);
      }

      private class SlotRunLogLineEnumerable : LogLineEnumerable
      {
         private readonly SlotRun _source;

         public SlotRunLogLineEnumerable(SlotRun source)
         {
            _source = source ?? throw new ArgumentNullException(nameof(source));
         }

         public override IEnumerator<LogLine> GetEnumerator()
         {
            return _source.UnitRuns.SelectMany(x => x.LogLines).OrderBy(x => x.Index).GetEnumerator();
         }
      }

      /// <summary>
      /// Creates and returns a <see cref="LogLineEnumerable"/> from an <see cref="UnitRun"/> source.
      /// </summary>
      /// <param name="source">The enumeration source.</param>
      /// <returns>A <see cref="LogLineEnumerable"/> from an <see cref="SlotRun"/> source.</returns>
      public static LogLineEnumerable Create(UnitRun source)
      {
         return new UnitRunLogLineEnumerable(source);
      }

      private class UnitRunLogLineEnumerable : LogLineEnumerable
      {
         private readonly UnitRun _source;

         public UnitRunLogLineEnumerable(UnitRun source)
         {
            _source = source ?? throw new ArgumentNullException(nameof(source));
         }

         public override IEnumerator<LogLine> GetEnumerator()
         {
            return _source.LogLines.GetEnumerator();
         }
      }
   }
}
