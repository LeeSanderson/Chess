/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// Win32Chess.cpp : Defines the entry point for the DLL application.
//
#include "Win32Base.h"
#include "ChessWrapperSentry.h"

#ifdef _MANAGED
#pragma managed(push, off)
#endif

HINSTANCE g_hinstDll = NULL;

BOOL WINAPI DllMain( HANDLE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch(ul_reason_for_call){
		case DLL_PROCESS_ATTACH :
			{
            g_hinstDll = (HINSTANCE)hModule;
				if( !ChessWrapperSentry::Init() ) {
					std::cerr<< "Cannot Init ChessWrapperEntry" << std::endl;
                return FALSE;
				}
			}

			__fallthrough;

		case DLL_THREAD_ATTACH :
			// by default all threads are 'in' chess - 
			//  The wrappers get into effect only for threads that are 'out' of chess
			ChessWrapperSentry::EnterChessImpl(); 
			break;

		case DLL_THREAD_DETACH :
			break;

		case DLL_PROCESS_DETACH :
			break;

		default:
        assert(FALSE);
	}
    return TRUE;
}

#ifdef _MANAGED
#pragma managed(pop)
#endif

