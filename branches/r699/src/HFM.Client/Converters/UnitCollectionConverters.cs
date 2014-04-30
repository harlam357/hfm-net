/*
 * HFM.NET - Unit Collection Data Converters
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Globalization;
using System.Text.RegularExpressions;

using HFM.Client.DataTypes;

namespace HFM.Client.Converters
{
   internal sealed class UnitStatusConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         switch (inputString)
         {
            case "RUNNING":
               return FahUnitStatus.Running;
            case "DOWNLOAD":
               return FahUnitStatus.Download;
            case "SEND":
               return FahUnitStatus.Send;
            case "READY":
               return FahUnitStatus.Ready;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse status value of '{0}'.", inputString));
      }
   }

   internal sealed class UnitTimeSpanConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;

         // should be able to do all this with a single regular expression

         Exception innerException = null;

         try
         {
            var regex1 = new Regex("(?<Hours>.+) hours (?<Minutes>.+) mins (?<Seconds>.+) secs", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            Match matchRegex1;
            if ((matchRegex1 = regex1.Match(inputString)).Success)
            {
               return new TimeSpan(System.Convert.ToInt32(matchRegex1.Result("${Hours}"), CultureInfo.InvariantCulture),
                                   System.Convert.ToInt32(matchRegex1.Result("${Minutes}"), CultureInfo.InvariantCulture),
                                   System.Convert.ToInt32(matchRegex1.Result("${Seconds}"), CultureInfo.InvariantCulture));
            }

            var regex2 = new Regex("(?<Hours>.+) hours (?<Minutes>.+) mins", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            Match matchRegex2;
            if ((matchRegex2 = regex2.Match(inputString)).Success)
            {
               return new TimeSpan(System.Convert.ToInt32(matchRegex2.Result("${Hours}"), CultureInfo.InvariantCulture),
                                   System.Convert.ToInt32(matchRegex2.Result("${Minutes}"), CultureInfo.InvariantCulture),
                                   0);
            }

            var regex3 = new Regex("(?<Minutes>.+) mins (?<Seconds>.+) secs", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            Match matchRegex3;
            if ((matchRegex3 = regex3.Match(inputString)).Success)
            {
               return new TimeSpan(0,
                                   System.Convert.ToInt32(matchRegex3.Result("${Minutes}"), CultureInfo.InvariantCulture),
                                   System.Convert.ToInt32(matchRegex3.Result("${Seconds}"), CultureInfo.InvariantCulture));
            }

            var regex4 = new Regex("(?<Seconds>.+) secs", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            Match matchRegex4;
            if ((matchRegex4 = regex4.Match(inputString)).Success)
            {
               return TimeSpan.FromSeconds(System.Convert.ToDouble(matchRegex4.Result("${Seconds}"), CultureInfo.InvariantCulture));
            }

            var regex5 = new Regex("(?<Days>.+) days", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            Match matchRegex5;
            if ((matchRegex5 = regex5.Match(inputString)).Success)
            {
               return TimeSpan.FromDays(System.Convert.ToDouble(matchRegex5.Result("${Days}"), CultureInfo.InvariantCulture));
            }
         }
         catch (FormatException ex)
         {
            innerException = ex;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse time span value of '{0}'.", inputString), innerException);
      }
   }
}
