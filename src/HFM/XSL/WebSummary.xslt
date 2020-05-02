<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns="http://www.w3.org/1999/xhtml"
                              xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
   <xsl:output method="html" encoding="utf-8" doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN" doctype-system="http://www.w3.org/TR/html4/loose.dtd" />
   <xsl:include href="FormatDate.xslt"/>
   <xsl:template match="SlotSummary">
      <html>
         <head>
            <title>Folding Client Summary</title>
            <meta http-equiv="Pragma" content="no-cache" />
            <meta http-equiv="Cache-Control" content="no-cache" />
            <link rel="stylesheet" type="text/css" href="$CSSFILE" />
         </head>
         <body>
            <table class="Overview" width="100%">
               <tr>
                  <td class="Heading" colspan="2">Summary</td>
                  <td class="Plain" colspan="14">
                     <a href="index.html">Overview Page</a>
                  </td>
               </tr>
               <tr>
                  <td class="Heading">Status</td>
                  <td class="Heading">Progress</td>
                  <td class="Heading">Name</td>
                  <td class="Heading">Client Type</td>
                  <td class="Heading">TPF</td>
                  <td class="Heading">PPD</td>
                  <td class="Heading">ETA</td>
                  <td class="Heading">Core</td>
                  <td class="Heading">Core ID</td>
                  <td class="Heading">Project (Run, Clone, Gen)</td>
                  <td class="Heading">Credit</td>
                  <td class="Heading">Completed</td>
                  <td class="Heading">Failed</td>
                  <td class="Heading">User Name</td>
                  <td class="Heading">Download Time</td>
                  <td class="Heading">Deadline</td>
               </tr>
               <xsl:apply-templates select="Slots/SlotData">
                  <xsl:with-param name="NumberFormat" select="NumberFormat" />
               </xsl:apply-templates>
               <tr>
                  <td class="Plain" colspan="18" align="center">
                  </td>
               </tr>
               <tr>
                  <td class="Heading" colspan="1">Totals</td>
                  <td class="Plain" colspan="1"></td>
                  <td class="Plain" colspan="16"></td>
               </tr>
               <tr>
                  <td class="RightCol" colspan="1">
                     <xsl:value-of select="SlotTotals/WorkingSlots"/> Clients
                  </td>
                  <td class="RightCol" colspan="1">
                     <xsl:value-of select="format-number(SlotTotals/PPD, NumberFormat)"/><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>PPD
                  </td>
                  <td class="Plain" colspan="16"></td>
               </tr>
               <tr>
                  <td class="Plain" colspan="18" align="center">
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="18" align="center">
                     Page rendered by <a href="https://github.com/harlam357/hfm-net">HFM.NET</a><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text><xsl:value-of select="HfmVersion"/> on <xsl:call-template name="FormatDate"><xsl:with-param name="dateTime" select="UpdateDateTime" /></xsl:call-template>
                  </td>
               </tr>
            </table>
         </body>
      </html>
   </xsl:template>
   <xsl:template match="Slots/SlotData">
      <xsl:param name="NumberFormat" />
      <tr>
         <td width="5%" class="StatusCol">
            <xsl:attribute name="bgcolor">
               <xsl:value-of select="StatusColor"/>
            </xsl:attribute>
            <font>
               <xsl:attribute name="color">
                  <xsl:value-of select="StatusFontColor"/>
               </xsl:attribute>
               <xsl:value-of select="Status"/>
            </font>
         </td>
         <td width="3%" class="RightCol">
            <xsl:value-of select="PercentComplete"/>%
         </td>
         <td width="15%">
            <xsl:choose>
               <xsl:when test="UserIdIsDuplicate='true'">
                  <xsl:attribute name="class">StatusCol</xsl:attribute>
                  <xsl:attribute name="bgcolor">Orange</xsl:attribute>
               </xsl:when>
               <xsl:otherwise>
                  <xsl:attribute name="class">RightCol</xsl:attribute>
               </xsl:otherwise>
            </xsl:choose>
            <a>
               <xsl:attribute name="href"><xsl:value-of select="Name"/>.html</xsl:attribute><xsl:value-of select="Name"/>
            </a>
         </td>
         <td width="5%" class="RightCol">
            <xsl:value-of select="SlotType"/>
         </td>
         <td width="5%" class="RightCol">
            <xsl:value-of select="TPF"/>
         </td>
         <td width="8%" class="RightCol">
            <xsl:value-of select="format-number(PPD, $NumberFormat)"/> (<xsl:value-of select="format-number(UPD, $NumberFormat)"/> WUs)
         </td>
         <td width="8%" class="RightCol">
            <xsl:value-of select="ETA"/>
         </td>
         <td width="8%" class="RightCol">
            <xsl:value-of select="Core"/>
         </td>
         <td width="2%" class="RightCol">
            <xsl:value-of select="CoreId"/>
         </td>
         <td width="8%">
            <xsl:choose>
               <xsl:when test="ProjectIsDuplicate='true'">
                  <xsl:attribute name="class">StatusCol</xsl:attribute>
                  <xsl:attribute name="bgcolor">Orange</xsl:attribute>
               </xsl:when>
               <xsl:otherwise>
                  <xsl:attribute name="class">RightCol</xsl:attribute>
               </xsl:otherwise>
            </xsl:choose>
            <xsl:value-of select="ProjectRunCloneGen"/>
         </td>
         <td width="3%" class="RightCol">
            <xsl:value-of select="format-number(Credit, $NumberFormat)"/>
         </td>
         <td width="3%" class="RightCol">
            <xsl:value-of select="Completed"/>
         </td>
         <td width="3%" class="RightCol">
            <xsl:value-of select="Failed"/>
         </td>
         <td width="8%">
            <xsl:choose>
               <xsl:when test="UsernameOk='false'">
                  <xsl:attribute name="class">StatusCol</xsl:attribute>
                  <xsl:attribute name="bgcolor">Orange</xsl:attribute>
               </xsl:when>
               <xsl:otherwise>
                  <xsl:attribute name="class">RightCol</xsl:attribute>
               </xsl:otherwise>
            </xsl:choose>
            <xsl:value-of select="Username"/>
         </td>
         <td width="8%" class="RightCol">
            <xsl:value-of select="DownloadTime"/>
         </td>
         <td width="8%" class="RightCol"> <!--100%-->
            <xsl:value-of select="PreferredDeadline"/>
         </td>
      </tr>
   </xsl:template>
</xsl:stylesheet>
