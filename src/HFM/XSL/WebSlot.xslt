<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns="http://www.w3.org/1999/xhtml"
                              xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
   <xsl:output method="html" encoding="utf-8" doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN" doctype-system="http://www.w3.org/TR/html4/loose.dtd" />
   <xsl:include href="FormatDate.xslt"/>
   <xsl:template match="SlotDetail">
      <html>
         <head>
            <title>Slot Data</title>
            <meta http-equiv="Pragma" content="no-cache" />
            <meta http-equiv="Cache-Control" content="no-cache" />
            <link rel="stylesheet" type="text/css" href="$CSSFILE" />
         </head>
         <body>
            <table class="Instance">
               <tr>
                  <td class="Heading">
                     <xsl:value-of select="GridData/Name"/>
                  </td>
                  <td class="Plain">
                     <a href="index.html">Overview Page</a>
                     <br />
                     <a href="summary.html">Summary Page</a>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">
                     Current Progress
                  </td>
                  <td class="AltRightCol">
                     <xsl:value-of select="GridData/PercentComplete"/> Percent Complete
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Time Per Frame
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="GridData/TPF"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">
                     PPD
                  </td>
                  <td class="AltRightCol">
                     <xsl:value-of select="GridData/PPD"/> (<xsl:value-of select="GridData/UPD"/> WUs)
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     PPW
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="format-number(GridData/PPD * 7, NumberFormat)"/> (<xsl:value-of select="format-number(GridData/UPD * 7, NumberFormat)"/> WUs)
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">
                     Work Units
                  </td>
                  <td class="AltRightCol">
                     Completed: <xsl:value-of select="TotalRunCompletedUnits"/> - Failed: <xsl:value-of select="TotalRunFailedUnits"/> - Total: <xsl:value-of select="TotalCompletedUnits"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Download Time
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="GridData/DownloadTime"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">
                     Expected Completion
                  </td>
                  <td class="AltRightCol">
                     <xsl:value-of select="GridData/ETA"/>
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
                  </td>
               </tr>
               <tr>
                  <td class="Heading">
                     Log File
                  </td>
                  <td class="Plain">
                     <xsl:choose>
                        <xsl:when test="LogFileAvailable='true' and LogFileName!=''">
                           <a><xsl:attribute name="href"><xsl:value-of select="LogFileName"/></xsl:attribute>Full Log File</a>
                        </xsl:when>
                     </xsl:choose>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" colspan="2">
                     <xsl:for-each select="CurrentLogLines/LogLine">
                        <xsl:value-of select="LineRaw" disable-output-escaping="yes"/><xsl:element name="br"/>
                     </xsl:for-each>
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
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
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
                     Page rendered by <a href="http://code.google.com/p/hfm-net/">HFM.NET</a><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text><xsl:value-of select="HfmVersion"/> on <xsl:call-template name="FormatDate"><xsl:with-param name="dateTime" select="UpdateDateTime" /></xsl:call-template>
                  </td>
               </tr>
            </table>
         </body>
      </html>
   </xsl:template>
   <xsl:template match="Protein">
      <tr>
         <td class="AltLeftCol">Project ID</td>
         <td class="AltRightCol">
            <xsl:value-of select="ProjectNumber"/>
         </td>
      </tr>
      <tr>
         <td class="LeftCol">Work Unit</td>
         <td class="RightCol">
            <xsl:value-of select="WorkUnitName"/>
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">Credit</td>
         <td class="AltRightCol">
            <xsl:value-of select="Credit"/>
         </td>
      </tr>
      <tr>
         <td class="LeftCol">KFactor</td>
         <td class="RightCol">
            <xsl:value-of select="KFactor"/>
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">Number of Frames</td>
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
         <td class="AltLeftCol">Number of Atoms</td>
         <td class="AltRightCol">
            <xsl:value-of select="NumberOfAtoms"/>
         </td>
      </tr>
      <tr>
         <td class="LeftCol">Preferred Deadline</td>
         <td class="RightCol">
            <xsl:value-of select="PreferredDays"/> Days
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">Final Deadline</td>
         <td class="AltRightCol">
            <xsl:value-of select="MaximumDays"/> Days
         </td>
      </tr>
      <tr>
         <td class="LeftCol">Contact Person</td>
         <td class="RightCol">
            <xsl:value-of select="Contact"/>
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">Server IP</td>
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
