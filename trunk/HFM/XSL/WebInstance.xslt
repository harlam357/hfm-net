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
                     Log<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>File
                  </td>
                  <td class="Plain" align="right">
                     <xsl:choose>
                        <xsl:when test="UnitLog/FullLogFile=''">
                           <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
                        </xsl:when>
                        <xsl:otherwise>
                           <a>
                              <xsl:attribute name="href"><xsl:value-of select="UnitLog/FullLogFile"/></xsl:attribute>
                              FAHlog<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>File
                           </a>
                        </xsl:otherwise>
                     </xsl:choose>
                  </td>
               </tr>
               <xsl:apply-templates select="UnitLog" />
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
                     Project
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
            Current<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Progress
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="FramesComplete"/> Frames Complete (<xsl:value-of select="PercentComplete"/>%)
         </td>
      </tr>
      <tr>
         <td class="LeftCol">
            Time<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Per<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Frame
         </td>
         <td class="RightCol">
            <xsl:value-of select="TimePerFrame"/>
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">
            PPD
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="EstPPD"/> (<xsl:value-of select="EstUPD"/> WUs)
         </td>
      </tr>
      <tr>
         <td class="LeftCol">
            PPW
         </td>
         <td class="RightCol">
            <xsl:value-of select="EstPPW"/> (<xsl:value-of select="EstUPW"/> WUs)
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">
            Work<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Units
         </td>
         <td class="AltRightCol">
            Completed: <xsl:value-of select="CompletedProjects"/> - Failed: <xsl:value-of select="FailedProjects"/> - Total: <xsl:value-of select="TotalProjects"/>
         </td>
      </tr>
      <tr>
         <td class="LeftCol">
            Download<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Time
         </td>
         <td class="RightCol">
            <xsl:value-of select="DownloadTime"/>
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">
            Expected<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Completion
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="ExpectedCompletionDate"/>
         </td>
      </tr>
   </xsl:template>
   <xsl:template match="UnitLog">
      <tr>
         <td class="LeftCol" colspan="2">
            <xsl:value-of select="Text" disable-output-escaping="yes"/>
         </td>
      </tr>
   </xsl:template>
   <xsl:template match="Protein">
      <tr>
         <td class="AltLeftCol">
            Project<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>ID
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="ProjectNumber"/>
         </td>
      </tr>
      <tr>
         <td class="LeftCol">Work Unit</td>
         <td class="RightCol">
            <xsl:value-of select="WorkUnit"/>
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">
            Credit
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="Credit"/>
         </td>
      </tr>
      <tr>
         <td class="LeftCol">
            KFactor
         </td>
         <td class="RightCol">
            <xsl:value-of select="KFactor"/>
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
            <xsl:value-of select="PreferredDays"/> Days
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">
            Final<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Deadline
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="MaxDays"/> Days
         </td>
      </tr>
      <tr>
         <td class="LeftCol">
            Contact<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Person
         </td>
         <td class="RightCol">
            <xsl:value-of select="Contact"/>
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">
            Server<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>IP
         </td>
         <td class="AltRightCol">
            <xsl:value-of select="ServerIP"/>
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
