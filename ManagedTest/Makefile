PERL=perl
EXES=autoresetevent.exe autoresetevent1.exe autoresetevent2.exe	\
balance.exe boundedqueue.exe boundedqueue2.exe \
choizeller.exe choose.exe deadlock.exe deadlock2.exe \
deadlock3.exe deadlock4.exe eventwaithandle.exe example.exe \
interlock1.exe interlock3.exe last.exe lazyinit.exe monitor.exe monitor2.exe monitor3.exe \
monitor_pulse_wait.exe mutex.exe pfx.tpl.repro1703.exe \
pinvoke.exe pinvoke2.exe pinvoke3.exe preempt1.exe preempt2.exe preempt3.exe \
preempt4.exe preempt5.exe preempt6.exe races.exe readerwriterlock.exe\
semaphore.exe signalandwait.exe signalwait2.exe static.exe template.exe\
threadpool.exe timer.exe waitall.exe waitany.exe

.SUFFIXES: .cs  .exe

NativeMethods.dll: NativeMethods.cs
	csc /debug /unsafe /target:library NativeMethods.cs

choose.exe choose2.exe choose3.exe choose4.exe:
	csc /debug /r:../bin/Microsoft.ManagedChessAPI.dll $*.cs
preempt1.exe preempt2.exe preempt3.exe preempt4.exe preempt5.exe preempt6.exe:
	csc /debug /r:../bin/Microsoft.ManagedChessAPI.dll $*.cs

.cs.exe:
! if "$(INSTALLTEST)"=="1"
	copy /y $*.cs ..\Installer\Tests\Managed
! else
	csc /debug /unsafe /r:../bin/Microsoft.ManagedChessAPI.dll /r:NativeMethods.dll $*.cs
! endif

tombuild: $(EXES)
	@echo
	@echo "============ EXEs built alright ============"

compare:
	$(PERL) ..\Test\regression.pl cmdfile exefile

build: NativeMethods.dll
	$(PERL) ..\test\regression.pl -nog nmake:build exefile

cleanbuild:
	$(PERL) ..\test\regression.pl -nog nmake exefile

regress:
	$(PERL) ..\Test\regression.pl cmdfile exefile

regress-csb1:
	$(PERL) ..\Test\regression.pl cmdfile:csb1 exefile

golden:
	$(PERL) ..\Test\regression.pl -g cmdfile exefile

all: build regress	

##############################################################################

clean:
	-del *~ *.pdb *.exe golden\out.* $(EXES) 2>nul
