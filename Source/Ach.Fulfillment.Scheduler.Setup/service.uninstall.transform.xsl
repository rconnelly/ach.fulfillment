<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
    xmlns="http://schemas.microsoft.com/wix/2006/wi"
    exclude-result-prefixes="wix">
    
    <xsl:output method="text" version="1.0" encoding="UTF-8" />

    
    <xsl:template match="wix:Wix">
    
<xsl:text>@echo off</xsl:text>

echo Running uninstallation
<Command>
    <Name>msiexec</Name>
    <Arguments>
        <Arg> /lx uninstall.log</Arg>
        <Arg> /uninstall Ach.Fulfillment.Scheduler.msi</Arg>
    </Arguments>
</Command>

    </xsl:template>
  
</xsl:stylesheet>