@IF "%1" == "" (
	@ECHO Invalid usage. Drag and drop a cmd file onto this one to run the cmd
	@ECHO file in the Chess tool development environment.
	@PAUSE
	@GOTO :EOF
)

:: Sets up the environment to use the MCHESS from the development bin folder
:: rather than the install folder.

:: From Chess\dev\main\scripts\env.cmd
:: Unfortunately, no way to know on a per-developer bases where the their
:: source depot copy is.

@ECHO Setting up development environment...
::SET CHESSHOME=C:\Chess\dev\main
@SET CHESSHOME=%~dp0..
@SET PATH=%CHESSHOME%\bin;%PATH%
@SET MCHESS_PATH=%CHESSHOME%\bin

:: Allow cmd files to know when they're in the dev environment and to allow them to run
:: against the Debug builds
@SET ChessConfig=Debug

:: Make current directory the path of the input cmd file
@CD /D %~dp1

:: And run the command
@ECHO Running input command...
%~nx1
IF ERRORLEVEL 1 PAUSE

@PAUSE