<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" encoding="utf-8" />
  <xsl:include href="FormatDate.xslt"/>
  <xsl:template match="SlotDetail">
    <xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
    <html>
      <head>
        <title>Slot Data</title>
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
              <span class="h2">
                <xsl:value-of select="SlotData/Name"/>
              </span>
              <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
              <a href="index.html">Overview Page</a>
              <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>/<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
              <a href="summary.html">Summary Page</a>
            </div>
          </div>
        </div>
        <div>
          <table class="table slot-table">
            <tr>
              <th scope="row" class="table-column-alt">
                Current Progress
              </th>
              <td class="table-column-alt">
                <xsl:value-of select="SlotData/PercentComplete"/>% Complete
              </td>
            </tr>
            <tr>
              <th scope="row" class="table-column">
                TPF (Time per Frame)
              </th>
              <td class="table-column">
                <xsl:value-of select="SlotData/TPF"/>
              </td>
            </tr>
            <tr>
              <th scope="row" class="table-column-alt">
                PPD (Points per Day)
              </th>
              <td class="table-column-alt">
                <xsl:value-of select="format-number(SlotData/PPD, NumberFormat)"/> (<xsl:value-of select="format-number(SlotData/UPD, NumberFormat)"/> WUs)
              </td>
            </tr>
            <tr>
              <th scope="row" class="table-column">
                PPW (Points per Week)
              </th>
              <td class="table-column">
                <xsl:value-of select="format-number(SlotData/PPD * 7, NumberFormat)"/> (<xsl:value-of select="format-number(SlotData/UPD * 7, NumberFormat)"/> WUs)
              </td>
            </tr>
            <tr>
              <th scope="row" class="table-column-alt">
                Work Unit Statistics for this Slot
              </th>
              <td class="table-column-alt">
                Completed: <xsl:value-of select="TotalRunCompletedUnits"/> - Failed: <xsl:value-of select="TotalRunFailedUnits"/> - Total Completed: <xsl:value-of select="TotalCompletedUnits"/> - Total Failed: <xsl:value-of select="TotalFailedUnits"/>
              </td>
            </tr>
            <tr>
              <th scope="row" class="table-column">
                Assigned
              </th>
              <td class="table-column">
                <xsl:value-of select="SlotData/DownloadTime"/>
              </td>
            </tr>
            <tr>
              <th scope="row" class="table-column-alt">
                ETA (Estimated Time of Arrival / Completion)
              </th>
              <td class="table-column-alt">
                <xsl:value-of select="SlotData/ETA"/>
              </td>
            </tr>
          </table>
        </div>
        <div class="container-fluid">
          <div class="row my-3">
            <div class="col">
              <span class="h4">
                Log File
              </span>
              <xsl:choose>
                <xsl:when test="LogFileAvailable='true' and LogFileName!=''">
                  <a>
                    <xsl:attribute name="href">
                      <xsl:value-of select="LogFileName"/>
                    </xsl:attribute>Click here to view the client log file
                  </a>
                </xsl:when>
              </xsl:choose>
            </div>
          </div>
        </div>
        <table class="table log-table">
          <tr>
            <td class="table-column" colspan="2">
              <xsl:for-each select="SlotData/CurrentLogLines/LogLine">
                <xsl:value-of select="Raw" disable-output-escaping="yes"/>
                <xsl:element name="br"/>
              </xsl:for-each>
            </td>
          </tr>
        </table>
        <div class="container-fluid">
          <div class="row my-3">
            <div class="col">
              <span class="h4">
                Project
              </span>
            </div>
          </div>
        </div>
        <table class="table project-table">
          <xsl:apply-templates select="SlotData/Protein" />
        </table>
        <div class="container-fluid">
          <div class="row">
            <div class="col">
              Page rendered by <a href="https://github.com/harlam357/hfm-net">HFM.NET</a><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text><xsl:value-of select="HfmVersion"/> on <xsl:call-template name="FormatDate">
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
  <xsl:template match="SlotData/Protein">
    <tr>
      <th scope="row" class="table-column-alt">Project ID</th>
      <td class="table-column-alt">
        <xsl:value-of select="ProjectNumber"/>
      </td>
    </tr>
    <tr>
      <th scope="row" class="table-column">Work Unit</th>
      <td class="table-column">
        <xsl:value-of select="WorkUnitName"/>
      </td>
    </tr>
    <tr>
      <th scope="row" class="table-column-alt">Base Credit</th>
      <td class="table-column-alt">
        <xsl:value-of select="Credit"/>
      </td>
    </tr>
    <tr>
      <th scope="row" class="table-column">KFactor</th>
      <td class="table-column">
        <xsl:value-of select="KFactor"/>
      </td>
    </tr>
    <tr>
      <th scope="row" class="table-column-alt">Number of Frames</th>
      <td class="table-column-alt">
        <xsl:value-of select="Frames"/>
      </td>
    </tr>
    <tr>
      <th scope="row" class="table-column">Core</th>
      <td class="table-column">
        <xsl:value-of select="Core"/>
      </td>
    </tr>
    <tr>
      <th scope="row" class="table-column-alt">Number of Atoms</th>
      <td class="table-column-alt">
        <xsl:value-of select="NumberOfAtoms"/>
      </td>
    </tr>
    <tr>
      <th scope="row" class="table-column">Timeout</th>
      <td class="table-column">
        <xsl:value-of select="PreferredDays"/> Days
      </td>
    </tr>
    <tr>
      <th scope="row" class="table-column-alt">Expiration</th>
      <td class="table-column-alt">
        <xsl:value-of select="MaximumDays"/> Days
      </td>
    </tr>
    <tr>
      <th scope="row" class="table-column">Contact</th>
      <td class="table-column">
        <xsl:value-of select="Contact"/>
      </td>
    </tr>
    <tr>
      <th scope="row" class="table-column-alt">Work Server</th>
      <td class="table-column-alt">
        <xsl:value-of select="ServerIP"/>
      </td>
    </tr>
    <tr>
      <th scope="row" class="table-column">Description</th>
      <td class="table-column">
        <xsl:value-of select="Description" disable-output-escaping="yes"/>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
