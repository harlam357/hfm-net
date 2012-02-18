<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns="http://www.w3.org/1999/xhtml"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:user="http://www.tempuri.org/User">
   <xsl:output method="html" encoding="utf-8" doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN" doctype-system="http://www.w3.org/TR/html4/loose.dtd" />
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
               <xsl:apply-templates select="Slots/SlotData" />
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
                     <xsl:value-of select="user:FormatNumber(NumberFormat, SlotTotals/PPD)"/><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>PPD
                  </td>
                  <td class="Plain" colspan="16"></td>
               </tr>
               <tr>
                  <td class="Plain" colspan="18" align="center">
                     <a href="mobilesummary.html">
                        Mobile Version
                     </a>
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="18" align="center">
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="18" align="center">
                     Page rendered by <a href="http://code.google.com/p/hfm-net/">HFM.NET</a><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text><xsl:value-of select="HfmVersion"/> on <xsl:value-of select="user:FormatDate(UpdateDateTime)"/>
                  </td>
               </tr>
            </table>
         </body>
      </html>
   </xsl:template>
   <xsl:template match="Slots/SlotData">
      <tr>
         <td width="5%" class="StatusCol">
            <xsl:attribute name="bgcolor">
               <xsl:value-of select="GridData/StatusColor"/>
            </xsl:attribute>
            <font>
               <xsl:attribute name="color">
                  <xsl:value-of select="GridData/StatusFontColor"/>
               </xsl:attribute>
               <xsl:value-of select="GridData/Status"/>
            </font>
         </td>
         <td width="3%" class="RightCol">
            <xsl:value-of select="GridData/PercentComplete"/>%
         </td>
         <td width="15%">
            <xsl:choose>
               <xsl:when test="GridData/UserIdIsDuplicate='true'">
                  <xsl:attribute name="class">StatusCol</xsl:attribute>
                  <xsl:attribute name="bgcolor">Orange</xsl:attribute>
               </xsl:when>
               <xsl:otherwise>
                  <xsl:attribute name="class">RightCol</xsl:attribute>
               </xsl:otherwise>
            </xsl:choose>
            <a>
               <xsl:attribute name="href"><xsl:value-of select="GridData/Name"/>.html</xsl:attribute><xsl:value-of select="GridData/Name"/>
            </a>
         </td>
         <td width="5%" class="RightCol">
            <xsl:value-of select="GridData/SlotType"/>
         </td>
         <td width="5%" class="RightCol">
            <xsl:value-of select="GridData/TPF"/>
         </td>
         <td width="8%" class="RightCol">
            <xsl:value-of select="GridData/PPD"/> (<xsl:value-of select="GridData/UPD"/> WUs)
         </td>
         <td width="8%" class="RightCol">
            <xsl:value-of select="GridData/ETA"/>
         </td>
         <td width="8%" class="RightCol">
            <xsl:value-of select="GridData/Core"/>
         </td>
         <td width="2%" class="RightCol">
            <xsl:value-of select="GridData/CoreId"/>
         </td>
         <td width="8%">
            <xsl:choose>
               <xsl:when test="GridData/ProjectIsDuplicate='true'">
                  <xsl:attribute name="class">StatusCol</xsl:attribute>
                  <xsl:attribute name="bgcolor">Orange</xsl:attribute>
               </xsl:when>
               <xsl:otherwise>
                  <xsl:attribute name="class">RightCol</xsl:attribute>
               </xsl:otherwise>
            </xsl:choose>
            <xsl:value-of select="GridData/ProjectRunCloneGen"/>
         </td>
         <td width="3%" class="RightCol">
            <xsl:value-of select="GridData/Credit"/>
         </td>
         <td width="3%" class="RightCol">
            <xsl:value-of select="GridData/Completed"/>
         </td>
         <td width="3%" class="RightCol">
            <xsl:value-of select="GridData/Failed"/>
         </td>
         <td width="8%">
            <xsl:choose>
               <xsl:when test="GridData/UsernameOk='false'">
                  <xsl:attribute name="class">StatusCol</xsl:attribute>
                  <xsl:attribute name="bgcolor">Orange</xsl:attribute>
               </xsl:when>
               <xsl:otherwise>
                  <xsl:attribute name="class">RightCol</xsl:attribute>
               </xsl:otherwise>
            </xsl:choose>
            <xsl:value-of select="GridData/Username"/>
         </td>
         <td width="8%" class="RightCol">
            <xsl:value-of select="GridData/DownloadTime"/>
         </td>
         <td width="8%" class="RightCol"> <!--100%-->
            <xsl:value-of select="GridData/PreferredDeadline"/>
         </td>
      </tr>
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
