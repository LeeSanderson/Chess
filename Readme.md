#CHESS#

CHESS is a open source tool created by **[Microsoft Research](http://research.microsoft.com/en-us/projects/chess/)** for systematic and disciplined testing of concurrent programs. 

CHESS repeatedly runs a concurrent test ensuring that every run takes a different interleaving. If an interleaving results in an error, CHESS can reproduce the interleaving for improved debugging. 

CHESS is available for both managed and native programs.

This repository is a copy of the [original CHESS Codeplex repository](https://chesstool.codeplex.com/) which seems to have been inactive since 2013. The [latest thread](https://chesstool.codeplex.com/discussions/450787) on the Codeplex seems to indicate that  Microsoft were intending to incorporate CHESS inside the next version of Visual Studio (although this does not seem to have happened):

> There was a rumour running around that it would become a part of VS 2012, but I'm not sure. Think it was decided to shift it to the next release 

## Status ##

The code here can be compiled using [Visual Studio 2015 Community edition](https://go.microsoft.com/fwlink/?LinkId=691978&clcid=0x409) or [Microsoft Build Tools 2015](https://www.microsoft.com/en-us/download/details.aspx?id=48159). 

Some minor modifications have been made to the [original CHESS Codeplex repository](https://chesstool.codeplex.com/) in order to get the code to compile. These change include:

1. w32chess projects have been removed from the solution (although they still exist in the repository). These projects build tools to run CHESS against native programs and have been removed because of problems getting them to compile using the Visual Studio 2015 C++ compiler.

2. The CopyBin project has been removed from the solution (although it still exist in the repository). This project contains some pre and post build scripts that register the newly compiled version of CHESS in the GAC and registry. These scripts cause the build to fail with UAC errors unless the build is run under an administrator account. The functionality of these scripts is being migrated to the NANT script.  


## Documentation ##

There is lots of documentation about CHESS on [Codeplex](https://chesstool.codeplex.com/documentation) and on the [CHESS Microsoft Research pages](http://research.microsoft.com/en-us/projects/chess/).
