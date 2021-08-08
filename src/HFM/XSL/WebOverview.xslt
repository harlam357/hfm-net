<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" encoding="utf-8" />
  <xsl:include href="FormatDate.xslt"/>
  <xsl:template match="SlotSummary">
    <xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
    <html>
      <head>
        <title>Folding Client Overview</title>
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
        <meta http-equiv="Pragma" content="no-cache" />
        <meta http-equiv="Cache-Control" content="no-cache" />
        <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/css/bootstrap.min.css" integrity="sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh" crossorigin="anonymous" />
        <link rel="stylesheet" href="HFM.css" />
        <link rel="stylesheet" href="$CSSFILE" />
      </head>
      <body>
        <xsl:variable name="PPW" select="SlotTotals/PPD * 7"/>
        <xsl:variable name="UPW" select="SlotTotals/UPD * 7"/>
        <div class="container-fluid">
          <div class="row my-3">
            <div class="col">
              <span class="h2">Overview</span>
              <xsl:text> </xsl:text>
              <a href="summary.html">
                <span style="white-space: nowrap;">Summary Page</span>
              </a>
            </div>
          </div>
        </div>
        <div>
          <table class="table w-auto">
            <tbody>
              <tr>
                <th scope="row" class="table-column">
                  Total Slots
                </th>
                <td class="table-column">
                  <xsl:value-of select="SlotTotals/TotalSlots"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column-alt">
                  Working Slots
                </th>
                <td class="table-column-alt">
                  <xsl:value-of select="SlotTotals/WorkingSlots"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column">
                  Idle Slots
                </th>
                <td class="table-column">
                  <xsl:value-of select="SlotTotals/TotalSlots - SlotTotals/WorkingSlots"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column-alt">
                  Total PPD (Points per Day)
                </th>
                <td class="table-column-alt">
                  <xsl:value-of select="format-number(SlotTotals/PPD, NumberFormat)"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column">
                  Total PPW (Points per Week)
                </th>
                <td class="table-column">
                  <xsl:value-of select="format-number($PPW, NumberFormat)"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column-alt">
                  Total UPD (Work Units per Day)
                </th>
                <td class="table-column-alt">
                  <xsl:value-of select="format-number(SlotTotals/UPD, NumberFormat)"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column">
                  Total UPW (Work Units per Week)
                </th>
                <td class="table-column">
                  <xsl:value-of select="format-number($UPW, NumberFormat)"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column-alt">
                  Average PPD (per Working Slot)
                </th>
                <td class="table-column-alt">
                  <xsl:value-of select="format-number(SlotTotals/PPD div SlotTotals/WorkingSlots, NumberFormat)"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column">
                  Average PPW (per Working Slot)
                </th>
                <td class="table-column">
                  <xsl:value-of select="format-number($PPW div SlotTotals/WorkingSlots, NumberFormat)"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column-alt">
                  Average UPD (per Working Slot)
                </th>
                <td class="table-column-alt">
                  <xsl:value-of select="format-number(SlotTotals/UPD div SlotTotals/WorkingSlots, NumberFormat)"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column">
                  Average UPW (per Working Slot)
                </th>
                <td class="table-column">
                  <xsl:value-of select="format-number($UPW div SlotTotals/WorkingSlots, NumberFormat)"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column-alt">Completed Work Units</th>
                <td class="table-column-alt">
                  <xsl:value-of select="SlotTotals/TotalRunCompletedUnits"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column">Failed Work Units</th>
                <td class="table-column">
                  <xsl:value-of select="SlotTotals/TotalRunFailedUnits"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column-alt">Total Completed Work Units</th>
                <td class="table-column-alt">
                  <xsl:value-of select="SlotTotals/TotalCompletedUnits"/>
                </td>
              </tr>
              <tr>
                <th scope="row" class="table-column">Total Failed Work Units</th>
                <td class="table-column">
                  <xsl:value-of select="SlotTotals/TotalFailedUnits"/>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="container-fluid">
          <div class="row my-3">
            <div class="col">
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
</xsl:stylesheet>
