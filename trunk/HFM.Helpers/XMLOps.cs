/*
 * HFM.NET - XML Generation Helper Class
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

using HFM.Preferences;

namespace HFM.Helpers
{
   /// <summary>
   /// Helper class for generating XML and HTML
   /// </summary>
   public static class XMLOps
   {
      /// <summary>
      /// Writes the appropriate text (nodeText) into the identified XML node (xPath) within the document (xmlData)
      /// </summary>
      /// <param name="xmlData">XML Document to use as source for node</param>
      /// <param name="xPath">Path into XML document</param>
      /// <param name="nodeText">String to write into the InnerText property</param>
      public static void setXmlNode(XmlElement xmlData, String xPath, String nodeText)
      {
         XmlNode xNode = xmlData.SelectSingleNode(xPath);
         if (xNode != null)
         {
            xNode.InnerText = nodeText;
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="xmlDoc"></param>
      /// <param name="xPath"></param>
      /// <param name="nodeText"></param>
      /// <returns></returns>
      public static XmlElement createXmlNode(XmlDocument xmlDoc, String xPath, String nodeText)
      {
         XmlElement xmlEl = xmlDoc.CreateElement(xPath);
         XmlNode xmlN = xmlDoc.CreateNode(XmlNodeType.Text, xPath, String.Empty);
         xmlN.Value = nodeText;
         xmlEl.AppendChild(xmlN);

         return xmlEl;
      }

      /// <summary>
      /// Transforms an XML document using the specified XSL file
      /// </summary>
      /// <param name="xmlDoc">XML source document</param>
      /// <param name="xslFile">XSL transform to apply (filename only)</param>
      /// <returns>Result of XML document after transform is applied</returns>
      public static String Transform(XmlDocument xmlDoc, String xslFile)
      {
         XslCompiledTransform xslt = new XslCompiledTransform();
         XmlReaderSettings xsltSettings = new XmlReaderSettings();
         xsltSettings.ProhibitDtd = false;
         XmlReader xmlReader = XmlReader.Create(Path.Combine(Path.Combine(PreferenceSet.AppPath, "XSL"), xslFile), xsltSettings);
         xslt.Load(xmlReader);

         StringBuilder sb = new StringBuilder();
         TextWriter tw = new StringWriter(sb);

         // Transform the XML data to an in memory stream - which happens to be a string
         xslt.Transform(xmlDoc, null, tw);

         // Return the transformed XML
         string sWebPage = sb.ToString();
         sWebPage = sWebPage.Replace("$APPPATH", PreferenceSet.AppPath);
         sWebPage = sWebPage.Replace("$CSSFILE", PreferenceSet.Instance.CSSFileName);
         return sWebPage;
      }
   }
}