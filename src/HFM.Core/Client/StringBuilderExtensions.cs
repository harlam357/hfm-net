using System;
using System.Collections.Generic;
using System.Text;

namespace HFM.Core.Client
{
    internal static class StringBuilderExtensions
    {
        // use maximum chunk size equal to that used by StringBuilder itself
        // https://github.com/dotnet/runtime/blob/master/src/libraries/System.Private.CoreLib/src/System/Text/StringBuilder.cs
        private const int MaxChunkSize = 8000;

        // TODO: when upgrading to .netcore 3+, use StringBuilder.GetChunks() as IEnumerable<ReadOnlyMemory<char>>
        internal static IEnumerable<char[]> GetChunks(this StringBuilder sb)
        {
            if (sb == null) throw new ArgumentNullException(nameof(sb));

            for (int i = 0; i < sb.Length; i += MaxChunkSize)
            {
                int length = i + MaxChunkSize < sb.Length ? MaxChunkSize : sb.Length - i;
                var chunkCopy = new char[length];
                sb.CopyTo(i, chunkCopy, 0, chunkCopy.Length);
                yield return chunkCopy;
            }
        }
    }
}
