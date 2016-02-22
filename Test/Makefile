PERL=perl.exe 

LIBFLAGS=/nologo
CFLAGS=/nologo /EHsc /Zi /Oi $(CLIB) /Gm- /W4 /Gs /D_X86_
DLLENTRY=_DllMainCRTStartup@12

! if "$(BUILDDEBUG)"=="1"
CLIB=/MDd
LIBFLAGS=$(LIBFLAGS) /debug
LINKFLAGS=$(LINKFLAGS) /machine:x86 /debugtype:cv,fixup /debug
! else
CLIB=/MD
LINKFLAGS=$(LINKFLAGS) /machine:x86 /release 
! endif

runtestloader.dll: runtestloader.cpp twothreads.dll
! if "$(INSTALLTEST)"=="1"
	copy /y $*.cpp ..\Installer\Tests\Win32
! endif
	cl /LD $(CFLAGS) runtestloader.cpp twothreads.lib

.SUFFIXES: .cpp  .dll

.cpp.dll:
! if "$(INSTALLTEST)"=="1"
	copy /y $*.cpp ..\Installer\Tests\Win32
! endif
	cl /LD $(CFLAGS) /Fe$@ /Fd$*.pdb $< \
		/link $(LINKFLAGS) /incremental:no /subsystem:console \
		/entry:$(DLLENTRY)
	mt.exe -manifest $@.manifest -outputresource:$@;2


vistabuild: 
	$(PERL) regression.pl -nog nmake:build dllfile.vista

build: 
	$(PERL) regression.pl -nog nmake:build dllfile

cleanbuild: 
	$(PERL) regression.pl -nog nmake dllfile dllfile.vista

regress:
	$(PERL) regression.pl cmdfile dllfile

vistaregress:
	$(PERL) regression.pl cmdfile dllfile.vista

golden:
	$(PERL) regression.pl -g cmdfile dllfile

vistagolden:
	$(PERL) regression.pl -g cmdfile dllfile.vista

all: build vistabuild regress vistaregress

##############################################################################

clean:
	-del *~ *.obj *.lib *.exp *.pdb *.dll 2>nul
