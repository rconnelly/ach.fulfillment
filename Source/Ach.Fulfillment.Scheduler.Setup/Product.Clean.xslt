<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
    xmlns="http://schemas.microsoft.com/wix/2006/wi"
    exclude-result-prefixes="wix">
  <xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" />

  <xsl:template match="wix:Wix">
    <xsl:variable name="binaries" select="
        wix:Fragment/wix:DirectoryRef/wix:Component[
        substring(wix:File/@Source, string-length(wix:File/@Source) - 2) = 'dll' 
        or substring(wix:File/@Source, string-length(wix:File/@Source) - 5) = 'config'
        ]"/>
    <xsl:variable name="subdirectories" select="wix:Fragment/wix:DirectoryRef/wix:Directory"/>
    <Wix>
        <Fragment>
            <DirectoryRef Id="{wix:Fragment/wix:DirectoryRef/@Id}">
                <xsl:apply-templates select="$binaries"/>
                <xsl:apply-templates select="$subdirectories"/>
            </DirectoryRef>
        </Fragment>
        <Fragment>
            <ComponentGroup Id="{wix:Fragment/wix:ComponentGroup/@Id}">
                <xsl:for-each select="$binaries">
                    <ComponentRef Id="{@Id}"/>
                </xsl:for-each>
                <xsl:for-each select="$subdirectories//wix:Component">
                    <ComponentRef Id="{@Id}"/>
                </xsl:for-each>
            </ComponentGroup>
        </Fragment>
    </Wix>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>
  
</xsl:stylesheet>