# usage regression.pl <options> arg_1 arg_2 ... arg_n
# Each arg_i is a file containing multiple lines L_i_1... L_i_m_i
# Each line L_i_j is of the form
#     <tag_i_j> : <cmd_i_j>
#
# Regression will run the command for all combinations of x,y,...z 
#    <cmd_1_x> <cmd_2_y> ... <cmd_n_z>
# store the output in
#    out.<arg_1>.<tag_1_x>.<arg_2>.<tag_2_y>...<arg_n>.<tag_n_z>
# and compare the output with 
#    golden.<arg_1>.<tag_1_x>.<arg_2>.<tag_2_y>...<arg_n>.<tag_n_z>

# version information (according to psinfo)

#Microsoft Windows 2000
# 5.0
#Microsoft® Windows® XP
# 5.1
#Vista
# 6.0
 
use strict;
use Win32;

my $opt_gen_golden = 0;
my $opt_no_golden = 0;
my $opt_passive = 0;
my $opt_nmake_dlls = 1;
my $opt_silent = 0;
my $opt_perf_clear = 0;
my $opt_no_perf_warnings = 0;
my $opt_output_tags = 0;

my $opt_perf_test = 1;
my $opt_perf_fidelity=10;

my $opt_out_prefix = "out";
my $opt_golden_prefix = "golden";
my $opt_perf_prefix = "perf.$ENV{COMPUTERNAME}";

sub ParseCmdLine {
  while (@ARGV) {
    if ($ARGV[0] =~ /^\-/) {
      my $argv = shift @ARGV;
      if ($argv eq "-g") {
	$opt_gen_golden = 1;
      }
      elsif ($argv eq "-p") {
	$opt_passive = 1;
      }
      elsif ($argv eq "-q") {
	$opt_silent = 1;
      }
      elsif ($argv eq "-nog") {
	$opt_no_golden = 1;
      }
      elsif ($argv eq "-perftest"){
	$opt_perf_test = 1;
      }
      elsif ($argv eq "-perf"){
	$opt_perf_test = 0;
      }
      elsif ($argv eq "-perfclear"){
	$opt_perf_clear = 1;
      }
      elsif ($argv eq "-noperfwarn"){
	$opt_no_perf_warnings = 1;
      }
      elsif ($argv eq "-outputtags"){
	$opt_output_tags = 1;
      }
    }
    else {
      return @ARGV;
    }
  }
}

sub PerfThis{
  my($cmd, $tag, $currtime) = @_;
  my ($lasttime, $avg, $num, $prevsum) = (0,0,0,0);

  if(!$opt_perf_clear && -e "$opt_perf_prefix.$tag"){
    open(FH, "<$opt_perf_prefix.$tag") || return "Cannot open $opt_perf_prefix.$tag\n";
    my $firstline = <FH>;
    my $secondline = <FH>;
    chomp($secondline);
    ($lasttime, $avg, $num, $prevsum) = split(/\s+/, $secondline);
  }
    
  # moving average = (total time + last time)/$num if $num > 0
  
  if($num == 0){
    $lasttime = $currtime;
  }
  else{
    $prevsum += $lasttime;
    $lasttime = $currtime;
    $avg = $prevsum*1.0/$num;
  }
  $num ++;

  if(!$opt_no_perf_warnings && $num > 3){
    my $delta = ($avg - $lasttime)*100.0/$avg;
    if($delta < -$opt_perf_fidelity){
	print ">>>>> Performance slowdown of ", -$delta, "% ($lasttime vs $avg)\n" unless $opt_silent;      
    }
    if($delta > $opt_perf_fidelity){
	print "+++++ Performance speedup of ", $delta, "% ($lasttime vs $avg)\n" unless $opt_silent;      
    }
  }

  # we are just doing a perf testing. Dont persist the measurements now
  if(!$opt_perf_test){
    open(FH, ">$opt_perf_prefix.$tag");
    print FH "#Current time, Moving Average, Total Count, Prev Sum of times (ms)\n";
    print FH "$lasttime\t$avg\t$num\t$prevsum\n";  
    close(FH);
  }
}

