@ECHO Setting up chess paths for development.
@SET CHESSHOME=%~dp0..
@SET PATH=%CHESSHOME%\bin;%CHESSHOME%\..\..\external\perl.v5.8.0\bin;%PATH%
@SET MCHESS_PATH=%CHESSHOME%\bin
@ECHO.
:: Now, with the modified settings, open a new cmd window
:: without it being forced closed.
@CMD /k
