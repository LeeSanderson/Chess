SET FrameworkSDKDIR=%~1
SET DevEnvDir=%~2
SET OutputPath=%~3
SET DisableDevInstall=%~4

:: The current directory is the directory this file is located in.
ECHO.
ECHO Executing %~nx0...

ECHO FrameworkSDKDIR=%FrameworkSDKDIR%
ECHO DevEnvDir=%DevEnvDir%
ECHO OutputPath=%OutputPath%
ECHO DisableDevInstall=%DisableDevInstall%
ECHO.

:: If disabling dev install, then we're done
IF "%DisableDevInstall%"=="true" GOTO :end

"%FrameworkSDKDIR%\bin\NETFX 4.0 Tools\gacutil" /ul gacAssembliesToUninstall.txt

regsvr32 /s /u "%OutputPath%Microsoft.ExtendedReflection.ClrMonitor.X86.dll"

:: By calling EXIT w/o specifying an exit code, the exit code will be zero
:end
EXIT /B