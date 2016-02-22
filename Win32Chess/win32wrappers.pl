use strict;

my $win_include = "c:\\Program\ Files\\Microsoft\ Visual\ Studio\ 8\\VC\\PlatformSDK\\Include\\";
my $kernel32_dll = "c:\\WINDOWS\\system32\\kernel32.dll";

my %kernel32_exports;
my %fn_signatures;

sub get_kernel32_exports{
  print STDERR "Reading exports from $kernel32_dll\n";
  open(DUMPEH, "dumpbin.exe /exports $kernel32_dll|") || die "Cannot get kernel32 exports using dumpbin";
  while(my $export = <DUMPEH>){
    last if $export =~ /ordinal\s*hint/; # till we read the header
  }
  while(my $export = <DUMPEH>){
    my($empty, $ordinal, $hint, $rva, $fn, @rest) = split(/\s+/, $export);
    next unless $ordinal =~ /^\d+$/; #ordinal is a number
    next unless $hint =~ /^[\dA-Fa-f]+$/; #hint is a hex number
    if($rva =~  /^[\dA-Fa-f]+$/){
      #rva is a hex number
      # do nothing
    }
    else{
      $fn = $rva; 
      # hack - when functions are forwarded to another library
      # there is no address - thus our split() will produce $fn in the 
      # third field
    }
    $kernel32_exports{$fn} = $ordinal;
  }
}


sub get_next_signature{
  #a function signature from DECLSPEC_IMPORT till the next semicolon
  my @signature = ();
  while(my $line = <WINBASEH>){
    next if $line =~ /^#/;
    if($line =~ /(__declspec\(dllimport\).*$)/){
      $line = "$1\n";
      while($line){
	if($line =~ /(^.*\;)/){
	  $line = $1;
	  push @signature, $line;
	  return join("", @signature);
	}
	push @signature, $line;
	$line = <WINBASEH>;
      }
    }
  }
  return "";
}

sub strip_sal_annotations{
  my($signature) = @_;
  $signature =~ s/__[\w_]*\([^\)]*\)//g;
  $signature =~ s/__[\w_]*//g;
  return $signature;
}

my $num_undefined = 0;
my @wrapped_functions;

sub get_kernel32_signatures{
  my($sign_file) = @_;
  print STDERR "Reading kernel32 function signatures from $sign_file\n";
  open(WINBASEH, "cl.exe /D_WIN32_WINNT=0x0520 /E \"$sign_file\"|") || die "Cannot open $sign_file\n";
  while(my $signature = get_next_signature()){
    $signature = strip_sal_annotations($signature);
    $signature =~ s/\s+/ /gm;
    if($signature =~ /(.*)\s(\w+)\s*\((.*)\)/){
      my $pre = $1;
      my $fn = $2;
      my $args = $3;
      my @args = split(/\s*,\s*/, $args);
      my @argvars;
      my @argtypes;
      for my $arg (@args) {
	my @f = split(/\s+\*?\*?/, $arg);
	unless($f[0]){
	  shift @f;
	}
	if (@f == 1) {
	  unless($f[0] =~ /\s*VOID\s*/i){
	    push @f, "dummy";
	    $arg .= " dummy";
	  }
	}
	my $v = pop @f;
	if ($v eq "OPTIONAL") {
	  $v = pop @f;
	}
	push @argtypes, join(" ", @f);
	$v =~ s/\[\]//;
	push @argvars, $v;
      }
      $args = join(", ", @args);
      my $argvars = join(", ", @argvars);
      if ($argvars =~ /\s*VOID\s*/i) {
	$argvars = "";
      }
      $pre =~ s/DECLSPEC_IMPORT//;
      $pre =~ s/DECLSPEC_NORETURN//;
      $pre =~ s/WINAPI//;
      $pre =~ s/^\s+//;
      $fn_signatures{$fn} = [$pre, $fn, $args, $argvars];
    }
    else{
      print STDERR "Cannot find function name in $signature\n";
    }
  }
  for my $fn (sort keys %kernel32_exports){
    if(!defined $fn_signatures{$fn}){
      $num_undefined ++;
      print STDERR "[$num_undefined] Cannot find signature for $fn\n";
      next;
    }
    push @wrapped_functions, $fn;
  }
}

