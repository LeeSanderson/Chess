/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

//////////////////////////////////////////////////////////////////////////////
//
//  File:       _win32.cpp
//  Module:     traceapi.dll
//
//  Copyright:  1996-2006, Microsoft Corporation
//
//  Microsoft Research Detours Package Version 2.1 (Build_203)
//

///////////////////////////////////////////////////////////////// Trampolines.
//

extern "C" {
  int _in_detours = 0;

@DETOUR_TRAMPOLINES;

}

////////////////////////////////////////////////////////////// AttachDetours.
//
static PCHAR DetRealName(PCHAR psz)
{
    PCHAR pszBeg = psz;
    // Move to end of name.
    while (*psz) {
        psz++;
    }
    // Move back through A-Za-z0-9 names.
    while (psz > pszBeg &&
           ((psz[-1] >= 'A' && psz[-1] <= 'Z') ||
            (psz[-1] >= 'a' && psz[-1] <= 'z') ||
            (psz[-1] >= '0' && psz[-1] <= '9'))) {
        psz--;
    }
    return psz;
}

static VOID Dump(PBYTE pbBytes, LONG nBytes, PBYTE pbTarget)
{
    CHAR szBuffer[256];
    PCHAR pszBuffer = szBuffer;

    for (LONG n = 0; n < nBytes; n += 12) {
#ifdef _CRT_INSECURE_DEPRECATE
        pszBuffer += sprintf_s(pszBuffer, sizeof(szBuffer), "  %p: ", pbBytes + n);
#else
        pszBuffer += sprintf(pszBuffer, "  %p: ", pbBytes + n);
#endif
        for (LONG m = n; m < n + 12; m++) {
            if (m >= nBytes) {
#ifdef _CRT_INSECURE_DEPRECATE
                pszBuffer += sprintf_s(pszBuffer, sizeof(szBuffer), "   ");
#else
                pszBuffer += sprintf(pszBuffer, "   ");
#endif
            }
            else {
#ifdef _CRT_INSECURE_DEPRECATE
                pszBuffer += sprintf_s(pszBuffer, sizeof(szBuffer), "%02x ", pbBytes[m]);
#else
                pszBuffer += sprintf(pszBuffer, "%02x ", pbBytes[m]);
#endif
            }
        }
        if (n == 0) {
#ifdef _CRT_INSECURE_DEPRECATE
            pszBuffer += sprintf_s(pszBuffer, sizeof(szBuffer), "[%p]", pbTarget);
#else
            pszBuffer += sprintf(pszBuffer, "[%p]", pbTarget);
#endif
        }
#ifdef _CRT_INSECURE_DEPRECATE
        pszBuffer += sprintf_s(pszBuffer, sizeof(szBuffer), "\n");
#else
        pszBuffer += sprintf(pszBuffer, "\n");
#endif
    }

    Syelog(SYELOG_SEVERITY_INFORMATION, "%s", szBuffer);
}

static VOID Decode(PBYTE pbCode, LONG nInst)
{
    PBYTE pbSrc = pbCode;
    PBYTE pbEnd;
    PBYTE pbTarget;
    for (LONG n = 0; n < nInst; n++) {
        pbTarget = NULL;
        pbEnd = (PBYTE)DetourCopyInstruction(NULL, (PVOID)pbSrc, (PVOID*)&pbTarget);
        Dump(pbSrc, (int)(pbEnd - pbSrc), pbTarget);
        pbSrc = pbEnd;

        if (pbTarget != NULL) {
            break;
        }
    }
}

VOID DetAttach(PVOID *ppvReal, PVOID pvMine, PCHAR psz)
{
    LONG l = DetourAttach(ppvReal, pvMine);
    if (l != 0) {
        Syelog(SYELOG_SEVERITY_NOTICE,
               "Attach failed: `%s': error %d\n", DetRealName(psz), l);

        Decode((PBYTE)*ppvReal, 3);
    }
}

VOID DetDetach(PVOID *ppvReal, PVOID pvMine, PCHAR psz)
{
    LONG l = DetourDetach(ppvReal, pvMine);
    if (l != 0) {
#if 0
        Syelog(SYELOG_SEVERITY_NOTICE,
               "Detach failed: `%s': error %d\n", DetRealName(psz), l);
#else
        (void)psz;
#endif
    }
}

#define ATTACH(x,y)   DetAttach(x,y,#x)
#define DETACH(x,y)   DetDetach(x,y,#x)

LONG AttachDetours(VOID)
{
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());

    // For this many APIs, we'll ignore one or two can't be detoured.
    DetourSetIgnoreTooSmall(TRUE);


@DETOUR_ATTACH;

    if (DetourTransactionCommit() != 0) {
        PVOID *ppbFailedPointer = NULL;
        LONG error = DetourTransactionCommitEx(&ppbFailedPointer);

        printf("traceapi.dll: Attach transaction failed to commit. Error %d (%p/%p)",
               error, ppbFailedPointer, *ppbFailedPointer);
        return error;
    }
    return 0;
}

LONG DetachDetours(VOID)
{
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());

    // For this many APIs, we'll ignore one or two can't be detoured.
    DetourSetIgnoreTooSmall(TRUE);

@DETOUR_DETACH;

    if (DetourTransactionCommit() != 0) {
        PVOID *ppbFailedPointer = NULL;
        LONG error = DetourTransactionCommitEx(&ppbFailedPointer);

        printf("traceapi.dll: Detach transaction failed to commit. Error %d (%p/%p)",
               error, ppbFailedPointer, *ppbFailedPointer);
        return error;
    }
    return 0;
}
//
///////////////////////////////////////////////////////////////// End of File.
