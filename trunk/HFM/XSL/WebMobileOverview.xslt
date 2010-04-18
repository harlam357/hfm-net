<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
   <xsl:output method="html" encoding="utf-8" doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN" doctype-system="http://www.w3.org/TR/html4/loose.dtd" />
   <xsl:template match="Overview">
      <html>
         <head>
            <title>Folding Client Overview (mobile)</title>
            <meta http-equiv="Pragma" content="no-cache" />
            <meta http-equiv="Cache-Control" content="no-cache" />
            <link rel="stylesheet" type="text/css" href="$CSSFILE" />
         </head>
         <body>
            <table class="Overview" width="85">
               <tr>
                  <td class="Heading" width="60">Overview</td>
                  <td class="Plain" width="25">
                     <a href="mobilesummary.html">
                        Summary<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Page
                     </a>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">
                     Total<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Clients
                  </td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="TotalHosts"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">
                     Working
                  </td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="GoodHosts"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">
                     Non-Working
                  </td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="BadHosts"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">
                     Total<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>PPD
                  </td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="EstPPD"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">
                     Total<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>PPW
                  </td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="EstPPW"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">
                     Total<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>UPD
                  </td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="EstUPD"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">
                     Total<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>UPW
                  </td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="EstUPW"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">
                     Average<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>PPD
                  </td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="AvEstPPD"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">
                     Average<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>PPW
                  </td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="AvEstPPW"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">
                     Average<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>UPD
                  </td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="AvEstUPD"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">
                     Average<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>UPW
                  </td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="AvEstUPW"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">Completed Units</td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="Completed"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol" width="60">Failed Units</td>
                  <td class="RightCol" width="25">
                     <xsl:value-of select="Failed"/>
                  </td>
               </tr>
               <tr>
                  <td class="AltLeftCol" width="60">Total Completed Units</td>
                  <td class="AltRightCol" width="25">
                     <xsl:value-of select="TotalCompleted"/>
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
                     <a href="index.html">
                        Standard<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Version
                     </a>
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
                     Page rendered by <a href="http://code.google.com/p/hfm-net/">HFM.NET</a><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text><xsl:value-of select="HFMVersion"/> on <xsl:value-of select="LastUpdatedDate"/>
                     at <xsl:value-of select="LastUpdatedTime"/>
                  </td>
               </tr>
            </table>
         </body>
      </html>
   </xsl:template>
</xsl:stylesheet>
