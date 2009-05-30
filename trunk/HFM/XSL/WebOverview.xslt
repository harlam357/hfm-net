<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
   <xsl:template match="Overview">
      <html>
         <head>
            <title>Folding System Overview</title>
            <link rel="stylesheet" type="text/css" href="$CSSFILE" />
         </head>
         <body>
            <!--Uncomment this line to enable a logo on the Overview Page-->
            <!--<img style="float:left; margin-right:0px; margin-bottom:0px" src="{photo}" alt="" title="Home" />-->
            <table class="Overview" width="50%">
               <tr>
                  <td class="Heading">Overview</td>
                  <td class="Plain">
                     <a href="summary.html">
                        Summary<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Page
                     </a>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Total<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Folding<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Clients
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="TotalHosts"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Working<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Clients
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="GoodHosts"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Non-Working<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Clients
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="BadHosts"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Total<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>PPD<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>(Working<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Clients)
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="EstPPD"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Total<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>PPW<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>(Working<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Clients)
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="EstPPW"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Total<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>UPD<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>(Working<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Clients)
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="EstUPD"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Total<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>UPW<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>(Working<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Clients)
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="EstUPW"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Average<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>PPD<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>(Working<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Clients)
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="AvEstPPD"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Average<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>PPW<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>(Working<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Clients)
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="AvEstPPW"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Average<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>UPD<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>(Working<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Clients)
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="AvEstUPD"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">
                     Average<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>UPW<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>(Working<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Clients)
                  </td>
                  <td class="RightCol">
                     <xsl:value-of select="AvEstUPW"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">Completed Units</td>
                  <td class="RightCol">
                     <xsl:value-of select="TotalCompleted"/>
                  </td>
               </tr>
               <tr>
                  <td class="LeftCol">Failed Units</td>
                  <td class="RightCol">
                     <xsl:value-of select="TotalFailed"/>
                  </td>
               </tr>
               <tr>
                  <td class="Plain" colspan="2" align="center">
                     Page rendered by HFM.NET on <xsl:value-of select="LastUpdatedDate"/>
                     at <xsl:value-of select="LastUpdatedTime"/>
                  </td>
               </tr>
            </table>
         </body>
      </html>
   </xsl:template>
</xsl:stylesheet>
