/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// IATDetours.cpp : Defines the entry point for the DLL application.
//

#include "Win32Base.h"

#ifdef _MANAGED
#pragma managed(push, off)
#endif

HMODULE GetModuleByName(const char* moduleName){
	HANDLE hProcess = GetCurrentProcess();
		HMODULE modules[32];
	HMODULE* lphModule = modules;

	DWORD needed;
	if(!EnumProcessModules(hProcess, lphModule, 32, &needed)){
		*GetChessErrorStream()<< "Need space for " << needed << " modules" << std::endl;
		return NULL;
	}
	HMODULE hModule = NULL;
	for(DWORD i=0; i<(needed/sizeof(HMODULE)); i++){
		char name[256];
		if(lphModule[i] == INVALID_HANDLE_VALUE)
			continue;

#pragma warning( push )
#pragma warning( disable: 25068)
		DWORD ret = GetModuleBaseNameA(hProcess, lphModule[i], name, 255);
#pragma warning( pop )

		if(ret == 0){
			//*GetChessErrorStream()<< "GetModuleBaseNameA failed with " << GetLastError() << std::endl;
			continue;
		}
		name[ret] = 0;
		if(strncmp(name, moduleName, ret) == 0){
			hModule = lphModule[i];
			break;
		}
	}
	if(hModule == NULL){
		*GetChessErrorStream()<< "Didnt find " << moduleName << " in list of loaded modules " << std::endl;
		return NULL;
	}
	return hModule;
}

bool IATDetour(HMODULE hModule, stdext::hash_map<void*, void*>& fnTable){
	HANDLE hProcess = GetCurrentProcess();
	MODULEINFO modInfo;
	if(!GetModuleInformation(hProcess, hModule, &modInfo, sizeof(MODULEINFO))){
		*GetChessErrorStream()<< "GetModuleInformation failed with " << GetLastError() << std::endl;
		return false;
	}

	ULONG IATSize;
	PVOID IATBase= ImageDirectoryEntryToData(modInfo.lpBaseOfDll, TRUE, IMAGE_DIRECTORY_ENTRY_IAT, &IATSize);
	if(IATBase == NULL){
		*GetChessErrorStream()<< "ImageDirectoryEntryToData failed with " << GetLastError() << std::endl;
		return false;
	}

	
    PVOID ProtectedIATBase = IATBase;
    ULONG ProtectedIATSize = IATSize;
	DWORD OldProtect;
	if(!VirtualProtect (ProtectedIATBase, ProtectedIATSize,PAGE_READWRITE, &OldProtect)){
		*GetChessErrorStream()<< "Virutal Protect failed with " << GetLastError() << std::endl;
		return false;
	}
    PVOID* ProcAddresses = (PVOID *)IATBase;
    ULONG NumberOfProcAddresses = (ULONG)(IATSize / sizeof(PVOID));

    for (ULONG Pi = 0; Pi < NumberOfProcAddresses; Pi += 1) {
		if(ProcAddresses[Pi] == NULL)
			continue;
		stdext::hash_map<void*, void*>::iterator fi = fnTable.find(ProcAddresses[Pi]);
		if(fi != fnTable.end()){
			void* prev = ProcAddresses[Pi];
			ProcAddresses[Pi] = fi->second;
			fi->second = prev;
		}
    }

    VirtualProtect(ProtectedIATBase, ProtectedIATSize, OldProtect, &OldProtect);
	return true;
}


DWORD GetFunctionAddressByName(HMODULE hModule, const char* moduleName, const char* moduleFn){
	HANDLE hProcess = GetCurrentProcess();
	MODULEINFO modInfo;
	if(!GetModuleInformation(hProcess, hModule, &modInfo, sizeof(MODULEINFO))){
		*GetChessErrorStream()<< "GetModuleInformation failed with " << GetLastError() << std::endl;
		return false;
	}

	ULONG impDescSize;

#pragma warning( push )
#pragma warning( disable: 25003) // I dont know why Prefix insists on wanting impDesc a const, it is not 
	PIMAGE_IMPORT_DESCRIPTOR impDesc = 
		(PIMAGE_IMPORT_DESCRIPTOR)ImageDirectoryEntryToData(modInfo.lpBaseOfDll, TRUE, IMAGE_DIRECTORY_ENTRY_IMPORT, &impDescSize);


	bool found = false;
	while (impDesc->Name)
	{
		const char* pszModName = (char*)((PBYTE) modInfo.lpBaseOfDll + impDesc->Name);
		if (_stricmp(pszModName, moduleName) == 0){
			found = true;
			break;   
		}
		impDesc++;
	}
	if(!found)
		return 0;

	PIMAGE_THUNK_DATA pThunk = 
		(PIMAGE_THUNK_DATA)((PBYTE) modInfo.lpBaseOfDll + impDesc->OriginalFirstThunk);

	PIMAGE_THUNK_DATA iat = 
		(PIMAGE_THUNK_DATA)((PBYTE) modInfo.lpBaseOfDll + impDesc->FirstThunk);

#pragma warning( pop )
	
	while(pThunk->u1.AddressOfData){
		DWORD rva = pThunk->u1.AddressOfData;
		if(!(rva &0x80000000)){
			PIMAGE_IMPORT_BY_NAME impName = (PIMAGE_IMPORT_BY_NAME)((PBYTE) modInfo.lpBaseOfDll + rva);
			if(_stricmp((char*)(impName->Name), moduleFn) == 0){
				return iat->u1.Function;
			}
		}
		pThunk++;
		iat++;
	}
	return 0;
}

//BOOL APIENTRY DllMain( HMODULE hModule,
//                       DWORD  ul_reason_for_call,
//                       LPVOID lpReserved
//					 )
//{
//    return TRUE;
//}

#ifdef _MANAGED
#pragma managed(pop)
#endif

