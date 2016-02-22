/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "Win32Base.h"

// fnTable is a map from Function Pointers to Function Pointers
// IATDetour modifies the Import Address Table for (an already loaded) hModule such that
//   an import function f in fnTable is detoured to fnTable[f]

// Returns false if detours fails

WIN32CHESS_API bool IATDetour(HMODULE hModule, stdext::hash_map<void*, void*>& fnTable);

HMODULE GetModuleByName(char* moduleName);
DWORD GetFunctionAddressByName(HMODULE hModule, char* moduleName, char* moduleFn);
