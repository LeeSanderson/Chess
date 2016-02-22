@ECHO OFF
@IF "%ChessConfig%" == "" @SET ChessConfig=Release

:: Make sure the folder is created and empty
@ECHO Setting up temp folder...
@SET TempFldrName=RegressionTests
@IF EXIST %TempFldrName%\ (
	::@ECHO Removing existing temp folder: %TempFldrName%
	:RMDIR_Tmp
	RMDIR %TempFldrName% /S /Q
	@IF EXIST %TempFldrName%\ GOTO RMDIR_Tmp
)
@IF NOT EXIST %TempFldrName%\ (
	::@ECHO Creating temp folder: %TempFldrName%
	:MD_Tmp
	MD %TempFldrName%
	@IF NOT EXIST %TempFldrName%\ GOTO MD_Tmp
)
@ECHO Temp folder setup complete.
@CD %TempFldrName%

@ECHO ON
@ECHO.
@ECHO Starting MCut...
mcut runAllTests ..\..\ConcurrencyTools\RegressionTesting\MCUT.Framework\bin\%ChessConfig%\MCUT.Framework.RegressionTests.dll
::mcut runAllTests ..\..\ConcurrencyTools\RegressionTesting\RegressionTesting.TestList.%ChessConfig%.xml
::@IF NOT ERRORLEVEL 0 PAUSE

@PAUSE