/*
 * HFM.NET - HTML Serializer
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Majestic12;

using HFM.Core.DataTypes;
using HFM.Core.Plugins;

namespace HFM.Proteins
{
   public class HtmlSerializer : IFileSerializer<List<Protein>>
   {
      #region IFileSerializer<List<Protein>> Members

      public string FileExtension
      {
         get { return "html"; }
      }

      public string FileTypeFilter
      {
         get { return "HTML Files|*.html"; }
      }

      public List<Protein> Deserialize(string fileName)
      {
         string text = File.ReadAllText(fileName);
         return ParseProteins(text);
      }

      #region HTML Parsing Methods

      private static List<Protein> ParseProteins(string html)
      {
         var htmlParser = new HTMLparser();
         htmlParser.Init(html);
         var list = new List<Protein>();

         HTMLchunk chunk;
         while ((chunk = htmlParser.ParseNext()) != null)
         {
            // Look for an Open "tr" Tag
            if (chunk.oType.Equals(HTMLchunkType.OpenTag) &&
                chunk.sTag.ToLower() == "tr")
            {
               var p = new Protein();
               int projectNumber;
               if (Int32.TryParse(GetNextTdValue(htmlParser), NumberStyles.Integer, CultureInfo.InvariantCulture, out projectNumber))
               {
                  p.ProjectNumber = projectNumber;
               }
               else
               {
                  continue;
               }
               p.ServerIP = GetNextTdValue(htmlParser);
               p.WorkUnitName = GetNextTdValue(htmlParser);
               try
               {
                  p.NumberOfAtoms = Int32.Parse(GetNextTdValue(htmlParser), CultureInfo.InvariantCulture);
               }
               catch (FormatException)
               {
                  p.NumberOfAtoms = 0;
               }
               p.PreferredDays = Double.Parse(GetNextTdValue(htmlParser), CultureInfo.InvariantCulture);
               p.MaximumDays = Double.Parse(GetNextTdValue(htmlParser), CultureInfo.InvariantCulture);
               p.Credit = Double.Parse(GetNextTdValue(htmlParser), CultureInfo.InvariantCulture);
               p.Frames = Int32.Parse(GetNextTdValue(htmlParser), CultureInfo.InvariantCulture);
               p.Core = GetNextTdValue(htmlParser);
               p.Description = GetNextTdValue(htmlParser, "href");
               p.Contact = GetNextTdValue(htmlParser);
               p.KFactor = Double.Parse(GetNextTdValue(htmlParser), CultureInfo.InvariantCulture);

               list.Add(p);
            }
         }

         return list;
      }

      /// <summary>
      /// Return the value enclosed in the HTML Table Column (td).
      /// </summary>
      /// <param name="pSummary">HTMLparser Instance</param>
      [CLSCompliant(false)]
      public static string GetNextTdValue(HTMLparser pSummary)
      {
         return GetNextTdValue(pSummary, String.Empty);
      }

      /// <summary>
      /// Return the value enclosed in the HTML Table Column (td).
      /// </summary>
      /// <param name="htmlParser">HTMLparser Instance</param>
      /// <param name="paramName">Name of Tag Parameter to Return</param>
      [CLSCompliant(false)]
      public static string GetNextTdValue(HTMLparser htmlParser, string paramName)
      {
         return GetNextValue(htmlParser, "td", paramName);
      }

      /// <summary>
      /// Return the value enclosed in the HTML Table Heading Column (th).
      /// </summary>
      /// <param name="htmlParser">HTMLparser Instance</param>
      [CLSCompliant(false)]
      public static string GetNextThValue(HTMLparser htmlParser)
      {
         return GetNextValue(htmlParser, "th", String.Empty);
      }

      /// <summary>
      /// Return the value enclosed in the HTML Table Column (td).
      /// </summary>
      /// <param name="htmlParser">HTMLparser Instance</param>
      /// <param name="tagName">Name of Tag to Search for</param>
      /// <param name="paramName">Name of Tag Parameter to Return</param>
      [CLSCompliant(false)]
      public static string GetNextValue(HTMLparser htmlParser, string tagName, string paramName)
      {
         HTMLchunk oChunk;
         while ((oChunk = htmlParser.ParseNext()) != null)
         {
            // Look for an Open Tag matching the given Tag Name
            if (oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                oChunk.sTag.ToLower() == tagName)
            {
               // If not looking for a Tag Parameter
               if (paramName.Length == 0)
               {
                  // Look inside the "td" Tag
                  oChunk = htmlParser.ParseNext();
                  if (oChunk != null)
                  {
                     // If it's an Open "font" Tag
                     if (oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                         oChunk.sTag.ToLower() == "font")
                     {
                        // Look inside the "font" Tag
                        oChunk = htmlParser.ParseNext();

                        // If it's Text, return it
                        if (oChunk != null &&
                            oChunk.oType.Equals(HTMLchunkType.Text))
                        {
                           return oChunk.oHTML.Trim();
                        }
                     }
                     // If it's Text, return it
                     else if (oChunk.oType.Equals(HTMLchunkType.Text))
                     {
                        return oChunk.oHTML.Trim();
                     }
                  }
               }
               // Looking for a Tag Parameter
               else
               {
                  // Look inside the "td" Tag
                  oChunk = htmlParser.ParseNext();

                  // If it's an Open Tag
                  if (oChunk != null &&
                      oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                      oChunk.oParams.Contains(paramName))
                  {
                     // Return the specified Parameter Name
                     return oChunk.oParams[paramName].ToString();
                  }
               }

               return String.Empty;
            }
         }

         //throw new InvalidOperationException("Could not complete operation to get td tag value.");
         return String.Empty;
      }
      #endregion

      public void Serialize(string fileName, List<Protein> value)
      {
         throw new NotSupportedException("HTML serialization is not supported.");
      }

      #endregion
   }
}
