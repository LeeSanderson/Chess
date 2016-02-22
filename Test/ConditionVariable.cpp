/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>

CRITICAL_SECTION CritSection;
CONDITION_VARIABLE ConditionVar;
BOOL x;

DWORD WINAPI Consumer(void *p)
{ 
  UNREFERENCED_PARAMETER(p);
  EnterCriticalSection(&CritSection);
  
  while( !x )
    {
      SleepConditionVariableCS(&ConditionVar, &CritSection, INFINITE);
    }

  x = FALSE;
  
  LeaveCriticalSection(&CritSection);
  return 0;
}

DWORD WINAPI Producer(void *p) 
{
  EnterCriticalSection(&CritSection);
  
  x = TRUE;
  
  LeaveCriticalSection(&CritSection);
  WakeAllConditionVariable(&ConditionVar);
  return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){
  
  HANDLE h1, h2;
  InitializeCriticalSection(&CritSection);
  InitializeConditionVariable(&ConditionVar);
  x = FALSE;

  h1 = CreateThread(NULL, 0, Producer, 0, 0, NULL);
  h2 = CreateThread(NULL, 0, Consumer, 0, 0, NULL);

  WaitForSingleObject(h1, INFINITE);
  WaitForSingleObject(h2, INFINITE);
  CloseHandle(h1);
  CloseHandle(h2);
  DeleteCriticalSection(&CritSection);

  return 0;
}
