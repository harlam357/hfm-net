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
                     <xsl:value-of select="SlotData/Name"/>
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
                     <xsl:value-of select="SlotData/PercentComplete"/>% Complete
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     TPF (Time per Frame)
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="SlotData/TPF"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">
                     PPD (Points per Day)
                  </td>
                  <td class="AltRightCol">
                     <xsl:value-of select="format-number(SlotData/PPD, NumberFormat)"/> (<xsl:value-of select="format-number(SlotData/UPD, NumberFormat)"/> WUs)
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     PPW (Points per Week)
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="format-number(SlotData/PPD * 7, NumberFormat)"/> (<xsl:value-of select="format-number(SlotData/UPD * 7, NumberFormat)"/> WUs)
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">
                     Work Unit Statistics for this Slot
                  </td>
                  <td class="AltRightCol">
                     Completed: <xsl:value-of select="TotalRunCompletedUnits"/> - Failed: <xsl:value-of select="TotalRunFailedUnits"/> - Total Completed: <xsl:value-of select="TotalCompletedUnits"/> - Total Failed: <xsl:value-of select="TotalFailedUnits"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Assigned
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="SlotData/DownloadTime"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">
                     ETA (Estimated Time of Arrival / Completion)
                  </td>
                  <td class="AltRightCol">
                     <xsl:value-of select="SlotData/ETA"/>
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
                           <a><xsl:attribute name="href"><xsl:value-of select="LogFileName"/></xsl:attribute>Click here to view the client log file</a>
                        </xsl:when>
                     </xsl:choose>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" colspan="2">
                     <xsl:for-each select="SlotData/CurrentLogLines/LogLine">
                        <xsl:value-of select="Raw" disable-output-escaping="yes"/><xsl:element name="br"/>
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
               <xsl:apply-templates select="SlotData/Protein" />
               <tr>
                  <td class="Plain" colspan="2" align="center">
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
                     Page rendered by <a href="https://github.com/harlam357/hfm-net">HFM.NET</a><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text><xsl:value-of select="HfmVersion"/> on <xsl:call-template name="FormatDate"><xsl:with-param name="dateTime" select="UpdateDateTime" /></xsl:call-template>
                  </td>
               </tr>
            </table>
         </body>
      </html>
   </xsl:template>
   <xsl:template match="SlotData/Protein">
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
         <td class="AltLeftCol">Base Credit</td>
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
         <td class="LeftCol">Timeout</td>
         <td class="RightCol">
            <xsl:value-of select="PreferredDays"/> Days
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">Expiration</td>
         <td class="AltRightCol">
            <xsl:value-of select="MaximumDays"/> Days
         </td>
      </tr>
      <tr>
         <td class="LeftCol">Contact</td>
         <td class="RightCol">
            <xsl:value-of select="Contact"/>
         </td>
      </tr>
      <tr>
         <td class="AltLeftCol">Work Server</td>
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
