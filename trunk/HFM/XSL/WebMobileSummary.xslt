<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
   <xsl:template match="Overview">
      <html>
         <head>
            <meta http-equiv="Pragma" content="no-cache" />
            <meta http-equiv="Cache-Control" content="no-cache" />
            <title>Mobile System Summary</title>
            <link rel="stylesheet" type="text/css" href="$CSSFILE" />
         </head>
         <body>
            <table class="Overview" width="85">
               <tr>
                  <td class="Heading" colspan="2">Mobile Summary</td>
                  <td class="Plain" colspan="4" align="right">
                     <a href="mobile.html">Overview Page</a>
                  </td>
               </tr>
               <tr>
                  <td class="Heading" width="5"></td>
                  <td class="Heading" width="10">%</td>
                  <td class="Heading" width="40">Name</td>
                  <td class="Heading" width="30">PPD</td>
               </tr>
               <xsl:apply-templates select="Instance" />
               <tr>
                  <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
               </tr>
               <tr>
                  <td class="Plain" colspan="4" align="center">
                     Page rendered by <a href="http://code.google.com/p/hfm-net/">HFM.NET</a> <xsl:value-of select="HFMVersion"/> on <xsl:value-of select="LastUpdatedDate"/>
                     at <xsl:value-of select="LastUpdatedTime"/>
                  </td>
               </tr>
            </table>
         </body>
      </html>
   </xsl:template>
   <xsl:template match="Instance">
      <tr>
         <td width="5" class="StatusCol">
            <xsl:attribute name="bgcolor">
               <xsl:value-of select="StatusColor"/>
            </xsl:attribute>
         </td>
         <td width="10" class="RightCol">
            <xsl:value-of select="PercentComplete"/>%
         </td>
         <td width="40" class="RightCol">
            <xsl:value-of select="Name"/>
         </td>
         <td width="30" class="RightCol">
            <xsl:value-of select="PPD"/>
         </td>
      </tr>
   </xsl:template>
</xsl:stylesheet>
