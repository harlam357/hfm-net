<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
   <xsl:output method="html" encoding="utf-8" doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN" doctype-system="http://www.w3.org/TR/html4/loose.dtd" />
   <xsl:template match="Overview">
      <html>
         <head>
            <title>Folding Client Summary</title>
            <meta http-equiv="Pragma" content="no-cache" />
            <meta http-equiv="Cache-Control" content="no-cache" />
            <link rel="stylesheet" type="text/css" href="$CSSFILE" />
         </head>
         <body>
            <!--Uncomment this line to enable a logo on the Summary Page-->
            <!--<img style="float:left; width:100%; bgcolor:Black; fgcolor:black; margin-right:0px; margin-bottom:0px" src="{photo}" alt="" title="Home" />-->
            <table class="Overview" width="100%">
               <tr>
                  <td class="Heading" colspan="2">Summary</td>
                  <td class="Plain" colspan="16" align="right">
                     <a href="index.html">Overview Page</a>
                  </td>
               </tr>
               <tr>
                  <td class="Heading">Status</td>
                  <td class="Heading">Progress</td>
                  <td class="Heading">Name</td>
                  <td class="Heading">Client<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Type</td>
                  <td class="Heading">TPF</td>
                  <td class="Heading">PPD</td>
                  <td class="Heading">MHz</td>
                  <td class="Heading">PPD/MHz</td>
                  <td class="Heading">ETA</td>
                  <td class="Heading">Core</td>
                  <td class="Heading">Version</td>
                  <td class="Heading">Project<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>(Run,<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Clone,<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Gen)</td>
                  <td class="Heading">Credit</td>
                  <td class="Heading">Completed</td>
                  <td class="Heading">Failed</td>
                  <td class="Heading">User<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Name</td>
                  <td class="Heading">Download<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Time</td>
                  <td class="Heading">Deadline</td>
               </tr>
               <xsl:apply-templates select="Instance" />
               <tr>
                  <td class="Plain" colspan="18" align="center">
                     Page rendered by <a href="http://code.google.com/p/hfm-net/">HFM.NET</a><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text><xsl:value-of select="HFMVersion"/> on <xsl:value-of select="LastUpdatedDate"/>
                     at <xsl:value-of select="LastUpdatedTime"/>
                  </td>
               </tr>
            </table>
         </body>
      </html>
   </xsl:template>
   <xsl:template match="Instance">
      <tr>
         <td width="7%" class="StatusCol">
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
         <td width="14%">
            <xsl:choose>
               <xsl:when test="UserIDDuplicate='True'">
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
         <td width="4%" class="RightCol">
            <xsl:value-of select="ClientType"/>
         </td>
         <td width="4%" class="RightCol">
            <xsl:value-of select="TPF"/>
         </td>
         <td width="8%" class="RightCol"> <!--40%-->
            <xsl:value-of select="PPD"/> (<xsl:value-of select="UPD"/> WUs)
         </td>
         <td width="3%" class="RightCol">
            <xsl:value-of select="MHz"/>
         </td>
         <td width="3%" class="RightCol">
            <xsl:value-of select="PPDMHz"/>
         </td>
         <td width="4%" class="RightCol">
            <xsl:value-of select="ETA"/>
         </td>
         <td width="4%" class="RightCol">
            <xsl:value-of select="Core"/>
         </td>
         <td width="3%" class="RightCol">
            <xsl:value-of select="CoreVersion"/>
         </td>
         <td width="8%"> <!--65%-->
            <xsl:choose>
               <xsl:when test="ProjectDuplicate='True'">
                  <xsl:attribute name="class">StatusCol</xsl:attribute>
                  <xsl:attribute name="bgcolor">Orange</xsl:attribute>
               </xsl:when>
               <xsl:otherwise>
                  <xsl:attribute name="class">RightCol</xsl:attribute>
               </xsl:otherwise>
            </xsl:choose>
            <xsl:value-of select="ProjectRunCloneGen"/>
         </td>
         <td width="4%" class="RightCol">
            <xsl:value-of select="Credit"/>
         </td>
         <td width="4%" class="RightCol">
            <xsl:value-of select="Completed"/>
         </td>
         <td width="3%" class="RightCol">
            <xsl:value-of select="Failed"/>
         </td>
         <td width="8%">
            <xsl:choose>
               <xsl:when test="UsernameMatch='False'">
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
            <xsl:value-of select="Deadline"/>
         </td>
      </tr>
   </xsl:template>
</xsl:stylesheet>
