@echo off
c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild ..\Source\Ach.Fulfillment.Migrations\Ach.Fulfillment.Migrations.csproj /nologo /v:m
        if %ERRORLEVEL% NEQ 0 (
            goto:end
        )

PUSHD ..\Source\Ach.Fulfillment.Migrations\bin\
    ECM7.Migrator.Console SqlServer "Data Source=(local);Initial Catalog=Ach;Integrated Security=True" Ach.Fulfillment.Migrations.dll
POPD
:end