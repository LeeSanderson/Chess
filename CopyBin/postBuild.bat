SET FrameworkSDKDIR=%~1
SET DevEnvDir=%~2
SET OutputPath=%~3
SET ConfigurationName=%~4
SET PublishPath=%~f5
SET DisableDevInstall=%~6

:: The current directory is the directory this file is located in.
ECHO.
ECHO Executing %~nx0...

ECHO FrameworkSDKDIR=%FrameworkSDKDIR%
ECHO DevEnvDir=%DevEnvDir%
ECHO OutputPath=%OutputPath%
ECHO ConfigurationName=%ConfigurationName%
ECHO PublishPath=%PublishPath%
ECHO DisableDevInstall=%DisableDevInstall%
ECHO.

ECHO Removing CopyBin.*...
DEL /F /Q "%OutputPath%CopyBin.*"
IF ERRORLEVEL 1 GOTO exitScript

ECHO Publishing binaries...
XCOPY /E /Y /I "%OutputPath%*" "%PublishPath%"
IF ERRORLEVEL 1 GOTO exitScript

ECHO Creating the 'Reference Assemblies' folder contents...
:: Note, if we're making something a reference assembly, we should
:: also just build it into the common output directory
SET CUTRefAssyFolder=%PublishPath%Reference Assemblies
ECHO CUTRefAssyFolder=%CUTRefAssyFolder%
IF NOT EXIST "%CUTRefAssyFolder%" MKDIR "%CUTRefAssyFolder%"
IF ERRORLEVEL 1 GOTO exitScript
XCOPY /Y /I "%OutputPath%Microsoft.Concurrency.UnitTestingFramework.*" "%CUTRefAssyFolder%\"
IF ERRORLEVEL 1 GOTO exitScript
XCOPY /Y /I "%OutputPath%Microsoft.Concurrency.UnitTesting.Extensions.*" "%CUTRefAssyFolder%\"
IF ERRORLEVEL 1 GOTO exitScript
XCOPY /Y /I "%OutputPath%Microsoft.Concurrency.Taskometer.*" "%CUTRefAssyFolder%\"
IF ERRORLEVEL 1 GOTO exitScript

:: If disabling dev install, then we're done
IF "%DisableDevInstall%"=="true" GOTO :exitScript

regsvr32 /s "..\ManagedChess\external\Microsoft.ExtendedReflection.ClrMonitor.X86.dll"
IF ERRORLEVEL 1 GOTO exitScript

ECHO Registering Reference Assemblies folder in registry...
::For VS2010, reference the following article on adding assemblies to the Add Reference box:
::http://msdn.microsoft.com/en-us/library/wkze6zky.aspx
:: TODO: Detect whether the machine is an x64 machine or not
SET ComponentKeyName=Microsoft Alpaca Reference Assemblies
SET RegKeyName=HKLM\SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\%ComponentKeyName% (dev)
REG ADD "%RegKeyName%" /d "%CUTRefAssyFolder%" /f
IF ERRORLEVEL 1 GOTO exitScript
SET RegKeyName=HKLM\SOFTWARE\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\%ComponentKeyName% (dev)
REG ADD "%RegKeyName%" /d "%CUTRefAssyFolder%" /f
IF ERRORLEVEL 1 GOTO exitScript

ECHO Registering assemblies for the GAC...
"%FrameworkSDKDIR%\bin\NETFX 4.0 Tools\gacutil" /il gacAssembliesToInstall.txt /f
IF ERRORLEVEL 1 GOTO exitScript

:exitScript
EXIT /B %ERRORLEVEL%
