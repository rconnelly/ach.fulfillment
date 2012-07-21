@echo off
copy /b/y NUL %WINDIR%\06CF2EB6-94E6-4a60-91D8-AB945AE8CF38 >NUL 2>&1
if errorlevel 1 goto:nonadmin
del %WINDIR%\06CF2EB6-94E6-4a60-91D8-AB945AE8CF38 >NUL 2>&1
:admin

set msdeploy="C:\Program Files (x86)\IIS\Microsoft Web Deploy\\msdeploy.exe"

set deploymentTransport=http://${DeploymentServer}/MSDeployAgentService
set packageDest=/M:%deploymentTransport%

if "${DeploymentServer}" EQU "local" (
    set packageDest=
)

set error=0
set errorMessage=

echo Setup Started...................................................

rem    echo        DB Install Started.......................................
rem    Database\Migrator.Console SqlServer "${DatabaseConnection}" Database\Ach.Migrations.dll
rem        if %ERRORLEVEL% NEQ 0 (
rem            set /A error=%error%+1
rem            set errorMessage=%errorMessage% Cannot update database structure.
rem        )
rem    echo        .........................................DB Install Ended

    echo        Site Install Started.....................................
    cmd /C Packages\Ach.Fulfillment.Web.deploy.cmd /Y %packageDest%
        if %ERRORLEVEL% NEQ 0 (
            set /A error=%error%+2
            set errorMessage=%errorMessage% Cannot install Ach.Fulfillment.Web.
        )
    cmd /C Packages\Ach.Fulfillment.Api.deploy.cmd /Y %packageDest%
        if %ERRORLEVEL% NEQ 0 (
            set /A error=%error%+4
            set errorMessage=%errorMessage% Cannot install Ach.Fulfillment.Api.
        )
    echo        .......................................Site Install Ended
    
echo Setup Ended...................................................

if %error% NEQ 0 (
    echo Setup Failure Reason is [%error%]:%errorMessage%
    exit /b %error%
)

goto:end
:nonadmin
    echo Setup Failure Reason is [16384]: You need administration privileges to run this script
    exit /b 16384
:end