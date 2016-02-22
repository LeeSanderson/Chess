/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Win32WrapperManager.h"
#include "IATDetours.h"

IChessWrapper* GetAsyncProcWrapper();
IChessWrapper* GetWrappersMSVCRT();


void Win32WrapperManager::LoadWrapper(const wchar_t* wrapperDll){
#ifndef UNDER_CE
	HMODULE vistaWrappers = LoadLibraryW(wrapperDll);
	if(vistaWrappers == NULL){
		//*GetChessErrorStream()<< "\nCHESS WARNING : Cannot load " << wrapperDll << " (Try setting the CHESS path)\n";
	}
	else{
		FARPROC getWrappers = GetProcAddress(vistaWrappers, "GetChessWrappers");
		if(getWrappers == NULL){
			*GetChessErrorStream()<< "\nCHESS WARNING : Cannot load GetChessWrappers() from " << wrapperDll << std::endl;
		}
		else{
			IChessWrapper** wrappers = ((IChessWrapper** (*)())getWrappers)();
			while(*wrappers){
				RegisterWrapper(*wrappers);
				wrappers++;
			}
		}
	}
#endif
}

void Win32WrapperManager::LoadWrappers(){

// No Extra wrappers to load in CE
#ifndef UNDER_CE
	RegisterWrapper(GetAsyncProcWrapper());
	RegisterWrapper(GetWrappersMSVCRT());

	HMODULE kern32 = LoadLibraryW(L"kernel32.dll");
	if(!kern32) {
       Chess::AbnormalExit(-1, "Cannot load kernel32.dll");
       return; // should never reach this
	}

	// Load W2k3 Wrappers if required
	FARPROC fake = GetProcAddress(kern32, "InterlockedCompareExchange64"); // try getting some function that is defined in w2k3 but not in winxp sp2
	if(fake != NULL){
		LoadWrapper(L"Win32W2k3Wrappers");
	}

	//LoadVistaWrappers if required
	fake = GetProcAddress(kern32, "AcquireSRWLockExclusive"); // try getting some function that is defined in vista but not in w2k3
	if(fake != NULL){
		LoadWrapper(L"Win32VistaWrappers");
	}

	//LoadWrapper(L"Win32MFCWrappers");
#endif

}

void GetWin32Wrappers(stdext::hash_map<void*, void*>& wrapperTable);
//void GetWrappersConsoleTable(stdext::hash_map<void*, void*>& wrapperTable);

bool Win32WrapperManager::RegisterTestModule(HMODULE hModule){

    // Shimming is done through AppVerifier Shim in CE
    // We don't need to do the Import Address Table rewrite
#ifndef UNDER_CE
	stdext::hash_map<void*, void*> fnTable;
	GetWin32Wrappers(fnTable);

	for(size_t i=0; i<wrappers.size(); i++){
		WrapperFunctionInfo* p = wrappers[i]->GetWrapperFunctions();
		while(p->origFunctionAddr){
			fnTable[p->origFunctionAddr] = p->wrapperFunctionAddr;
			p++;
		}
	}

	if(!IATDetour(hModule, fnTable)){
		*GetChessErrorStream()<< "Cannot detour Win32 functions" << std::endl;
		return false;
	}
#endif

//	GetWrappersMSVCRT()->RegisterTestModule(hModule);


	//stdext::hash_map<void*, void*>fltTable;
	//GetFltTable(fltTable);
	//if(!IATDetour(hModule, fltTable)){
	//	*GetChessErrorStream()<< "Cannot detour Flt functions" << std::endl;
	//	return false;
	//}

	//stdext::hash_map<void*, void*>consoleTable;
	//GetWrappersConsoleTable(consoleTable);
	//if(!IATDetour(hModule, consoleTable)){
	//	*GetChessErrorStream()<< "Cannot detour Console functions" << std::endl;
	//	return false;
	//}
	return true;
}
