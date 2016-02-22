/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// Win32VistaWrappers.cpp : Defines the exported functions for the DLL application.
#include "Win32Base.h"
#include "IChessWrapper.h"

IChessWrapper* GetConditionVariableWrappers();
IChessWrapper* GetSRWLockWrappers();

IChessWrapper* vistaWrappers[8];

extern "C" __declspec(dllexport) IChessWrapper** GetChessWrappers(){
	int i = 0;
	vistaWrappers[i] = GetConditionVariableWrappers();
	if(vistaWrappers[i]) i++;

	vistaWrappers[i] = GetSRWLockWrappers();
	if(vistaWrappers[i]) i++;

	vistaWrappers[i] = 0;
	return vistaWrappers;
} 