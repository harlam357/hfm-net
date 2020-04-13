/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

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
