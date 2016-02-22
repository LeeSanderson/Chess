/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

//#define _WIN32_WINNT        0x0501
#define NT
#include "ChessBase.h"

#include <CodeAnalysis\Warnings.h>  // For the definition of ALL_CODE_ANALYSIS_WARNINGS

#pragma warning( push )  // Push the existing state of all warnings
#pragma warning( disable: 25000 25001 25003 25004 25005 25007 25011 25019 25025 25033 25048 25057 ALL_CODE_ANALYSIS_WARNINGS )  // Disable all PREfast warnings

#include <windows.h>
#ifndef UNDER_CE
#include <wincon.h> // Not available in CE
#endif
#include <crtdbg.h>
#ifndef UNDER_CE
#include <signal.h> // Not available in CE
#endif
#include <psapi.h>
#ifndef UNDER_CE
#include <Dbghelp.h> // Not available in CE
#include <process.h> // Not available in CE
#endif
#include <map>
#include <queue>
#include <deque>
#pragma warning( pop )  // Restore all warnings to their previous state

#include "ChessStl.h"

#ifdef WIN32CHESS_EXPORTS
#define WIN32CHESS_API __declspec(dllexport)
#else
#define WIN32CHESS_API __declspec(dllimport)
#endif
