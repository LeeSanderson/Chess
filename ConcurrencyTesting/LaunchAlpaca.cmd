@ECHO OFF

:: This should no-longer be needed
:: Setup the VS command line vars so building using msbuild will work
::SET VCVarsBat="C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat"
::CALL %VCVarsBat% x86
::IF ERRORLEVEL 1 PAUSE

ECHO Starting Alpaca...
::alpaca /import AllTests.xml
alpaca
IF ERRORLEVEL 1 PAUSE

