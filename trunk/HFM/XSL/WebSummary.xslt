<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
   <xsl:template match="Overview">
      <html>
         <head>
            <title>Folding System Summary</title>
            <link rel="stylesheet" type="text/css" href="$CSSFILE" />
         </head>
         <body>
            <!--Uncomment this line to enable a logo on the Summary Page-->
            <!--<img style="float:left; width:100%; bgcolor:Black; fgcolor:black; margin-right:0px; margin-bottom:0px" src="{photo}" alt="" title="Home" />-->
            <table class="Overview" width="100%">
               <tr>
                  <td class="Heading" colspan="2">Summary</td>
                  <td class="Plain" colspan="7" align="right">
                     <a href="index.html">Overview Page</a>
                  </td>
               </tr>
               <tr>
                  <td class="Heading2">Name</td>
                  <td class="Heading2" align="center">
                     %<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Complete
                  </td>
                  <td class="Heading2" align="center">Credit</td>
                  <td class="Heading2" align="center">PPD</td>
                  <td class="Heading2" align="center">PPW</td>
                  <td class="Heading2" align="center">Completed</td>
                  <td class="Heading2" align="center">Failed</td>
                  <td class="Heading2" align="center">
                     Last<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Updated
                  </td>
                  <td class="Heading2" align="center">
                     Will<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Complete
                  </td>
               </tr>
               <xsl:apply-templates select="Instance" />
               <tr>
                  <td class="Plain" colspan="9" align="center">
                     Page rendered by HFM.NET on <xsl:value-of select="LastUpdatedDate"/>
                     at <xsl:value-of select="LastUpdatedTime"/>
                  </td>
               </tr>
            </table>
         </body>
      </html>
   </xsl:template>
   <xsl:template match="Instance">
      <tr>
         <td width="14%" class="LeftCol">
            <a>
               <xsl:attribute name="href">
                  <xsl:value-of select="Name" />.html
               </xsl:attribute>
               <xsl:value-of select="Name"/>
            </a>
         </td>
         <td width="8%" align="center" class="RightCol">
            <xsl:value-of select="PercentComplete"/><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>%
         </td>
         <td width="6%" align="center" class="RightCol">
            <xsl:value-of select="Credit"/>
         </td>
         <td width="16%" align="center" class="RightCol">
            <xsl:value-of select="PPD"/> (<xsl:value-of select="UPD"/> WUs)
         </td>
         <td width="16%" align="center" class="RightCol">
            <xsl:value-of select="PPW"/> (<xsl:value-of select="UPW"/> WUs)
         </td>
         <td width="8%" align="center" class="RightCol">
            <xsl:value-of select="Completed"/>
         </td>
         <td width="8%" align="center" class="RightCol">
            <xsl:value-of select="Failed"/>
         </td>
         <td width="12%" align="center" class="RightCol">
            <xsl:value-of select="LastUpdate"/>
         </td>
         <td width="12%" align="center" class="RightCol">
            <xsl:value-of select="CompleteTime"/>
         </td>
      </tr>
   </xsl:template>
</xsl:stylesheet>
