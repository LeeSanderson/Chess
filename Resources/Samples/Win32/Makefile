CLIB=/MDd
CFLAGS=/nologo /EHsc /Zi /Oi $(CLIB) /Gm- /W4 
LIBFLAGS=/nologo
CFLAGS=$(CFLAGS) /Gs  /D_X86_
LIBFLAGS=$(LIBFLAGS) /debug
DLLENTRY=_DllMainCRTStartup@12
LINKFLAGS=$(LINKFLAGS) /machine:x86 /debug /debugtype:cv,fixup

.SUFFIXES: .cpp  .dll

.cpp.dll:
	cl /LD $(CFLAGS) /Fe$@ /Fd$*.pdb $< \
		/link $(LINKFLAGS) /release /incremental:no /subsystem:console
	mt.exe -manifest $@.manifest -outputresource:$@;2


.cpp.exe:
	cl $(CFLAGS) /Fe$@ /Fd$*.pdb $< \
		/link $(LINKFLAGS) /release /incremental:no /subsystem:console
	mt.exe -manifest $@.manifest -outputresource:$@;1


##############################################################################

clean:
	-del *~ *.manifest *.dll *.exe *.obj *.lib *.exp *.pdb  $(DLLS) 2>nul
