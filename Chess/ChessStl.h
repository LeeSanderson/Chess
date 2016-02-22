#pragma once

#include <CodeAnalysis\Warnings.h>  // For the definition of ALL_CODE_ANALYSIS_WARNINGS

#pragma warning( push )  // Push the existing state of all warnings
#pragma warning( disable: 25000 25001 25003 25004 25005 25007 25011 25019 25025 25033 25048 25057 ALL_CODE_ANALYSIS_WARNINGS )  // Disable all PREfast warnings

#include <vector>
#include <set>
#include <iostream>
#include <hash_map>
#include <hash_set>
#include <fstream>
#include <string>
#include <iomanip>
#include <sstream>
#include <time.h>


#pragma warning( pop )  // Restore all warnings to their previous state
 
//extern std::ostream* GetChessErrorStream();
//extern std::ostream* GetChessOutputStream();
