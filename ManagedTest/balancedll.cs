/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

public class TestBalance {

  public bool Run() {
    Account a = new Account();
    Thread t = new Thread(delegate(object o) { 
        Account b = (Account)o;
        b.withdraw(10);
    });
    t.Start(a);
    // put parent thread code here
    a.deposit(10);
    t.Join();
    return (a.read() == 10);
  }

}


public class Account {
  
int balance;

public Account() {
  balance = 10;
}

public void withdraw(int n) {
  int r = read();
  lock(this) {
    balance = r - n;
  }
}

public int read() {
   int r;
   lock(this) {
      r = balance;
   }
   return r;
}

public void deposit(int n) {
  lock(this) {
     balance = balance + n;
  }
}

}
