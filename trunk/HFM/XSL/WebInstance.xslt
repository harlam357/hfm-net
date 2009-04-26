<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="Instance">
		<html>
			<head>
				<title>Folding Data for Host</title>
				<link rel="stylesheet" type="text/css" href="$CSSFILE" />
			</head>
			<body>
				<table class="Instance">
					<tr>
						<td class="Heading"><xsl:value-of select="@Name"/></td>
            <td class="Plain" colspan="2" align="right">
              <a href="index.html">Overview Page</a>
              <br />
              <a href="summary.html">Summary Page</a>
            </td>
          </tr>
					<tr>
						<td class="LeftCol">Protein</td>
						<td class="RightCol">
							<xsl:value-of select="Protein/WorkUnit"/>
						</td>
					</tr>
					<xsl:apply-templates select="UnitInfo" />
					<tr>
						<td class="Empty"><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text></td>
						<td class="Empty"><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text></td>
					</tr>
					<tr>
						<td class="Heading">
							<xsl:value-of select="Protein/WorkUnit"/>
						</td>
						<td class="Blank"> </td>
					</tr>
					<xsl:apply-templates select="Protein" />
					<tr>
						<td class="Empty"><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text></td>
						<td class="Empty"><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text></td>
					</tr>
					<tr>
						<td class="Plain" colspan="2" align="center">
							Page rendered on <xsl:value-of select="LastUpdatedDate"/>
							at <xsl:value-of select="LastUpdatedTime"/>
						</td>
					</tr>
					<tr>
						<td class="Plain" colspan="2" align="center">
							Data last updated on <xsl:value-of select="LastRetrievedDate"/>
							at <xsl:value-of select="LastRetrievedTime"/>
						</td>
					</tr>
				</table>
			</body>
		</html>
	</xsl:template>
	<xsl:template match="UnitInfo">
		<tr>
			<td class="AltLeftCol">Date<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Started</td>
			<td class="AltRightCol">
				<xsl:value-of select="DateStarted"/>
			</td>
		</tr>
		<tr>
			<td class="LeftCol">Current<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Progress</td>
			<td class="RightCol">
				<xsl:value-of select="FramesComplete"/> frames complete (<xsl:value-of select="PercentComplete"/>%)
		</td>
		</tr>
		<tr>
			<td class="AltLeftCol">Time<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Per<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Frame</td>
			<td class="AltRightCol">
				<xsl:value-of select="TimePerFrame"/>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="Protein">
<!--		<tr>
			<td class="LeftCol">Work Unit</td>
			<td class="RightCol">
				<xsl:value-of select="WorkUnit"/>
			</td>
		</tr> -->
		<tr>
			<td class="LeftCol">Project<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Number</td>
			<td class="RightCol">
				<xsl:value-of select="ProjectNumber"/>
			</td>
		</tr>
		<tr>
			<td class="AltLeftCol">Server<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Address</td>
			<td class="AltRightCol">
				<xsl:value-of select="ServerIP"/>
			</td>
		</tr>
		<tr>
			<td class="LeftCol">Number<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>of<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Atoms</td>
			<td class="RightCol">
				<xsl:value-of select="NumAtoms"/>
			</td>
		</tr>
		<tr>
			<td class="AltLeftCol">Preferred<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Deadline</td>
			<td class="AltRightCol">
				<xsl:value-of select="PreferredDays"/> days
			</td>
		</tr>
		<tr>
			<td class="LeftCol">Final<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Deadline</td>
			<td class="RightCol">
				<xsl:value-of select="MaxDays"/> days
			</td>
		</tr>
		<tr>
			<td class="AltLeftCol">Points<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Credit</td>
			<td class="AltRightCol">
				<xsl:value-of select="Credit"/>
			</td>
		</tr>
		<tr>
			<td class="LeftCol">Number<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>of<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Frames</td>
			<td class="RightCol">
				<xsl:value-of select="Frames"/>
			</td>
		</tr>
		<tr>
			<td class="AltLeftCol">Core</td>
			<td class="AltRightCol">
				<xsl:value-of select="Core"/>
			</td>
		</tr>
		<tr>
			<td class="LeftCol">Contact<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>Person</td>
			<td class="RightCol">
				<xsl:value-of select="Contact"/>
			</td>
		</tr>
		<tr>
			<td class="AltLeftCol">Description</td>
      <td class="AltRightCol">
        <xsl:value-of select="Description" disable-output-escaping="yes"/>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>