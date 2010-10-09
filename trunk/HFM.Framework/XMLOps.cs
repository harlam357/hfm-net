/*
 * HFM.NET - XML Generation Helper Class
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

namespace HFM.Framework
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
      public static void setXmlNode(XmlElement xmlData, string xPath, string nodeText)
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
      public static XmlElement createXmlNode(XmlDocument xmlDoc, string xPath, string nodeText)
      {
         XmlElement xmlEl = xmlDoc.CreateElement(xPath);
         XmlNode xmlN = xmlDoc.CreateNode(XmlNodeType.Text, xPath, String.Empty);
         xmlN.Value = nodeText;
         xmlEl.AppendChild(xmlN);

         return xmlEl;
      }

      /// <summary>
      /// Transforms an XML Document using the given XSLT file
      /// </summary>
      /// <param name="xmlDoc">XML Source Document/Node</param>
      /// <param name="xsltFilePath">Path to the XSL Transform to apply</param>
      /// <param name="cssFileName">CSS file name to embed in the transformed XML</param>
      public static string Transform(XmlNode xmlDoc, string xsltFilePath, string cssFileName)
      {
         // Create XmlReaderSettings and XmlReader
         var xsltSettings = new XmlReaderSettings();
         xsltSettings.ProhibitDtd = false;
         XmlReader xmlReader = XmlReader.Create(xsltFilePath, xsltSettings);

         // Create the XslCompiledTransform and Load the XmlReader
         var xslt = new XslCompiledTransform();
         xslt.Load(xmlReader);

         // Transform the XML data to an in memory stream
         var ms = new MemoryStream();
         xslt.Transform(xmlDoc, null, ms);
         
         // Return the transformed XML
         string sWebPage = Encoding.UTF8.GetString(ms.ToArray());
         sWebPage = sWebPage.Replace("$CSSFILE", cssFileName);
         return sWebPage;
      }
   }
}
