@echo off
SET src=%1
rem IF NOT EXIST %src% SET src=%CD%\%1

@echo Executing power shell script %src%

C:\Windows\system32\WindowsPowerShell\v1.0\powershell.exe -Command "cd 'C:\'; Get-ChildItem 'C:\Program Files\Microsoft SDKs\Windows Azure\PowerShell\*.psd1' | ForEach-Object {Import-Module $_}; %src%"