sub CreateCmdList {
    my $cmd_file = $_[0];
    my @cmds = ();

    open(CMDH, "<$cmd_file") || die "Cannot open command file $cmd_file :$!\n";
    my $line;
    while ($line = <CMDH>) {
	chomp $line;
	next unless $line;      # empty lines
	next if $line =~ /^\#/; # comment lines
	if($line =~ /^\s*(\S+)\s*\:\s*(.*)$/){
	  my $tag = $1;
	  my $cmd = $2;
	  # check here for WD=...
	  push(@cmds, $tag, $cmd);
	} 
	else{
	    print "Cannot parse line: \"$line\"\n";
	    next;
	}
    }
    close(CMDH);
    return \@cmds;
}

sub Normalize{
  my ($line) = @_;
  
  # replace anything that looks like a CHESS_ROOT directory with a "$(CHESS_ROOT)" 
  # Infer a CHESS_ROOT from a path of the form <drive>:(...)/main/(...)
  
  #  my $nonfile = "\/\\\"\*\:\?\<\>\|";
  #  my $sep = "\\\/";
  $line=~ s/([a-zA-Z]:([\\\/][^\/\\\"\*\:\?\<\>\|]*)*)[\\\/]main[\\\/]/\$\(CHESS_ROOT\)\\/g;
  return lc($line);
}

sub DoDiff {
  my($golden, $outfile) = @_;
  unless(-e $golden){
    print "Golden file $golden missing \n";
    return 0;
  }
  unless(open(GOLDH, "<$golden")){
    print "Cannot open $golden filen\n";
    return 0;
  }
  unless(open(OUTH, "<$outfile")){
    print "Cannot open $outfile filen\n";
    return 0;
  }
  while(1){
    my $g = <GOLDH>;
    my $o = <OUTH>;
    if(!$g && !$o){
      return 1;
    } 
#    $g = Normalize($g);
#    $o = Normalize($o);
    unless($g eq $o){
      print "Golden: $g";
      print "Output: $o";
      return 0;
    }
  }
}

sub Filter {
    my ($outfile) = @_;
    my $new_outfile = "$outfile.tmp";
    
    open(OUTH, "<$outfile");
    open(NEWOUTH, ">$new_outfile");
    my $o;
    my $found = 0;
    while ($o = <OUTH>) {
	if ($o =~ /^Stack trace:/) {
	    $found = 1;
	}
	if ($o =~ /^Exit code/) {
	    $found = 0;
	}
	if ($found == 0) {
	    print NEWOUTH $o;
	} 
    }
    close (OUTH);
    close (NEWOUTH);
    system "move $new_outfile $outfile";
}

sub ProcessCommandMacros{
  my($cmd) = @_;
  
  $cmd =~ s/\$EATCHESSARGS\s+([\/\-][\w\:\;\!\.\_]*[ ]*)*//g;
  return $cmd;
}


my @failedTags;

sub Execute {
  my($workingdir, $cmd, $tag) = @_;
  my $out = "$opt_out_prefix.$tag";
  my $golden = "golden\\$opt_golden_prefix.$tag";
  my $pwd = `cd`;
  chomp $pwd;
  my $outfile = "golden\\$out";
  my $printout = $out;

  if($opt_gen_golden){
    $outfile = "$pwd\\golden\\golden.$tag";
    $printout = "golden.$tag";
  }
  my $full_cmd = ProcessCommandMacros("$cmd");
  unless($opt_no_golden){
    $full_cmd .= " > $outfile 2>&1";
  }
  if (!($workingdir eq "")) {
    $full_cmd = "cd $workingdir & ($full_cmd)";
  }
  if($opt_passive){
    print "$full_cmd\n";
    print "echo Exit code = %ERRORLEVEL% >> $outfile\n";
  }
  elsif ($opt_output_tags) {
    print "$tag\n";
  }
  else{
    if($opt_no_golden){
      print "$full_cmd\n";
    }
    else{
      # print "$full_cmd\n";
      print "-> $printout\n" unless $opt_silent;
    }
    open(EXECCMD,"> execute_cmd.cmd");
    print EXECCMD "$full_cmd\n";
    print EXECCMD "echo Exit code = %ERRORLEVEL% >> $outfile";
    close(EXECCMD);
    my $start_time = Win32::GetTickCount();
    `execute_cmd.cmd`;
    my $end_time = Win32::GetTickCount();
    my $timems = $end_time - $start_time;
    PerfThis($full_cmd, $tag, $timems);

    if(!$opt_no_golden){
      Filter($outfile);
      if(!$opt_gen_golden && !DoDiff($golden, $outfile)){
	print ">>>>> Regression Failed\n" unless $opt_silent;
	push(@failedTags, $tag);
      }
    }
  }
}

sub RunRegression {
  my ($workingdir,$cmd, $tag, @cmdrefs) = @_;
  if(@cmdrefs == 0){
    # base case
    Execute($workingdir,$cmd, $tag);
    return;
  }
  
  my $file = shift @cmdrefs;
  my $cmdref = shift @cmdrefs;
  my @cmds = @{$cmdref}; # make a copy
  while(@cmds){
    my $t = shift @cmds;
    $t = "$file.$t";
    my $c = shift @cmds;
    my $wd = "";
    if ($c =~ /\s*WD\=\"(.*)\"\s+(.*)/) {
       die "Two occurrences of WD"
         if (!($workingdir eq "")); 
       $wd  = $1;
       $c = $2;
    } else {
       $wd = $workingdir;
    }
    $c = "$cmd $c" if $cmd;
    $t = "$tag.$t" if $tag;
    # cross product via recursive call
    RunRegression($wd, $c, $t, @cmdrefs);
  }

#   foreach my $t (keys %{$cmdref}){
#     my $c = $cmdref->{$t};
#     $t = "$file.$t";
#     $c = "$cmd $c" if $cmd;
#     $t = "$tag.$t" if $tag;
#     RunRegression($c, $t, @cmdrefs);
#   }
}

sub Main {
  my @files  = ParseCmdLine();
  my @cmdrefs;
  foreach my $file (@files){
    my @tags = ();
    if($file =~ /\:/){
      my @f = split(/\:/, $file);
      if(@f != 2){
	die "Invalid file argument $file\n";
      }
      $file = $f[0];
      @tags = split(/\,/, $f[1]);
    }
    my $cmdref = CreateCmdList($file);
    if(@tags){
      # run only the tag in @tags
      my @onlytagcmd;
      my %cmdHash = @{$cmdref}; #convert (tag, cmd) to (tag => cmd)
      foreach my $tag (@tags) {
	if (!(defined $cmdHash{$tag})) {
	  if ($tag =~ /(.*)\*/) {
	    # tag ends with wildcard *, so match on all
	    my $tagprefix = $1;
	    foreach my $ctag (keys %cmdHash) {
	      if ($ctag =~ /^$tagprefix/) {
		push(@onlytagcmd, $ctag, $cmdHash{$ctag});
	      }
	    }
	  } else {
	    die "No match for tag: $tag\n";
	  }
        } else {
	  #	w$onlytagcmd{$tag} = $cmdref->{$tag};
	  push(@onlytagcmd, $tag, $cmdHash{$tag});
	}
      }
      $cmdref = \@onlytagcmd;
    }
    push @cmdrefs, $file;
    push @cmdrefs, $cmdref;
  }
  RunRegression("", "", "", @cmdrefs);

  if(@failedTags){
    print "\n\n\n########################################################\n";
    print "The following ", scalar @failedTags, " regressions failed: \n";
    foreach my $tag (@failedTags){
      print "\t $tag\n";
    }
    die "\n\nTry windiff $opt_out_prefix.* $opt_golden_prefix.*\n";
  }
}
Main();
