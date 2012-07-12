@echo off
set parentfolder=%~dp0
set webscript=%parentfolder%Web\ps_ach_service_update.ps1
set apiscript=%parentfolder%Api\ps_ach_service_update.ps1
set servicescript=%parentfolder%Service\ps_ach_service_update.ps1
ps_execute.cmd "%webscript%"
ps_execute.cmd "%apiscript%"
ps_execute.cmd "%servicescript%"