my $SYNC_FUNCTION = 1;
my $BLOCKING_SYNC_FUNCTION = 2;

my %sync_functions = 
  ("APCProc" => $SYNC_FUNCTION,
#"GetOverlappedResult" => $SYNC_FUNCTION,
"GetQueuedCompletionStatus" => $SYNC_FUNCTION,
"PostQueuedCompletionStatus" => $SYNC_FUNCTION,
"QueueUserAPC" => $SYNC_FUNCTION,
"QueueUserWorkItem" => $SYNC_FUNCTION,
"AcquireSRWLockExclusive" => $SYNC_FUNCTION,
"AcquireSRWLockShared" => $SYNC_FUNCTION,
#"InitializeConditionVariable" => $SYNC_FUNCTION,
#"InitializeSRWLock" => $SYNC_FUNCTION,
"ReleaseSRWLockExclusive" => $SYNC_FUNCTION,
"ReleaseSRWLockShared" => $SYNC_FUNCTION,
"SleepConditionVariableCS" => $SYNC_FUNCTION,
"SleepConditionVariableSRW" => $SYNC_FUNCTION,
"Sleep" => $SYNC_FUNCTION,
"SleepEx" => $SYNC_FUNCTION,
"SwitchToThread" => $SYNC_FUNCTION,
"WakeAllConditionVariable" => $SYNC_FUNCTION,
"WakeConditionVariable" => $SYNC_FUNCTION,
#"DeleteCriticalSection" => $SYNC_FUNCTION,
"EnterCriticalSection" => $SYNC_FUNCTION,
#"InitializeCriticalSection" => $SYNC_FUNCTION,
#"InitializeCriticalSectionAndSpinCount" => $SYNC_FUNCTION,
#"InitializeCriticalSectionEx" => $SYNC_FUNCTION,
"LeaveCriticalSection" => $SYNC_FUNCTION,
#"SetCriticalSectionSpinCount" => $SYNC_FUNCTION,
"TryEnterCriticalSection" => $SYNC_FUNCTION,
"CreateIoCompletionPort" => $SYNC_FUNCTION,
"CreateThread" => $SYNC_FUNCTION,
"SuspendThread" => $SYNC_FUNCTION,
"ResumeThread" => $SYNC_FUNCTION,
"CreateEvent" => $SYNC_FUNCTION,
"CreateEventEx" => $SYNC_FUNCTION,
"OpenEvent" => $SYNC_FUNCTION,
"PulseEvent" => $SYNC_FUNCTION,
"ResetEvent" => $SYNC_FUNCTION,
"SetEvent" => $SYNC_FUNCTION,
"InitOnceBeginInitialize" => $SYNC_FUNCTION,
"InitOnceComplete" => $SYNC_FUNCTION,
"InitOnceExecuteOnce" => $SYNC_FUNCTION,
"InitOnceInitialize" => $SYNC_FUNCTION,
"InterlockedAdd" => $SYNC_FUNCTION,
"InterlockedAdd64" => $SYNC_FUNCTION,
"InterlockedAddAcquire" => $SYNC_FUNCTION,
"InterlockedAddAcquire64" => $SYNC_FUNCTION,
"InterlockedAddRelease" => $SYNC_FUNCTION,
"InterlockedAddRelease64" => $SYNC_FUNCTION,
"InterlockedAnd" => $SYNC_FUNCTION,
"InterlockedAndAcquire" => $SYNC_FUNCTION,
"InterlockedAndRelease" => $SYNC_FUNCTION,
"InterlockedAnd8" => $SYNC_FUNCTION,
"InterlockedAnd8Acquire" => $SYNC_FUNCTION,
"InterlockedAnd8Release" => $SYNC_FUNCTION,
"InterlockedAnd16" => $SYNC_FUNCTION,
"InterlockedAnd16Acquire" => $SYNC_FUNCTION,
"InterlockedAnd16Release" => $SYNC_FUNCTION,
"InterlockedAnd64" => $SYNC_FUNCTION,
"InterlockedAnd64Acquire" => $SYNC_FUNCTION,
"InterlockedAnd64Release" => $SYNC_FUNCTION,
"InterlockedBitTestAndReset" => $SYNC_FUNCTION,
"InterlockedBitTestAndReset64" => $SYNC_FUNCTION,
"InterlockedBitTestAndSet" => $SYNC_FUNCTION,
"InterlockedBitTestAndSet64" => $SYNC_FUNCTION,
"InterlockedCompare64Exchange128" => $SYNC_FUNCTION,
"InterlockedCompare64ExchangeAcquire128" => $SYNC_FUNCTION,
"InterlockedCompare64ExchangeRelease128" => $SYNC_FUNCTION,
"InterlockedCompareExchange" => $SYNC_FUNCTION,
"InterlockedCompareExchange64" => $SYNC_FUNCTION,
"InterlockedCompareExchangeAcquire" => $SYNC_FUNCTION,
"InterlockedCompareExchangeAcquire64" => $SYNC_FUNCTION,
"InterlockedCompareExchangePointer" => $SYNC_FUNCTION,
"InterlockedCompareExchangePointerAcquire" => $SYNC_FUNCTION,
"InterlockedCompareExchangePointerRelease" => $SYNC_FUNCTION,
"InterlockedCompareExchangeRelease" => $SYNC_FUNCTION,
"InterlockedCompareExchangeRelease64" => $SYNC_FUNCTION,
"InterlockedDecrement" => $SYNC_FUNCTION,
"InterlockedDecrement64" => $SYNC_FUNCTION,
"InterlockedDecrementAcquire" => $SYNC_FUNCTION,
"InterlockedDecrementAcquire64" => $SYNC_FUNCTION,
"InterlockedDecrementRelease" => $SYNC_FUNCTION,
"InterlockedDecrementRelease64" => $SYNC_FUNCTION,
"InterlockedExchange" => $SYNC_FUNCTION,
"InterlockedExchange64" => $SYNC_FUNCTION,
"InterlockedExchangeAcquire" => $SYNC_FUNCTION,
"InterlockedExchangeAcquire64" => $SYNC_FUNCTION,
#"InterlockedExchangeAdd" => $SYNC_FUNCTION,
"InterlockedExchangeAdd64" => $SYNC_FUNCTION,
"InterlockedExchangeAddAcquire" => $SYNC_FUNCTION,
"InterlockedExchangeAddAcquire64" => $SYNC_FUNCTION,
"InterlockedExchangeAddRelease" => $SYNC_FUNCTION,
"InterlockedExchangeAddRelease64" => $SYNC_FUNCTION,
"InterlockedExchangePointer" => $SYNC_FUNCTION,
"InterlockedExchangePointerAcquire" => $SYNC_FUNCTION,
"InterlockedIncrement" => $SYNC_FUNCTION,
"InterlockedIncrement64" => $SYNC_FUNCTION,
"InterlockedIncrementAcquire" => $SYNC_FUNCTION,
"InterlockedIncrementAcquire64" => $SYNC_FUNCTION,
"InterlockedIncrementRelease" => $SYNC_FUNCTION,
"InterlockedIncrementRelease64" => $SYNC_FUNCTION,
"InterlockedOr" => $SYNC_FUNCTION,
"InterlockedOrAcquire" => $SYNC_FUNCTION,
"InterlockedOrRelease" => $SYNC_FUNCTION,
"InterlockedOr8" => $SYNC_FUNCTION,
"InterlockedOr8Acquire" => $SYNC_FUNCTION,
"InterlockedOr8Release" => $SYNC_FUNCTION,
"InterlockedOr16" => $SYNC_FUNCTION,
"InterlockedOr16Acquire" => $SYNC_FUNCTION,
"InterlockedOr16Release" => $SYNC_FUNCTION,
"InterlockedOr64" => $SYNC_FUNCTION,
"InterlockedOr64Acquire" => $SYNC_FUNCTION,
"InterlockedOr64Release" => $SYNC_FUNCTION,
"InterlockedXor" => $SYNC_FUNCTION,
"InterlockedXorAcquire" => $SYNC_FUNCTION,
"InterlockedXorRelease" => $SYNC_FUNCTION,
"InterlockedXor8" => $SYNC_FUNCTION,
"InterlockedXor8Acquire" => $SYNC_FUNCTION,
"InterlockedXor8Release" => $SYNC_FUNCTION,
"InterlockedXor16" => $SYNC_FUNCTION,
"InterlockedXor16Acquire" => $SYNC_FUNCTION,
"InterlockedXor16Release" => $SYNC_FUNCTION,
"InterlockedXor64" => $SYNC_FUNCTION,
"InterlockedXor64Acquire" => $SYNC_FUNCTION,
"InterlockedXor64Release" => $SYNC_FUNCTION,
"CreateMutex" => $SYNC_FUNCTION,
"CreateMutexEx" => $SYNC_FUNCTION,
"OpenMutex" => $SYNC_FUNCTION,
"ReleaseMutex" => $SYNC_FUNCTION,
"AddSIDToBoundaryDescriptor" => $SYNC_FUNCTION,
"ClosePrivateNamespace" => $SYNC_FUNCTION,
"CreateBoundaryDescriptor" => $SYNC_FUNCTION,
"CreatePrivateNamespace" => $SYNC_FUNCTION,
"DeleteBoundaryDescriptor" => $SYNC_FUNCTION,
"OpenPrivateNamespace" => $SYNC_FUNCTION,
"Semaphore" => $SYNC_FUNCTION,
"CreateSemaphore" => $SYNC_FUNCTION,
"CreateSemaphoreEx" => $SYNC_FUNCTION,
"OpenSemaphore" => $SYNC_FUNCTION,
"ReleaseSemaphore" => $SYNC_FUNCTION,
#"InitializeSListHead" => $SYNC_FUNCTION,
#"InterlockedFlushSList" => $SYNC_FUNCTION,
#"InterlockedPopEntrySList" => $SYNC_FUNCTION,
#"InterlockedPushEntrySList" => $SYNC_FUNCTION,
#"QueryDepthSList" => $SYNC_FUNCTION,
#"ChangeTimerQueueTimer" => $SYNC_FUNCTION,
"CreateTimerQueue" => $SYNC_FUNCTION,
"CreateTimerQueueTimer" => $SYNC_FUNCTION,
"DeleteTimerQueue" => $SYNC_FUNCTION,
"DeleteTimerQueueEx" => $SYNC_FUNCTION,
"DeleteTimerQueueTimer" => $SYNC_FUNCTION,
"MsgWaitForMultipleObjects" => $SYNC_FUNCTION,
"MsgWaitForMultipleObjectsEx" => $SYNC_FUNCTION,
#"RegisterWaitForSingleObject" => $SYNC_FUNCTION,
"SignalObjectAndWait" => $SYNC_FUNCTION,
#"UnregisterWait" => $SYNC_FUNCTION,
#"UnregisterWaitEx" => $SYNC_FUNCTION,
"WaitForMultipleObjects" => $SYNC_FUNCTION,
"WaitForMultipleObjectsEx" => $SYNC_FUNCTION,
"WaitForSingleObject" => $SYNC_FUNCTION,
"WaitForSingleObjectEx" => $SYNC_FUNCTION,
"WaitOrTimerCallback" => $SYNC_FUNCTION,
#"CancelWaitableTimer" => $SYNC_FUNCTION,
#"CreateWaitableTimer" => $SYNC_FUNCTION,
"CreateWaitableTimerEx" => $SYNC_FUNCTION,
"OpenWaitableTimer" => $SYNC_FUNCTION,
#"SetWaitableTimer" => $SYNC_FUNCTION,
"TimerAPCProc" => $SYNC_FUNCTION,
"DuplicateHandle" => $SYNC_FUNCTION,
"ReadFile" => $SYNC_FUNCTION,
"ReadFileEx" => $SYNC_FUNCTION,
"WriteFile" => $SYNC_FUNCTION,
"WriteFileEx" => $SYNC_FUNCTION,
);

