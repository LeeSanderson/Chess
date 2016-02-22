/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// stdafx.cpp : source file that includes just the standard includes
// RunDll.pch will be the pre-compiled header
// stdafx.obj will contain the pre-compiled type information

#pragma warning(push)

#include <CodeAnalysis\Warnings.h>

#pragma warning( disable: 25000 25001 25003 25004 25005 25007 25011 25019 25025 25032 25033 25048 25057 ALL_CODE_ANALYSIS_WARNINGS )  // Disable all PREfast warnings

#include "stdafx.h"

#pragma warning(pop)

// TODO: reference any additional headers you need in STDAFX.H
// and not in this file
