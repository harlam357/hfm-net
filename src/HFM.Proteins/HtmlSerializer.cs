
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using Majestic12;

using HFM.Core.DataTypes;
using HFM.Core.DataTypes.Serializers;

namespace HFM.Proteins
{
   public class HtmlSerializer : IFileSerializer<List<Protein>>
   {
      private readonly HTMLparser _htmlParser;

      public HtmlSerializer()
      {
         _htmlParser = new HTMLparser();
      }

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
         var proteins = new List<Protein>();

         string[] lines = File.ReadAllLines(fileName);
         foreach (string line in lines)
         {
            try
            {
               Protein p = ParseProteinRow(line);
               if (p != null && p.IsValid())
               {
                  proteins.Add(p);
               }
            }
            catch (Exception)
            {
               Debug.Assert(false);   
            }
         }

         return proteins;
      }

      #region HTML Parsing Methods

      /// <summary>
      /// Parse the HTML Table Row (tr) into a Protein Instance.
      /// </summary>
      private Protein ParseProteinRow(string html)
      {
         _htmlParser.Init(html);
         var p = new Protein();

         HTMLchunk oChunk;
         while ((oChunk = _htmlParser.ParseNext()) != null)
         {
            // Look for an Open "tr" Tag
            if (oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                oChunk.sTag.ToLower() == "tr")
            {
               int projectNumber;
               if (Int32.TryParse(GetNextTdValue(_htmlParser), NumberStyles.Integer, CultureInfo.InvariantCulture,
                                    out projectNumber))
               {
                  p.ProjectNumber = projectNumber;
               }
               else
               {
                  return null;
               }
               p.ServerIP = GetNextTdValue(_htmlParser);
               p.WorkUnitName = GetNextTdValue(_htmlParser);
               try
               {
                  p.NumberOfAtoms = Int32.Parse(GetNextTdValue(_htmlParser), CultureInfo.InvariantCulture);
               }
               catch (FormatException)
               {
                  p.NumberOfAtoms = 0;
               }
               p.PreferredDays = Double.Parse(GetNextTdValue(_htmlParser), CultureInfo.InvariantCulture);
               p.MaximumDays = Double.Parse(GetNextTdValue(_htmlParser), CultureInfo.InvariantCulture);
               p.Credit = Double.Parse(GetNextTdValue(_htmlParser), CultureInfo.InvariantCulture);
               p.Frames = Int32.Parse(GetNextTdValue(_htmlParser), CultureInfo.InvariantCulture);
               p.Core = GetNextTdValue(_htmlParser);
               p.Description = GetNextTdValue(_htmlParser, "href");
               p.Contact = GetNextTdValue(_htmlParser);
               p.KFactor = Double.Parse(GetNextTdValue(_htmlParser), CultureInfo.InvariantCulture);

               return p;
            }
         }

         return null;
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
