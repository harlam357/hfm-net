<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns="http://www.w3.org/1999/xhtml"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:user="http://www.tempuri.org/User">
   <xsl:output method="html" encoding="utf-8" doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN" doctype-system="http://www.w3.org/TR/html4/loose.dtd" />
   <xsl:template match="SlotSummary">
      <html>
         <head>
            <title>Folding Client Overview (mobile)</title>
            <meta http-equiv="Pragma" content="no-cache" />
            <meta http-equiv="Cache-Control" content="no-cache" />
            <link rel="stylesheet" type="text/css" href="$CSSFILE" />
         </head>
         <body>
            <xsl:variable name="PPW" select="SlotTotals/PPD * 7"/>
            <xsl:variable name="UPW" select="SlotTotals/UPD * 7"/>
            <table class="Overview" width="85">
               <tr>
                  <td class="Heading" width="60">Overview</td>
                  <td class="Plain" width="25">
                     <a href="mobilesummary.html">
                        Summary Page
                     </a>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">
                     Total Slots
                  </td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="SlotTotals/TotalSlots"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">
                     Working
                  </td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="SlotTotals/WorkingSlots"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">
                     Non-Working
                  </td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="SlotTotals/TotalSlots - SlotTotals/WorkingSlots"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">
                     Total PPD
                  </td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="user:FormatNumber(NumberFormat, SlotTotals/PPD)"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">
                     Total PPW
                  </td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="user:FormatNumber(NumberFormat, $PPW)"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">
                     Total UPD
                  </td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="user:FormatNumber(NumberFormat, SlotTotals/UPD)"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">
                     Total UPW
                  </td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="user:FormatNumber(NumberFormat, $UPW)"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">
                     Average PPD
                  </td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="user:FormatNumber(NumberFormat, SlotTotals/PPD div SlotTotals/WorkingSlots)"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">
                     Average PPW
                  </td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="user:FormatNumber(NumberFormat, $PPW div SlotTotals/WorkingSlots)"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">
                     Average UPD
                  </td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="user:FormatNumber(NumberFormat, SlotTotals/UPD div SlotTotals/WorkingSlots)"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">
                     Average UPW
                  </td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="user:FormatNumber(NumberFormat, $UPW div SlotTotals/WorkingSlots)"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">Completed Units</td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="SlotTotals/TotalRunCompletedUnits"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">Failed Units</td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="SlotTotals/TotalRunFailedUnits"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">Total Completed Units</td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="SlotTotals/TotalCompletedUnits"/>
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
                     <a href="index.html">
                        Standard Version
                     </a>
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
                     Page rendered by <a href="http://code.google.com/p/hfm-net/">HFM.NET</a><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text><xsl:value-of select="HfmVersion"/> on <xsl:value-of select="user:FormatDate(UpdateDateTime)"/>
                  </td>
               </tr>
            </table>
         </body>
      </html>
   </xsl:template>

   <msxsl:script implements-prefix="user" language="C#">
      <![CDATA[
         public string FormatNumber(string format, string value)
         {
            decimal decimalValue;
            if (Decimal.TryParse(value, out decimalValue))
            {
               return decimalValue.ToString(format);
            }
            return String.Empty;
         }

         public string FormatDate(string dateValue)
         {
            DateTime value = DateTime.Parse(dateValue);
            return String.Format("{0} at {1}", value.ToLongDateString(), value.ToString("h:mm:ss tt zzz"));
         }
      ]]>
   </msxsl:script>
</xsl:stylesheet>
