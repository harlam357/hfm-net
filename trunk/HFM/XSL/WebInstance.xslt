<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
   <xsl:output method="html" encoding="utf-8" doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN" doctype-system="http://www.w3.org/TR/html4/loose.dtd" />
   <xsl:template match="Instance">
      <html>
         <head>
            <title>Client Instance Data</title>
            <meta http-equiv="Pragma" content="no-cache" />
            <meta http-equiv="Cache-Control" content="no-cache" />
            <link rel="stylesheet" type="text/css" href="$CSSFILE" />
         </head>
         <body>
            <table class="Instance">
               <tr>
                  <td class="Heading">
                     <xsl:value-of select="@Name"/>
                  </td>
                  <td class="Plain" colspan="2" align="right">
                     <a href="index.html">Overview Page</a>
                     <br />
                     <a href="summary.html">Summary Page</a>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">Protein</td>
                  <td class="RightCol">
                     <xsl:value-of select="Protein/WorkUnit"/>
                  </td>
               </tr>
               <xsl:apply-templates select="UnitInfo" />
               <tr>
                  <td class="Empty">
                     <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
                  </td>
                  <td class="Empty">
                     <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
                  </td>
               </tr>
               <tr>
                  <td class="Heading">
                     <xsl:value-of select="Protein/WorkUnit"/>
                  </td>
                  <td class="Blank"> </td>
               </tr>
               <xsl:apply-templates select="Protein" />
               <tr>
                  <td class="Plain" colspan="2" align="center">
                  </td>
                  <td class="Plain" colspan="2" align="center">
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
                     Page rendered by <a href="http://code.google.com/p/hfm-net/">HFM.NET</a><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text><xsl:value-of select="HFMVersion"/> on <xsl:value-of select="LastUpdatedDate"/>
                     at <xsl:value-of select="LastUpdatedTime"/>
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
                     Data last updated on <xsl:value-of select="LastRetrievedDate"/>
                     at <xsl:value-of select="LastRetrievedTime"/>
                  </td>
               </tr>
            </table>
         </body>
      </html>
   </xsl:template>
   <xsl:template match="UnitInfo">
      <tr>
         <td class="AltLeftCol">
            Download<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Time
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="DownloadTime"/>
         </td>
      </tr>
      <tr>
         <td class="LeftCol">
            Current<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Progress
         </td>
         <td class="RightCol">
            <xsl:value-of select="FramesComplete"/> frames complete (<xsl:value-of select="PercentComplete"/>%)
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">
            Time<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Per<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Frame
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="TimePerFrame"/>
         </td>
      </tr>
      <tr>
         <td class="LeftCol">
            Expected<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Completion
         </td>
         <td class="RightCol">
            <xsl:value-of select="ExpectedCompletionDate"/>
         </td>
      </tr>
   </xsl:template>
   <xsl:template match="Protein">
      <tr>
         <td class="LeftCol">Work Unit</td>
         <td class="RightCol">
            <xsl:value-of select="WorkUnit"/>
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">
            Project<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Number
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="ProjectNumber"/>
         </td>
      </tr>
      <tr>
         <td class="LeftCol">
            Server<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Address
         </td>
         <td class="RightCol">
            <xsl:value-of select="ServerIP"/>
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">
            Number<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>of<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Atoms
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="NumAtoms"/>
         </td>
      </tr>
      <tr>
         <td class="LeftCol">
            Preferred<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Deadline
         </td>
         <td class="RightCol">
            <xsl:value-of select="PreferredDays"/> days
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">
            Final<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Deadline
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="MaxDays"/> days
         </td>
      </tr>
      <tr>
         <td class="LeftCol">
            Points<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Credit
         </td>
         <td class="RightCol">
            <xsl:value-of select="Credit"/>
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">
            Number<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>of<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Frames
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="Frames"/>
         </td>
      </tr>
      <tr>
         <td class="LeftCol">Core</td>
         <td class="RightCol">
            <xsl:value-of select="Core"/>
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">
            Contact<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Person
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="Contact"/>
         </td>
      </tr>
      <tr>
         <td class="LeftCol">Description</td>
         <td class="RightCol">
            <xsl:value-of select="Description" disable-output-escaping="yes"/>
         </td>
      </tr>
   </xsl:template>
</xsl:stylesheet>