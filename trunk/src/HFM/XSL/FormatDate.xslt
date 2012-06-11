<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns="http://www.w3.org/1999/xhtml"
                              xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
   <!-- converts FROM 
        <date>2001-12-31T12:00:00Z</date> OR
        <date>2001-12-31T12:00:00.00000-00:00</date> OR
        <date>2001-12-31T12:00:00.00000+00:00</date> TO 
        some new format (DEFINED below) -->
   <xsl:template name="FormatDate">
      <xsl:param name="dateTime" />

      <xsl:variable name="year" select="substring($dateTime,1,4)" />
      <xsl:variable name="month-temp" select="substring-after($dateTime,'-')" />
      <xsl:variable name="month" select="substring-before($month-temp,'-')" />
      <xsl:variable name="day-temp" select="substring-after($month-temp,'-')" />
      <xsl:variable name="day" select="substring($day-temp,1,2)" />
      <xsl:variable name="time" select="substring-after($dateTime,'T')" />
      <xsl:variable name="hh" select="substring($time,1,2)" />
      <xsl:variable name="mm" select="substring($time,4,2)" />
      <xsl:variable name="ss" select="substring($time,7,2)" />
      <xsl:variable name="tz-temp1" select="substring-after($time,'-')" />
      <xsl:variable name="tz-temp2" select="substring-after($time,'+')" />

      <xsl:choose>
         <xsl:when test="$month = '1' or $month= '01'">January</xsl:when>
         <xsl:when test="$month = '2' or $month= '02'">February</xsl:when>
         <xsl:when test="$month= '3' or $month= '03'">March</xsl:when>
         <xsl:when test="$month= '4' or $month= '04'">April</xsl:when>
         <xsl:when test="$month= '5' or $month= '05'">May</xsl:when>
         <xsl:when test="$month= '6' or $month= '06'">June</xsl:when>
         <xsl:when test="$month= '7' or $month= '07'">July</xsl:when>
         <xsl:when test="$month= '8' or $month= '08'">August</xsl:when>
         <xsl:when test="$month= '9' or $month= '09'">September</xsl:when>
         <xsl:when test="$month= '10'">October</xsl:when>
         <xsl:when test="$month= '11'">November</xsl:when>
         <xsl:when test="$month= '12'">December</xsl:when>
      </xsl:choose>
      <xsl:value-of select="' '"/>
      <!--January -->
      <xsl:value-of select="$day"/>
      <!--January 12 -->
      <xsl:value-of select="','"/>
      <!--January 12,-->
      <xsl:value-of select="' '"/>
      <!--January 12, -->
      <xsl:value-of select="$year"/>
      <!--January 12, 2001-->
      <xsl:value-of select="' at '"/>
      <!--January 12, 2001 at -->
      <xsl:choose>
         <xsl:when test="$hh = 0">
            <xsl:value-of select="'12'"/>
         </xsl:when>
         <xsl:when test="$hh &gt; 12">
            <xsl:value-of select="format-number($hh - 12, 0)"/>
         </xsl:when>
         <xsl:otherwise>
            <xsl:value-of select="format-number($hh, 0)"/>
         </xsl:otherwise>
      </xsl:choose>
      <!--January 12, 2001 at 00-->
      <xsl:value-of select="':'"/>
      <!--January 12, 2001 at 00:-->
      <xsl:value-of select="$mm"/>
      <!--January 12, 2001 at 00:00-->
      <xsl:value-of select="':'"/>
      <!--January 12, 2001 at 00:00:-->
      <xsl:value-of select="$ss"/>
      <!--January 12, 2001 at 00:00:00-->
      <xsl:value-of select="' '"/>
      <!--January 12, 2001 at 00:00:00 -->
      <xsl:choose>
         <xsl:when test="$hh &gt; 12">
            <xsl:value-of select="'PM'"/>
         </xsl:when>
         <xsl:otherwise>
            <xsl:value-of select="'AM'"/>
         </xsl:otherwise>
      </xsl:choose>
      <!--January 12, 2001 at 00:00:00 PM-->
      <xsl:value-of select="' '"/>
      <!--January 12, 2001 at 00:00:00 PM -->
      <xsl:if test="(string-length($tz-temp1) != 0)">
         <xsl:value-of select="'-'"/>
         <xsl:value-of select="$tz-temp1"/>
      </xsl:if>
      <xsl:if test="(string-length($tz-temp2) != 0)">
         <xsl:value-of select="'+'"/>
         <xsl:value-of select="$tz-temp2"/>
      </xsl:if>
   </xsl:template>
</xsl:stylesheet>
