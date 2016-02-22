/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Win32Base.h"
#include "Semaphore.h"
#include "ChessAssert.h"

Semaphore::Semaphore(){
	sem = NULL;
	//val = 0;
	//signature = 0xfeedbeef;
}

void Semaphore::AtomicUpDown(Semaphore* downSem){
	//val++;
	//if(downSem)
	//	downSem->val--;
	if(!ReleaseSemaphore(sem, 1, NULL)){
		*GetChessErrorStream()<< "Error in Up() Semaphore, Code " << GetLastError() << std::endl;
		assert(false);
	}
	if(downSem)
		WaitForSingleObject(downSem->sem, INFINITE);
}

void Semaphore::Up(){
//	val++;
//	std::cout << "Releasing " << (void*)sem << std::endl;
	if(!ReleaseSemaphore(sem, 1, NULL)){
		*GetChessErrorStream()<< "Error in Up() Semaphore, Code " << GetLastError() << std::endl;
		assert(false);
	}
}

bool Semaphore::IsNull() const{
	return sem == NULL;
}

void Semaphore::Down(){
//	val--;
	WaitForSingleObject(sem, INFINITE);
}

bool Semaphore::TryDown(){
	DWORD ret = WaitForSingleObject(sem, 0);
	if(ret == WAIT_OBJECT_0){
		//assert(val > 0);
		//val--;
		return true;
	}
	else if(ret == WAIT_TIMEOUT){
		return false;
	}
	else{
		*GetChessErrorStream()<< "Semaphore TryDown Failed " << GetLastError() << std::endl;
		return false;
	}
}

void Semaphore::Init(){
	sem = CreateSemaphoreW(NULL, 0, 1, NULL);
	//val = 0;
	if(sem == NULL)
	{
		*GetChessErrorStream()<< "Cannot create semaphore " << std::endl;
	}
	//signature = 0xdefaced0;
}

void Semaphore::Clear(){
	if(sem != NULL){
		CloseHandle(sem);
		sem = NULL;
	}
	//signature = 0xdeadbeef;
}

bool Semaphore::InternalStateValid(){
	//assert(signature == 0xfeedbeef || signature == 0xdeadbeef || signature == 0xdefaced0);
	return true;
}