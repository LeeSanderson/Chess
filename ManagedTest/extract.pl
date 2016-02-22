%chesscodes = (
   0 => "Success",
   -1 => "TestFailure",
   -2 => "ChessDeadlock",
   -3 => "ChessLivelock",
   -4 => "ChessTimeout",
   -5 => "ChessNonDet",
   -6 => "ChessInvalidTest",
   -7 => "ChessRace",
   -8 => "ChessIncompleteInterleavingCoverage",
   -9 => "ChessInvalidObservation",
   -100 => "ChessFailure",
   -10 => "ChessAtomicityViolation",
   -201 => "UnitTestAssertFailure",
   -202 => "UnitTestException",
   -42 => "PERL_EXIT_CODE_NOT_FOUND"
);

open(LS,"dir golden |");
while(<LS>) {
  if (/cmdfile\.(.*)\.exefile\.(.*)/) {
     $cmd = $1;
     $file = $2;
     processFile($cmd,$file);
  }
}
close(LS);

foreach $file (keys %tests) {
  open(OUT,"> $file.test.cs");
  print OUT "// $file.cs\n";
  print OUT "[ChessMethod]\n";
  foreach $cmd (keys %{$tests{$file}}) {
     print OUT "[ExpectedChessResult(\"$cmd\", ChessExitCode.$chesscodes{$exitcode{$file}{$cmd}}";
     print OUT ", SchedulesRan = $tests{$file}{$cmd}";
     print OUT ", LastThreadCount = $threads{$file}{$cmd}";
     print OUT ", LastExecSteps = $steps{$file}{$cmd}";
     print OUT ", LastHBExecSteps = $hbexecs{$file}{$cmd}";
     print OUT ")]\n";
  }
  close(OUT);
} 

sub processFile {
  local($cmd,$file) = @_;
  local($fullfile) = "golden\\golden.cmdfile.$cmd.exefile.$file";
  local($tests,$threads,$steps,$hbexecs) = (-1,-1,-1,-1);
  local($exitcode) = (-42);
  open (FILE,$fullfile);
  while(<FILE>) {
     if (/Tests:\s+(\d+)\s+Threads:\s+(\d+)\s+ExecSteps:\s+(\d+)\s+HBExecs:\s+(\d+)/) {
        ($tests,$threads,$steps,$hbexecs) = ($1,$2,$3,$4);
     } elsif (/Exit code =\s+(-?\d+)/) {
        $exitcode = $1;
     }
  }
  close(FILE);
  $tests{$file}{$cmd} = $tests;
  $threads{$file}{$cmd} = $threads;
  $steps{$file}{$cmd} = $steps;
  $hbexecs{$file}{$cmd} = $hbexecs;
  $exitcode{$file}{$cmd} = $exitcode;
}
