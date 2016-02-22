set PATH=%PATH%;..\..\..\external\perl.v5.8.0\bin
copy /y cmdfile cmdfile.bak
copy /y exefile exefile.bak
xcopy /r /y cmdreleasefile cmdfile
xcopy /r /y exereleasefile exefile
rem get rid of DLLs
perl ..\Test\regression.pl -nog nmake:clean exefile
rem make DLLs and copy them over
perl ..\Test\regression.pl -nog nmake:install exefile
rem create the build script
perl ..\Test\regression.pl -p nmake:build exefile > ..\Installer\Tests\Managed\build.cmd
rem create the test script
perl ..\Test\regression.pl -p cmdfile exefile > ..\Installer\Tests\Managed\test.cmd
rem copy over the expected results
mkdir ..\Installer\Tests\Managed\golden
perl ..\Test\regression.pl -outputtags cmdfile exefile > tags.out
perl ..\Test\copygolden.pl tags.out ..\Installer\Tests\Managed\
copy /y cmdfile.bak cmdfile
rem copy over the unittest solution
rem rmdir /s /q UnitTest\UnitTest\bin UnitTest\UnitTest\obj
rem xcopy /r /y /s /i UnitTest ..\Installer\Tests\Managed\UnitTest