sub WrapperForChess(){
  for my $fn (@wrapped_functions) {
    if ($sync_functions{$fn}) {
      print "#define WRAP_$fn\n";
    }
  }
  print "\n";
  
  for my $fn (@wrapped_functions) {
    my ($pre, $fn, $args, $argvars) = @{$fn_signatures{$fn}};
    if ($sync_functions{$fn}) {
      if ($pre =~ /\s*void\s*/) {
	# a win32 synchronization function returning void
	print <<HERE;
$pre (WINAPI * Real_$fn)($args)
   = $fn;

__declspec(dllexport) $pre WINAPI Mine_$fn($args){
#ifdef WRAP_$fn
  if(ChessWrapperSentry::Wrap("$fn")){
     ChessWrapperSentry sentry;
     Chess::LogCall("$fn");
     __wrapper_$fn($argvars);
     return;
  }
#endif
  return Real_$fn($argvars);
}
HERE
      } else {
	print <<HERE;
$pre (WINAPI * Real_$fn)($args)
   = $fn;

__declspec(dllexport) $pre WINAPI Mine_$fn($args){
#ifdef WRAP_$fn
  if(ChessWrapperSentry::Wrap("$fn")){
     ChessWrapperSentry sentry;
     Chess::LogCall("$fn");
     $pre res = __wrapper_$fn($argvars);
     return res;
  }
#endif
  return Real_$fn($argvars);
}
HERE
      }
    } else {
      print <<HERE;
$pre (WINAPI * Real_$fn)($args)
  = $fn;

__declspec(dllexport) $pre WINAPI Mine_$fn($args){
  if(ChessWrapperSentry::Wrap("$fn")){
     ChessWrapperSentry sentry;
     Chess::LogCall("$fn");
   }
  return Real_$fn($argvars);
}
HERE
    }
  }

  print "LONG AttachDetours(){\n";
  print "   DetourTransactionBegin();\n";
  for my $fn (@wrapped_functions) {
     if($sync_functions{$fn}){
         print "   ChessDetourAttach(&(PVOID&)Real_$fn, Mine_$fn);\n"; 
     }
  }
  print "   return ChessDetourTransactionCommit();\n";
  print "}\n";

  print "LONG DetachDetours(){\n";
  print "   DetourTransactionBegin();\n";
  for my $fn (@wrapped_functions) {
     if($sync_functions{$fn}){
        print "   ChessDetourDetach(&(PVOID&)Real_$fn, Mine_$fn);\n"; 
     }
  }
  print "   return ChessDetourTransactionCommit();\n";
  print "}\n";



}

