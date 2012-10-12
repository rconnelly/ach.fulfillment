<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
    xmlns="http://schemas.microsoft.com/wix/2006/wi"
    exclude-result-prefixes="wix">
    
    <xsl:output method="text" version="1.0" encoding="UTF-8" />

    
    <xsl:template match="wix:Wix">
    
<xsl:text>@echo off</xsl:text>

echo Setting defaults
SET INSTALLLOCATION=C:\Pps\Ach.Fulfillment.Scheduler
<xsl:for-each select="//wix:Product/wix:Property">
SET <xsl:value-of select="./@Id"/>=<xsl:value-of select="./@Value"/>
  
</xsl:for-each> 

echo Running installation
<Command>
    <Name>msiexec</Name>
    <Arguments>
        <Arg> /lx install.log</Arg>
        <Arg> /i Ach.Fulfillment.Scheduler.msi</Arg>
        <Arg> INSTALLLOCATION="%INSTALLLOCATION%" </Arg>
        <xsl:for-each select="//wix:Product/wix:Property">
        <Arg> <xsl:value-of select="./@Id"/>="%<xsl:value-of select="./@Id"/>%" </Arg>
        </xsl:for-each> 
    </Arguments>
</Command>

    </xsl:template>
  
</xsl:stylesheet>