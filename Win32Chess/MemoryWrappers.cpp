/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Win32Base.h"
#include "ChessAssert.h"
#include "IChessWrapper.h"

stdext::hash_map<void *, size_t> ptrSize;

void HeapStatusReset() {
	/*fprintf(stderr, "%d pointers stored\n", ptrSize.size());
	fflush(stderr);*/
}

void malloc_plugin(void *ptr, size_t size) {
	assert(ptrSize.find(ptr) == ptrSize.end());
	ptrSize[ptr] = size;
}

void free_plugin(void *ptr) {
	if(ptrSize.find(ptr) == ptrSize.end())
		fprintf(stderr, "Double free or garbage free");

	memset(ptr, 0xCCCCCCCC, ptrSize[ptr]);
	ptrSize.erase(ptrSize.find(ptr));
}

void realloc_plugin(void *ptr, size_t size) {
	// For now, just update the size.
	// Later, try to cleanup memory old, freed memory based on return value
	if(ptrSize.find(ptr) == ptrSize.end())
		fprintf(stderr, "Realloc of a garbage pointer");

	ptrSize[ptr] = size;
}

void *__wrapper_malloc(size_t _Size) {
	void *res = malloc(_Size);
	ChessErrorSentry sentry;
	malloc_plugin(res, _Size);

	return res;
}

void *__wrapper_realloc(void *_Memory, size_t _Size) {
	void *res = realloc(_Memory, _Size);
	ChessErrorSentry sentry;
	realloc_plugin(_Memory, _Size);

	return res;
}

void __wrapper_free(void *_Memory) {
	free_plugin(_Memory);
	free(_Memory);	
	return;
}

void *__wrapper_HeapAlloc(HANDLE hHeap, DWORD dwFlags, SIZE_T _Size) {
	void *res = HeapAlloc(hHeap, dwFlags, _Size);
	ChessErrorSentry sentry;
	malloc_plugin(res, (size_t) _Size);

	return res;
}

void *__wrapper_HeapReAlloc(HANDLE hHeap, DWORD dwFlags, void *_Memory, SIZE_T _Size) {
	void *res = HeapReAlloc(hHeap, dwFlags, _Memory, _Size);
	ChessErrorSentry sentry;
	realloc_plugin(_Memory, (size_t) _Size);

	return res;
}

BOOL __wrapper_HeapFree(HANDLE hHeap, DWORD dwFlags, void *_Memory) {
	free_plugin(_Memory);
	return HeapFree(hHeap, dwFlags, _Memory);
}