my %hb_sync_functions = 
  ("APCProc" => $SYNC_FUNCTION,
#"GetOverlappedResult" => $SYNC_FUNCTION,
# "GetQueuedCompletionStatus" => $SYNC_FUNCTION,
# "PostQueuedCompletionStatus" => $SYNC_FUNCTION,
# "QueueUserAPC" => $SYNC_FUNCTION,
# "QueueUserWorkItem" => $SYNC_FUNCTION,
# "AcquireSRWLockExclusive" => $SYNC_FUNCTION,
# "AcquireSRWLockShared" => $SYNC_FUNCTION,
#"InitializeConditionVariable" => $SYNC_FUNCTION,
#"InitializeSRWLock" => $SYNC_FUNCTION,
# "ReleaseSRWLockExclusive" => $SYNC_FUNCTION,
# "ReleaseSRWLockShared" => $SYNC_FUNCTION,
# "SleepConditionVariableCS" => $SYNC_FUNCTION,
# "SleepConditionVariableSRW" => $SYNC_FUNCTION,
# "Sleep" => $SYNC_FUNCTION,
# "SleepEx" => $SYNC_FUNCTION,
# "SwitchToThread" => $SYNC_FUNCTION,
# "WakeAllConditionVariable" => $SYNC_FUNCTION,
# "WakeConditionVariable" => $SYNC_FUNCTION,
#"DeleteCriticalSection" => $SYNC_FUNCTION,
"EnterCriticalSection" => $SYNC_FUNCTION,
#"InitializeCriticalSection" => $SYNC_FUNCTION,
#"InitializeCriticalSectionAndSpinCount" => $SYNC_FUNCTION,
#"InitializeCriticalSectionEx" => $SYNC_FUNCTION,
"LeaveCriticalSection" => $SYNC_FUNCTION,
#"SetCriticalSectionSpinCount" => $SYNC_FUNCTION,
"TryEnterCriticalSection" => $SYNC_FUNCTION,
# "CreateIoCompletionPort" => $SYNC_FUNCTION,
# "CreateThread" => $SYNC_FUNCTION,
# "SuspendThread" => $SYNC_FUNCTION,
# "ResumeThread" => $SYNC_FUNCTION,
# "CreateEvent" => $SYNC_FUNCTION,
# "CreateEventEx" => $SYNC_FUNCTION,
# "OpenEvent" => $SYNC_FUNCTION,
# "PulseEvent" => $SYNC_FUNCTION,
# "ResetEvent" => $SYNC_FUNCTION,
# "SetEvent" => $SYNC_FUNCTION,
# "InitOnceBeginInitialize" => $SYNC_FUNCTION,
# "InitOnceComplete" => $SYNC_FUNCTION,
# "InitOnceExecuteOnce" => $SYNC_FUNCTION,
# "InitOnceInitialize" => $SYNC_FUNCTION,
# "InterlockedAdd" => $SYNC_FUNCTION,
# "InterlockedAdd64" => $SYNC_FUNCTION,
# "InterlockedAddAcquire" => $SYNC_FUNCTION,
# "InterlockedAddAcquire64" => $SYNC_FUNCTION,
# "InterlockedAddRelease" => $SYNC_FUNCTION,
# "InterlockedAddRelease64" => $SYNC_FUNCTION,
# "InterlockedAnd" => $SYNC_FUNCTION,
# "InterlockedAndAcquire" => $SYNC_FUNCTION,
# "InterlockedAndRelease" => $SYNC_FUNCTION,
# "InterlockedAnd8" => $SYNC_FUNCTION,
# "InterlockedAnd8Acquire" => $SYNC_FUNCTION,
# "InterlockedAnd8Release" => $SYNC_FUNCTION,
# "InterlockedAnd16" => $SYNC_FUNCTION,
# "InterlockedAnd16Acquire" => $SYNC_FUNCTION,
# "InterlockedAnd16Release" => $SYNC_FUNCTION,
# "InterlockedAnd64" => $SYNC_FUNCTION,
# "InterlockedAnd64Acquire" => $SYNC_FUNCTION,
# "InterlockedAnd64Release" => $SYNC_FUNCTION,
# "InterlockedBitTestAndReset" => $SYNC_FUNCTION,
# "InterlockedBitTestAndReset64" => $SYNC_FUNCTION,
# "InterlockedBitTestAndSet" => $SYNC_FUNCTION,
# "InterlockedBitTestAndSet64" => $SYNC_FUNCTION,
# "InterlockedCompare64Exchange128" => $SYNC_FUNCTION,
# "InterlockedCompare64ExchangeAcquire128" => $SYNC_FUNCTION,
# "InterlockedCompare64ExchangeRelease128" => $SYNC_FUNCTION,
# "InterlockedCompareExchange" => $SYNC_FUNCTION,
# "InterlockedCompareExchange64" => $SYNC_FUNCTION,
# "InterlockedCompareExchangeAcquire" => $SYNC_FUNCTION,
# "InterlockedCompareExchangeAcquire64" => $SYNC_FUNCTION,
# "InterlockedCompareExchangePointer" => $SYNC_FUNCTION,
# "InterlockedCompareExchangePointerAcquire" => $SYNC_FUNCTION,
# "InterlockedCompareExchangePointerRelease" => $SYNC_FUNCTION,
# "InterlockedCompareExchangeRelease" => $SYNC_FUNCTION,
# "InterlockedCompareExchangeRelease64" => $SYNC_FUNCTION,
# "InterlockedDecrement" => $SYNC_FUNCTION,
# "InterlockedDecrement64" => $SYNC_FUNCTION,
# "InterlockedDecrementAcquire" => $SYNC_FUNCTION,
# "InterlockedDecrementAcquire64" => $SYNC_FUNCTION,
# "InterlockedDecrementRelease" => $SYNC_FUNCTION,
# "InterlockedDecrementRelease64" => $SYNC_FUNCTION,
# "InterlockedExchange" => $SYNC_FUNCTION,
# "InterlockedExchange64" => $SYNC_FUNCTION,
# "InterlockedExchangeAcquire" => $SYNC_FUNCTION,
# "InterlockedExchangeAcquire64" => $SYNC_FUNCTION,
#"InterlockedExchangeAdd" => $SYNC_FUNCTION,
# "InterlockedExchangeAdd64" => $SYNC_FUNCTION,
# "InterlockedExchangeAddAcquire" => $SYNC_FUNCTION,
# "InterlockedExchangeAddAcquire64" => $SYNC_FUNCTION,
# "InterlockedExchangeAddRelease" => $SYNC_FUNCTION,
# "InterlockedExchangeAddRelease64" => $SYNC_FUNCTION,
# "InterlockedExchangePointer" => $SYNC_FUNCTION,
# "InterlockedExchangePointerAcquire" => $SYNC_FUNCTION,
# "InterlockedIncrement" => $SYNC_FUNCTION,
# "InterlockedIncrement64" => $SYNC_FUNCTION,
# "InterlockedIncrementAcquire" => $SYNC_FUNCTION,
# "InterlockedIncrementAcquire64" => $SYNC_FUNCTION,
# "InterlockedIncrementRelease" => $SYNC_FUNCTION,
# "InterlockedIncrementRelease64" => $SYNC_FUNCTION,
# "InterlockedOr" => $SYNC_FUNCTION,
# "InterlockedOrAcquire" => $SYNC_FUNCTION,
# "InterlockedOrRelease" => $SYNC_FUNCTION,
# "InterlockedOr8" => $SYNC_FUNCTION,
# "InterlockedOr8Acquire" => $SYNC_FUNCTION,
# "InterlockedOr8Release" => $SYNC_FUNCTION,
# "InterlockedOr16" => $SYNC_FUNCTION,
# "InterlockedOr16Acquire" => $SYNC_FUNCTION,
# "InterlockedOr16Release" => $SYNC_FUNCTION,
# "InterlockedOr64" => $SYNC_FUNCTION,
# "InterlockedOr64Acquire" => $SYNC_FUNCTION,
# "InterlockedOr64Release" => $SYNC_FUNCTION,
# "InterlockedXor" => $SYNC_FUNCTION,
# "InterlockedXorAcquire" => $SYNC_FUNCTION,
# "InterlockedXorRelease" => $SYNC_FUNCTION,
# "InterlockedXor8" => $SYNC_FUNCTION,
# "InterlockedXor8Acquire" => $SYNC_FUNCTION,
# "InterlockedXor8Release" => $SYNC_FUNCTION,
# "InterlockedXor16" => $SYNC_FUNCTION,
# "InterlockedXor16Acquire" => $SYNC_FUNCTION,
# "InterlockedXor16Release" => $SYNC_FUNCTION,
# "InterlockedXor64" => $SYNC_FUNCTION,
# "InterlockedXor64Acquire" => $SYNC_FUNCTION,
# "InterlockedXor64Release" => $SYNC_FUNCTION,
# "CreateMutex" => $SYNC_FUNCTION,
# "CreateMutexEx" => $SYNC_FUNCTION,
# "OpenMutex" => $SYNC_FUNCTION,
# "ReleaseMutex" => $SYNC_FUNCTION,
# "AddSIDToBoundaryDescriptor" => $SYNC_FUNCTION,
# "ClosePrivateNamespace" => $SYNC_FUNCTION,
# "CreateBoundaryDescriptor" => $SYNC_FUNCTION,
# "CreatePrivateNamespace" => $SYNC_FUNCTION,
# "DeleteBoundaryDescriptor" => $SYNC_FUNCTION,
# "OpenPrivateNamespace" => $SYNC_FUNCTION,
# "Semaphore" => $SYNC_FUNCTION,
# "CreateSemaphore" => $SYNC_FUNCTION,
# "CreateSemaphoreEx" => $SYNC_FUNCTION,
# "OpenSemaphore" => $SYNC_FUNCTION,
# "ReleaseSemaphore" => $SYNC_FUNCTION,
#"InitializeSListHead" => $SYNC_FUNCTION,
#"InterlockedFlushSList" => $SYNC_FUNCTION,
#"InterlockedPopEntrySList" => $SYNC_FUNCTION,
#"InterlockedPushEntrySList" => $SYNC_FUNCTION,
#"QueryDepthSList" => $SYNC_FUNCTION,
#"ChangeTimerQueueTimer" => $SYNC_FUNCTION,
# "CreateTimerQueue" => $SYNC_FUNCTION,
# "CreateTimerQueueTimer" => $SYNC_FUNCTION,
# "DeleteTimerQueue" => $SYNC_FUNCTION,
# "DeleteTimerQueueEx" => $SYNC_FUNCTION,
# "DeleteTimerQueueTimer" => $SYNC_FUNCTION,
# "MsgWaitForMultipleObjects" => $SYNC_FUNCTION,
# "MsgWaitForMultipleObjectsEx" => $SYNC_FUNCTION,
#"RegisterWaitForSingleObject" => $SYNC_FUNCTION,
#"SignalObjectAndWait" => $SYNC_FUNCTION,
#"UnregisterWait" => $SYNC_FUNCTION,
#"UnregisterWaitEx" => $SYNC_FUNCTION,
#"WaitForMultipleObjects" => $SYNC_FUNCTION,
#"WaitForMultipleObjectsEx" => $SYNC_FUNCTION,
#"WaitForSingleObject" => $SYNC_FUNCTION,
#"WaitForSingleObjectEx" => $SYNC_FUNCTION,
#"WaitOrTimerCallback" => $SYNC_FUNCTION,
#"CancelWaitableTimer" => $SYNC_FUNCTION,
#"CreateWaitableTimer" => $SYNC_FUNCTION,
#"CreateWaitableTimerEx" => $SYNC_FUNCTION,
#"OpenWaitableTimer" => $SYNC_FUNCTION,
#"SetWaitableTimer" => $SYNC_FUNCTION,
#"TimerAPCProc" => $SYNC_FUNCTION,
#"DuplicateHandle" => $SYNC_FUNCTION,
#"ReadFile" => $SYNC_FUNCTION,
#"ReadFileEx" => $SYNC_FUNCTION,
#"WriteFile" => $SYNC_FUNCTION,
#"WriteFileEx" => $SYNC_FUNCTION,
);

