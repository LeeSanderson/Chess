/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>
#include "RemoteUI.h"
#include "ShimSettings.h"

BOOL GetShimSettings (LPCTSTR pszAppName, LPVOID *ppData, DWORD *pcbData)
{
    *ppData = LocalAlloc (LMEM_ZEROINIT, sizeof (SHIM_SETTINGS));
    if (*ppData)
    {
        // TODO
        // Retrieve run-time settings for pszAppName. For example, these could
        // be saved in the registry, an .ini file, retrieved from a test server,
        // etc.

        if (pcbData)
        {
            *pcbData = sizeof (SHIM_SETTINGS);
        }

        return TRUE;
    }

    return FALSE;
}

void FreeShimSettings (LPVOID pData, DWORD cbData)
{
    LocalFree (pData);
}

BOOL SetShimSettings (LPCTSTR pszAppName, LPVOID pData, DWORD cbData)
{
    // TODO
    // Save run-time settings for pszAppName.

    return TRUE;
}

