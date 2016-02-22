/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>
#include <stdio.h>

class BankAccount{
public:
	BankAccount(){
		InitializeCriticalSection(&savingsLock);
		InitializeCriticalSection(&checkingLock);
		savingsBalance = 0;
		checkingBalance = 0;
	}

	~BankAccount(){
		DeleteCriticalSection(&savingsLock);
		DeleteCriticalSection(&checkingLock);
	}

	void SavingsDeposit(int amt){
		EnterCriticalSection(&savingsLock);
		savingsBalance += amt;
		LeaveCriticalSection(&savingsLock);
	}

	void CheckingDeposit(int amt){
		EnterCriticalSection(&checkingLock);
		checkingBalance += amt;
		LeaveCriticalSection(&checkingLock);
	}

	void TransferFromSavingsToChecking(int amt){
		EnterCriticalSection(&savingsLock);
		EnterCriticalSection(&checkingLock);
		savingsBalance -= amt;
		checkingBalance += amt;
		LeaveCriticalSection(&checkingLock);
		LeaveCriticalSection(&savingsLock);
	}

	void TransferFromCheckingToSaving(int amt){
		EnterCriticalSection(&checkingLock);
		EnterCriticalSection(&savingsLock);
		checkingBalance -= amt;
		savingsBalance += amt;
		LeaveCriticalSection(&savingsLock);
		LeaveCriticalSection(&checkingLock);
	}


private:
	CRITICAL_SECTION savingsLock;
	CRITICAL_SECTION checkingLock;
	int savingsBalance;
	int checkingBalance;
};


DWORD WINAPI SavingsToCheckingThread(LPVOID param) {
	BankAccount* account = (BankAccount*) param;
	account->TransferFromSavingsToChecking(100);
	return 0;
}

DWORD WINAPI CheckingToSavingsThread(LPVOID param) {
	BankAccount* account = (BankAccount*) param;
	account->TransferFromCheckingToSaving(100);
	return 0;
}

// this has to be even
const int NUM_TRANSFER_THREADS = 2;

extern "C"
__declspec(dllexport) int ChessTestRun() {
  HANDLE hThread[NUM_TRANSFER_THREADS];
  
  if(NUM_TRANSFER_THREADS %2 != 0){
	  return -1; // set NUM_TRANSFER_THREADS to an even number
  }

  BankAccount* account = new BankAccount();
  int initialDeposit = 100*NUM_TRANSFER_THREADS/2;
  account->SavingsDeposit(initialDeposit);
  account->CheckingDeposit(initialDeposit);
	
  for(int i=0; i<NUM_TRANSFER_THREADS/2; i++){
    hThread[2*i]   = CreateThread(NULL, 0, SavingsToCheckingThread, (LPVOID)account, 0, NULL);
    hThread[2*i+1] = CreateThread(NULL, 0, CheckingToSavingsThread, (LPVOID)account, 0, NULL);
  }
  
  WaitForMultipleObjects(NUM_TRANSFER_THREADS, hThread, true, INFINITE);

  for(int i=0; i<NUM_TRANSFER_THREADS; i++){
    CloseHandle(hThread[i]);
  }
  delete account;
	 
  return 0;
}

const int NUM_ITER = 10;
int main(){
	for(int i=0; i<NUM_ITER; i++){
		int testSuccess = ChessTestRun();
		if(testSuccess < 0){
			printf("Iteration %d failed \n", i+1);
		}
		else{
			printf("Iteration %d succeeded \n", i+1);
		}
	}
}
