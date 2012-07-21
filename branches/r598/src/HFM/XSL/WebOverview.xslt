<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns="http://www.w3.org/1999/xhtml"
                              xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
   <xsl:output method="html" encoding="utf-8" doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN" doctype-system="http://www.w3.org/TR/html4/loose.dtd" />
   <xsl:include href="FormatDate.xslt"/>
   <xsl:template match="SlotSummary">
      <html>
         <head>
            <title>Folding Client Overview</title>
            <meta http-equiv="Pragma" content="no-cache" />
            <meta http-equiv="Cache-Control" content="no-cache" />
            <link rel="stylesheet" type="text/css" href="$CSSFILE" />
         </head>
         <body>
            <xsl:variable name="PPW" select="SlotTotals/PPD * 7"/>
            <xsl:variable name="UPW" select="SlotTotals/UPD * 7"/>
            <table class="Overview" width="32%">
               <tr>
                  <td class="Heading" width="50%">Overview</td>
                  <td class="Plain" width="50%">
                     <a href="summary.html">
                        Summary Page
                     </a>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Total Folding Slots
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="SlotTotals/TotalSlots"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">
                     Working Slots
                  </td>
                  <td class="AltRightCol">
                     <xsl:value-of select="SlotTotals/WorkingSlots"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Non-Working Slots
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="SlotTotals/TotalSlots - SlotTotals/WorkingSlots"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">
                     Total PPD
                  </td>
                  <td class="AltRightCol">
                     <xsl:value-of select="format-number(SlotTotals/PPD, NumberFormat)"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Total PPW
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="format-number($PPW, NumberFormat)"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">
                     Total UPD
                  </td>
                  <td class="AltRightCol">
                     <xsl:value-of select="format-number(SlotTotals/UPD, NumberFormat)"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Total UPW
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="format-number($UPW, NumberFormat)"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">
                     Average PPD
                  </td>
                  <td class="AltRightCol">
                     <xsl:value-of select="format-number(SlotTotals/PPD div SlotTotals/WorkingSlots, NumberFormat)"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Average PPW
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="format-number($PPW div SlotTotals/WorkingSlots, NumberFormat)"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">
                     Average UPD
                  </td>
                  <td class="AltRightCol">
                     <xsl:value-of select="format-number(SlotTotals/UPD div SlotTotals/WorkingSlots, NumberFormat)"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Average UPW
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="format-number($UPW div SlotTotals/WorkingSlots, NumberFormat)"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">Completed Units</td>
                  <td class="AltRightCol">
                     <xsl:value-of select="SlotTotals/TotalRunCompletedUnits"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">Failed Units</td>
                  <td class="RightCol">
                     <xsl:value-of select="SlotTotals/TotalRunFailedUnits"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol">Total Completed Units</td>
                  <td class="AltRightCol">
                     <xsl:value-of select="SlotTotals/TotalCompletedUnits"/>
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
                     <a href="mobile.html">
                        Mobile Version
                     </a>
                  </td>
               </tr>
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
</xsl:stylesheet>
