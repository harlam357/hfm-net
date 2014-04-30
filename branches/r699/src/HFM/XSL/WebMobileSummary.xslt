<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns="http://www.w3.org/1999/xhtml"
                              xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
   <xsl:output method="html" encoding="utf-8" doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN" doctype-system="http://www.w3.org/TR/html4/loose.dtd" />
   <xsl:include href="FormatDate.xslt"/>
   <xsl:template match="SlotSummary">
      <html>
         <head>
            <title>Folding Client Summary (mobile)</title>
            <meta http-equiv="Pragma" content="no-cache" />
            <meta http-equiv="Cache-Control" content="no-cache" />
            <link rel="stylesheet" type="text/css" href="$CSSFILE" />
         </head>
         <body>
            <table class="Overview" width="85">
               <tr>
                  <td class="Heading" colspan="2">Mobile Summary</td>
                  <td class="Plain" colspan="4">
                     <a href="mobile.html">Overview Page</a>
                  </td>
               </tr>
               <tr>
                  <td class="Heading" width="5"></td>
                  <td class="Heading" width="10">%</td>
                  <td class="Heading" width="40">Name</td>
                  <td class="Heading" width="30">PPD</td>
               </tr>
               <xsl:apply-templates select="Slots/SlotData" />
               <tr>
                  <td class="Plain" colspan="4" align="center">
                  </td>
               </tr>
               <tr>
                  <td class="Heading" colspan="2">Totals</td>
                  <td class="Plain" colspan="2"></td>
               </tr>
               <tr>
                  <td class="RightCol" colspan="2">
                     <xsl:value-of select="SlotTotals/WorkingSlots"/> Clients
                  </td>
                  <td class="RightCol" colspan="2">
                     <xsl:value-of select="format-number(SlotTotals/PPD, NumberFormat)"/> PPD
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="4" align="center">
                     <a href="summary.html">
                        Standard Version
                     </a>
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="4" align="center">
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="4" align="center">
                     Page rendered by <a href="http://code.google.com/p/hfm-net/">HFM.NET</a><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text><xsl:value-of select="HfmVersion"/> on <xsl:call-template name="FormatDate"><xsl:with-param name="dateTime" select="UpdateDateTime" /></xsl:call-template>
                  </td>
               </tr>
            </table>
         </body>
      </html>
   </xsl:template>
   <xsl:template match="Slots/SlotData">
      <tr>
         <td width="5" class="StatusCol">
            <xsl:attribute name="bgcolor">
               <xsl:value-of select="GridData/StatusColor"/>
            </xsl:attribute>
         </td>
         <td width="10" class="RightCol">
            <xsl:value-of select="GridData/PercentComplete"/>%
         </td>
         <td width="40" class="RightCol">
            <xsl:value-of select="GridData/Name"/>
         </td>
         <td width="30" class="RightCol">
            <xsl:value-of select="GridData/PPD"/>
         </td>
      </tr>
   </xsl:template>
</xsl:stylesheet>
