/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>

// Do not modify this function. This is required only for compatibility with
// previous version of application verifier for windows ce.
__declspec (dllexport) LPVOID InitializeHooksEx (DWORD dwReason, DWORD *pdwHookCount)
{
    if (pdwHookCount)
    {
        *pdwHookCount = 1;
    }

    return NULL;
}

