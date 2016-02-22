#pragma once

#include "SyncVar.h"
typedef size_t HashValue;

inline HashValue IncHash(HashValue a, HashValue inc){
	return a ^ inc;
}

inline HashValue DecHash(HashValue a, HashValue inc){
	return a ^ inc;
}


// Jenkins Hash - from: http://burtleburtle.net/bob/hash/doobs.html
// Quoted:
//  By Bob Jenkins, 1996.  bob_jenkins@burtleburtle.net.  You may use this
//  code any way you wish, private, educational, or commercial.  It's free.
// 
// The use of Jenkins Hash was suggested by Gerard Holzmann after his success with
// this hash in the SPIN model checker

#define mix(a,b,c) \
{ \
  a -= b; a -= c; a ^= (c>>13); \
  b -= c; b -= a; b ^= (a<<8); \
  c -= a; c -= b; c ^= (b>>13); \
  a -= b; a -= c; a ^= (c>>12);  \
  b -= c; b -= a; b ^= (a<<16); \
  c -= a; c -= b; c ^= (b>>5); \
  a -= b; a -= c; a ^= (c>>3);  \
  b -= c; b -= a; b ^= (a<<10); \
  c -= a; c -= b; c ^= (b>>15); \
}

inline size_t Jenkins(size_t tid, size_t i, size_t var, size_t j){
	size_t a;
	size_t b;
	size_t c;
	a = b = 0x9e3779b9; // arbitrary value
	c=0;
	a += tid;
	b += i;
	c += var;
	mix(a,b,c);
	a += j;
	mix(a,b,c);
	return c;
}

inline size_t Jenkins(size_t tid, size_t i, size_t choice){
	size_t a;
	size_t b;
	size_t c;
	a = b = 0x9e3779b9; // arbitrary value
	c=0;
	a += tid;
	b += i;
	c += choice;
	mix(a,b,c);
	return c;
}

inline HashValue ComputeHash(Task tid, size_t i, SyncVar var, size_t j){
	return Jenkins(tid, i, var, j);
}

inline HashValue ComputeHash(Task tid, size_t i, size_t choice){
	return Jenkins(tid, i, choice);
}

inline HashValue ComputeHash(const SyncVar var[], int n){
	size_t a;
	size_t b;
	size_t c;
	a = b = 0x9e3779b9; // arbitrary value
	c=0;
	assert(n > 0);
	int p = 0;
	while(n >= 3){
		// var[0..p-1] has been hashed; var[p...p+n-1] needs to be hashed
		a += var[p++];
		b += var[p++];
		c += var[p++];
		mix(a,b,c);
		n -= 3;
	}
	switch(n){
		case 2: b += var[p+1];
			__fallthrough;
		case 1: a += var[p];
			__fallthrough;
		case 0: break;
		default: assert(false);
	}
	mix(a,b,c);
	return c;
}


inline HashValue ComputeHash(const char* buf, size_t len){
	size_t a;
	size_t b;
	size_t c;
	a = b = 0x9e3779b9; // arbitrary value
	c=0;
	assert(len > 0);
	int p = 0;
	while(len >= 12){
		// buf[0..p-1] has been hashed; buf[p...p+len-1] needs to be hashed
		a += *((size_t*)(buf+p));
		b += *((size_t*)(buf+p+4));
		c += *((size_t*)(buf+p+8));
		mix(a,b,c);
		len -= 12;
		p += 12;
	}
	switch(len){
		case 11: a+= buf[p+10]<<8;
			__fallthrough;
		case 10: a+= (buf[p+9]<<16);
			__fallthrough;
		case 9:  a+= (buf[p+8]<<24);
			__fallthrough;
		case 8:  b+= buf[p+7];
			__fallthrough;
		case 7:  b+= (buf[p+6]<<8);
			__fallthrough;
		case 6:  b+= (buf[p+5]<<16);
			__fallthrough;
		case 5:  b+= (buf[p+4]<<24);
			__fallthrough;
		case 4:  b+= buf[p+3];
			__fallthrough;
		case 3:  b+= (buf[p+2]<<8);
			__fallthrough;
		case 2:  b+= (buf[p+1]<<16);
			__fallthrough;
		case 1:  b+= (buf[p]<<24);
			__fallthrough;
		case 0: break;
		default: assert(false);
	}
	mix(a,b,c);
	return c;	
}