sub WrapperForHBLogger(){
  for my $fn (@wrapped_functions) {
    my ($pre, $fn, $args, $argvars) = @{$fn_signatures{$fn}};
    if ($hb_sync_functions{$fn}) {
      print <<HERE;
$pre __wrapper_$fn($args);
HERE
    }
  }
  print "\n";
  for my $fn (@wrapped_functions) {
    my ($pre, $fn, $args, $argvars) = @{$fn_signatures{$fn}};
    if ($hb_sync_functions{$fn}) {
      my $fn_call = "$pre res = __wrapper_$fn($argvars)";
      my $return = "return res";
      if ($pre =~ /\s*void\s*/) {
	$fn_call = "__wrapper_$fn($argvars)";
	$return = "return";
      }
      print <<HERE;
$pre (WINAPI * Real_$fn)($args)
  = $fn;

__declspec(dllexport) $pre WINAPI Mine_$fn($args){
   if(HBLogger::IsInitialized() && !HBLogger::InHBLogger()){
      HBLogger::Enter();
      $fn_call;
      HBLogger::Leave();
      $return;
   }
   return Real_$fn($argvars);
}

HERE
    }
    else{
      print <<HERE;
__declspec(dllexport) $pre WINAPI Mine_$fn($args){
   return $fn($argvars);
}

HERE
    }
  }
}


#main

get_kernel32_exports();
get_kernel32_signatures("win32defs.h");
WrapperForChess();
#WrapperForHBLogger();
