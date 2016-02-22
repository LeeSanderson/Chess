/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>
#include <stdio.h>


class Account {
  int balance;
  CRITICAL_SECTION cs;
public:
  Account(int b) {
    balance = b;
    InitializeCriticalSection(&cs);
  }
  ~Account(){
    DeleteCriticalSection(&cs);
  }

  int read() {
    int r;
    EnterCriticalSection(&cs);
      r = balance;
    LeaveCriticalSection(&cs);
    return r;
  }

  void deposit(int n) {
    EnterCriticalSection(&cs);
      balance = balance + n;
    LeaveCriticalSection(&cs);    
  }

  void withdraw(int n) {
    int r = read();
    EnterCriticalSection(&cs);
      balance = r - n;
    LeaveCriticalSection(&cs);
  }
};


DWORD WINAPI WithdrawThread(LPVOID param) {
  Account* a = (Account*) param;
  a->withdraw(2);
  return 0;
}

char* sep = "*******************************************";
extern "C" 
__declspec(dllexport) int ChessTestRun(){
  DWORD tid;
  HANDLE hThread;

  Account* a = new Account(10);
  hThread = CreateThread(NULL, 0, WithdrawThread, a, 0, &tid);

  a->deposit(1);
    
  WaitForSingleObject(hThread, INFINITE);
  CloseHandle(hThread);
  
  bool expResult  = (a->read() == 9);
  if(expResult)
    printf("True\n");
  else
    printf("**** False ****\n");

  delete a;
  return expResult ? 0 : -1;
}

int main(){
  ChessTestRun();
}


