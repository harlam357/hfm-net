<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" encoding="utf-8" />
  <xsl:include href="FormatDate.xslt"/>
  <xsl:template match="SlotSummary">
    <xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
    <html>
      <head>
        <title>Folding Client Summary</title>
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
        <meta http-equiv="Pragma" content="no-cache" />
        <meta http-equiv="Cache-Control" content="no-cache" />
        <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/css/bootstrap.min.css" integrity="sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh" crossorigin="anonymous" />
        <link rel="stylesheet" href="HFM.css" />
        <link rel="stylesheet" href="$CSSFILE" />
      </head>
      <body>
        <div class="container-fluid">
          <div class="row my-3">
            <div class="col">
              <span class="h2">Summary</span>
                <xsl:text> </xsl:text>
                <a href="index.html">
                  <span style="white-space: nowrap;">Overview Page</span>
                </a>
            </div>
          </div>
        </div>
        <div class="table-responsive">
          <table class="table">
            <thead>
              <tr class="table-heading">
                <th>Status</th>
                <th>Progress</th>
                <th>Name</th>
                <th>Slot Type</th>
                <th>Processor</th>
                <th>TPF</th>
                <th>PPD</th>
                <th>ETA</th>
                <th>Core</th>
                <th>Project (Run, Clone, Gen)</th>
                <th>Credit</th>
                <th>Completed</th>
                <th>Failed</th>
                <th>Username</th>
                <th>Assigned</th>
                <th>Timeout</th>
              </tr>
            </thead>
            <tbody>
              <xsl:apply-templates select="Slots/SlotData">
                <xsl:with-param name="NumberFormat" select="NumberFormat" />
              </xsl:apply-templates>
            </tbody>
          </table>
        </div>
        <div class="container-fluid">
          <div class="row justify-content-end mt-3">
            <div class="col-auto column">
              <xsl:value-of select="SlotTotals/WorkingSlots"/> Working Slots
            </div>
            <div class="col-auto column">
              <xsl:value-of select="format-number(SlotTotals/PPD, NumberFormat)"/> PPD
            </div>
          </div>
          <div class="row justify-content-center my-3">
            <div class="col-auto">
              Page rendered by <a href="https://github.com/harlam357/hfm-net">HFM.NET</a><xsl:text> </xsl:text><xsl:value-of select="HfmVersion"/> on <xsl:call-template name="FormatDate">
                <xsl:with-param name="dateTime" select="UpdateDateTime" />
              </xsl:call-template>
            </div>
          </div>
        </div>

        <script src="https://code.jquery.com/jquery-3.4.1.slim.min.js" integrity="sha384-J6qa4849blE2+poT4WnyKhv5vZF5SrPo0iEjwBvKU7imGFAV0wwj1yYfoRSJoZ+n" crossorigin="anonymous"></script>
        <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js" integrity="sha384-Q6E9RHvbIyZFJoft+2mJbHaEWldlvI9IOYy5n3zV9zzTtmI3UksdQRVvoxMfooAo" crossorigin="anonymous"></script>
        <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/js/bootstrap.min.js" integrity="sha384-wfSDF2E50Y2D1uUdj0O3uMBJnjuUD4Ih7YwaYd1iqfktj0Uod8GCExl3Og8ifwB6" crossorigin="anonymous"></script>
      </body>
    </html>
  </xsl:template>
  <xsl:template match="Slots/SlotData">
    <xsl:param name="NumberFormat" />
    <tr>
      <td>
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
      <td class="table-column">
        <xsl:value-of select="PercentComplete"/>%
      </td>
      <td class="table-column">
        <xsl:choose>
          <xsl:when test="UserIdIsDuplicate='true'">
            <xsl:attribute name="bgcolor">Orange</xsl:attribute>
          </xsl:when>
        </xsl:choose>
        <a>
          <xsl:attribute name="href">
            <xsl:value-of select="Name"/>.html
          </xsl:attribute>
          <xsl:value-of select="Name"/>
        </a>
      </td>
      <td class="table-column">
        <xsl:value-of select="SlotType"/>
      </td>
      <td class="table-column">
        <xsl:value-of select="Processor"/>
      </td>
      <td class="table-column">
        <xsl:value-of select="TPF"/>
      </td>
      <td class="table-column" style="text-align: right">
        <xsl:value-of select="format-number(PPD, $NumberFormat)"/> (<xsl:value-of select="format-number(UPD, $NumberFormat)"/> WUs)
      </td>
      <td class="table-column">
        <xsl:value-of select="ETA"/>
      </td>
      <td class="table-column">
        <xsl:value-of select="Core"/>
      </td>
      <td class="table-column">
        <xsl:choose>
          <xsl:when test="ProjectIsDuplicate='true'">
            <xsl:attribute name="bgcolor">Orange</xsl:attribute>
          </xsl:when>
        </xsl:choose>
        <xsl:value-of select="ProjectRunCloneGen"/>
      </td>
      <td class="table-column" style="text-align: right">
        <xsl:value-of select="format-number(Credit, $NumberFormat)"/>
      </td>
      <td class="table-column">
        <xsl:value-of select="Completed"/>
      </td>
      <td class="table-column">
        <xsl:value-of select="Failed"/>
      </td>
      <td class="table-column">
        <xsl:choose>
          <xsl:when test="UsernameOk='false'">
            <xsl:attribute name="bgcolor">Orange</xsl:attribute>
          </xsl:when>
        </xsl:choose>
        <xsl:value-of select="Username"/>
      </td>
      <td class="table-column">
        <xsl:value-of select="DownloadTime"/>
      </td>
      <td class="table-column">
        <xsl:value-of select="PreferredDeadline"/>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
