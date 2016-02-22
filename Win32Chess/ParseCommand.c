/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>

#ifdef UNDER_CE

// Function definitions for shell extensions
typedef void (*PFN_FmtPuts)(const TCHAR *s,...);
typedef TCHAR * (*PFN_Gets)(TCHAR *s, int cch);
typedef BOOL (*PFN_PARSECOMMAND)(LPCTSTR szCmd, LPTSTR szCmdLine, PFN_FmtPuts pfnFmtPuts, PFN_Gets pfnGets);

#if 0
--------------------------------------------------------------------------------
Implement and export this command to expose a new command in Platform Builder's
CE Target Control.

When an unknown command is entered, the Target Control will enumerate through
all loaded extensions. Each extension's ParseCommand function will be called,
until an extension returns TRUE to indicate that the command has been handled.

szCmd will be the first token in the command string.
szCmdLine will be the rest of the command string.
pfnFmtPuts is the callback output function provided. When called by the Target
Control, this function will allow the shim to output to the Target Control
window.

A shell extension can be loaded using the 'loadext' command.
--------------------------------------------------------------------------------
#endif

// ----------------------------------------------------------------------------
// ParseCommand

BOOL
ParseCommand (LPCTSTR szCmd, LPTSTR szCmdLine, PFN_FmtPuts pfnFmtPuts, PFN_Gets pfnGets)
{
    // Help?
    if (!_tcscmp (szCmd, _T("?"))) {
        pfnFmtPuts (_T("help :%s\r\n"), szCmdLine);
        return FALSE;
    }

    // Is this command meant for us?
    // TODO - change 'shimgen' to the command you'd like to expose.
    if (0 !=_tcsicmp (szCmd, _T("shimgen")))
        return FALSE;

    pfnFmtPuts (_T("mindll (%s):\r\n"), szCmdLine);

    // TRUE will let shell.exe know that we've handled this command.
    return TRUE;
}

#endif // UNDER_CE


