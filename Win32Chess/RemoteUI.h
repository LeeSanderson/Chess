/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#ifndef _REMOTEUI_H_
#define _REMOTEUI_H_

#include <windows.h>

//
// Implement and export these functions in both the desktop and device code.
//

// GetShimSettings will be called to retrieve shim-specific run-time settings
// to be sent to the device.
BOOL GetShimSettings (LPCTSTR pszAppName, LPVOID *ppData, DWORD *pcbData);
// [in] pszAppName - the name of the module to get settings for.
// [out] ppData - a pointer to a data blob. The shim writer is responsible for
//                allocating this blob. The app verifier UI will not use this
//                blob at all; it will simply send it to the device.
// [out] pcbData - the size (in bytes) of the allocated data blob.
//
// Return TRUE to indicate *ppData contains valid data, or FALSE if *ppData
// does not contain any data to send to the device.

// FreeShimSettings will be called when the shim-allocated data blob returned
// by GetShimSettings is no longer needed.
void FreeShimSettings (LPVOID pData, DWORD cbData);
// [in] pData - a pointer to a shim-allocated data blob.
// [in] cbData - the size (in bytes) of teh allocated data blob.

// SetShimSettings will be called to give the shim an opportunity to save
// shim-specific run-time settings.
BOOL SetShimSettings (LPCTSTR pszAppName, LPVOID pData, DWORD cbData);
// [in] pszAppName - the name of the module to get settings for.
// [in] pData -a shim-defined data blob, returned from GetShimSettings.
// [in] cbData - the size (in bytes) of the allocated data blob.

typedef BOOL (*PFN_GetShimSettings)(LPCTSTR, LPVOID *, DWORD *);
typedef void (*PFN_FreeShimSettings)(LPVOID, DWORD);
typedef BOOL (*PFN_SetShimSettings)(LPCTSTR, LPVOID, DWORD);

#endif // _REMOTEUI_H_

