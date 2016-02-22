/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>
#include <prsht.h>
#include "RemoteUI.h"
#include "OptionsDlg.h"
#include "ShimSettings.h"

#ifdef UNDER_NT
#include <tchar.h>
#include <strsafe.h>

#define NKDbgPrintfW _tprintf
#endif

SHIM_SETTINGS gShimSettings;

LRESULT CALLBACK
DlgOptions(
    HWND   hDlg,
    UINT   message,
    WPARAM wParam,
    LPARAM lParam
    );

//
// GetOptionsDialogProc will be called to retrieve a dialog procedure and
// resource template for a shim. This function is optional; a shim may implement
// another means of gathering shim-specific run-time options.
//
// [out] pDlgProc is a pointer to your dialog proc.
// [out] pres_template is a pointer to your dialog template.
//
BOOL GetOptionsDialogProc (DLGPROC *pDlgProc, LPCWSTR *pres_template)
{
    if (!pDlgProc || !pres_template)
        return FALSE;

    *pDlgProc = DlgOptions;
    *pres_template = (LPCWSTR) IDD_OPTIONS_DLG;

    return TRUE;
}

BOOL
RefreshDlgData(
    HWND hDlg
    )
{
    TCHAR szTemp[20];

    //
    // Limit the number of characters for each edit control.
    //
    SendDlgItemMessage(hDlg, IDC_EDIT_FOO, EM_LIMITTEXT, (WPARAM)5, 0);

    StringCchPrintf (szTemp, 20, L"%d", gShimSettings.dwFoo);
    SetDlgItemText(hDlg, IDC_EDIT_FOO, szTemp);

    return TRUE;
}

//
// This function should only be called from the dialog proc while handling
// the WM_INITDIALOG message.
//
LPCWSTR ExeNameFromLParam(LPARAM lParam)
{
    if (lParam) {
        LPCWSTR szRet = (LPCWSTR)(((LPPROPSHEETPAGE)lParam)->lParam);
        if (szRet) {
            return szRet;
        }
    }

    return _T("{default}");
}

LRESULT CALLBACK
DlgOptions(
    HWND   hDlg,
    UINT   message,
    WPARAM wParam,
    LPARAM lParam
    )
{
    static LPCWSTR szExeName;
    SHIM_SETTINGS ShimSettingsTemp;
    LPSHIM_SETTINGS pShimSettings = & gShimSettings;
    DWORD cbSize;

    switch (message) {
    case WM_INITDIALOG:
        {
            //
            // find out what exe we're handling settings for
            //
            szExeName = ExeNameFromLParam(lParam);

            GetShimSettings (szExeName, & pShimSettings, & cbSize);

            RefreshDlgData (hDlg);

            return TRUE;
        }

    case WM_COMMAND:
        switch (LOWORD(wParam)) {
        case IDC_DEFAULT:
            {
                // Save off the globals
                memcpy (& ShimSettingsTemp, & gShimSettings, sizeof (SHIM_SETTINGS));

                // Set the default values
                gShimSettings.dwFoo = 0;

                // Update the UI to reflect the default values.
                RefreshDlgData (hDlg);

                // Restore the globals (in case this isn't 'applied')
                memcpy (& gShimSettings, & ShimSettingsTemp, sizeof (SHIM_SETTINGS));

                break;
            }
        }
        break;

    case WM_NOTIFY:
        switch (((NMHDR FAR *) lParam)->code) {

        case PSN_KILLACTIVE:
            {
                break;
            }

        case PSN_APPLY:
            {
                NKDbgPrintfW (_T("Applying settings for %s\r\n"), szExeName);

                // Retrieve settings from the UI.
                gShimSettings.dwFoo = GetDlgItemInt (
                    hDlg,
                    IDC_EDIT_FOO,
                    NULL,
                    FALSE
                    );

                SetShimSettings (szExeName, & gShimSettings, sizeof (SHIM_SETTINGS));

                break;
            }
        }
        break;
    }

    return FALSE;
}

