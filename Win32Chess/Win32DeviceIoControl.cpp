/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

  #include "Win32Base.h"
  #include "ChessAssert.h"
  #include "Chess.h"
#include "Win32Wrappers.h"
#include "Win32WrapperAPI.h"
#include "Win32SyncManager.h"
using namespace std;

//#define ONE_THREAD_PER_REQUEST_MODEL
#define ONE_THREAD_PER_HANDLE_MODEL

#ifdef ONE_THREAD_PER_HANDLE_MODEL
map<HANDLE, HANDLE> rootHandle;
#endif


typedef struct {
	// Arguments for the call
	HANDLE hDevice;
	DWORD dwIoControlCode;
	LPVOID lpInBuffer;
	DWORD nInBufferSize;
	LPVOID lpOutBuffer;
	DWORD nOutBufferSize;
	LPDWORD lpBytesReturned;
	LPOVERLAPPED lpOverlapped;

	// Thread ID on behalf of which this call is being made
	Task id;
#ifdef ONE_THREAD_PER_HANDLE_MODEL
	// Event to signal completion
	HANDLE hEvent;
#endif

	// Return values of the call
	BOOL retVal;
	DWORD lastError;
} DeviceIoControlArgs;

#ifdef ONE_THREAD_PER_HANDLE_MODEL

typedef pair<HANDLE, DWORD> DioQParam;

class DeviceIoQueue {
private:
	DioQParam param;

	queue<DeviceIoControlArgs *> qRequests;
	HANDLE mutex;
	
public:
	HANDLE requestSignal;
	DeviceIoControlArgs *curr;

	DeviceIoQueue(DioQParam param_t) {
		param = param_t;

		mutex = CreateMutexW(NULL, false, NULL);
		assert(mutex != NULL || !"Cannot create mutex");

		requestSignal = CreateEventW(NULL, false, false, NULL);
		assert(requestSignal != NULL || !"Cannot create Event");
	}

	void push(DeviceIoControlArgs *request) {
		DWORD locked = WaitForSingleObject(mutex, INFINITE);
		assert(locked == WAIT_OBJECT_0);

		qRequests.push(request);
		
		if(qRequests.size() == 1)
			SetEvent(requestSignal);

		ReleaseMutex(mutex);
	}

	DWORD pop(DeviceIoControlArgs **request) {
		DWORD res;

		DWORD locked = WaitForSingleObject(mutex, INFINITE);
		assert(locked == WAIT_OBJECT_0);

		if(qRequests.size() == 0)
			res = -1;
		else {
			*request = qRequests.front();
			qRequests.pop();
			res = 0;
		}
		
		ReleaseMutex(mutex);
		return res;
	}

	size_t size() const{
		return qRequests.size();
	}
};

DWORD WINAPI RunThread(LPVOID p) {
	DeviceIoQueue *IORequests = (DeviceIoQueue *) p;

	while(true) {
		DeviceIoControlArgs *args = NULL;

		ResetEvent(IORequests->requestSignal);

		DWORD reqPresent = IORequests->pop(&args);
		if(reqPresent == -1) {
			//fprintf(stderr, "(%d) Waiting for new request signal\n", GetCurrentThreadId());
			fflush(stderr);
			WaitForSingleObject(IORequests->requestSignal, INFINITE);
			continue;
		}
		assert(args != NULL);
		if(args == NULL){
			continue;
		}
		IORequests->curr = args;

		//fprintf(stderr, "DIO called: %p (ID: %d, in: %d, out: %d)\n", args->hDevice, args->id, args->nInBufferSize, args->nOutBufferSize);
		//fflush(stderr);
		args->retVal = DeviceIoControl(args->hDevice,
										args->dwIoControlCode,
										args->lpInBuffer,
										args->nInBufferSize,
										args->lpOutBuffer,
										args->nOutBufferSize,
										args->lpBytesReturned,
										args->lpOverlapped);
		args->lastError = GetLastError();

		IORequests->curr = NULL;
		//fprintf(stderr, "Call returned: %p (ID: %d, in: %d, out: %d)\n", args->hDevice, args->id, args->nInBufferSize, args->nOutBufferSize);
		//fflush(stderr);
		SetEvent(args->hEvent);
	}
	return 0;
}

map<DioQParam, DeviceIoQueue *> handleToQueue;
#endif

#ifdef ONE_THREAD_PER_REQUEST_MODEL
DWORD WINAPI myAsyncDeviceIoControl(LPVOID p) {
	DeviceIoControlArgs *args = (DeviceIoControlArgs *) p;

	fprintf(stderr, "DIO called: %p (ID: %d, in: %d, out: %d)\n", args->hDevice, args->id, args->nInBufferSize, args->nOutBufferSize);
	fflush(stderr);
	args->retVal = DeviceIoControl(args->hDevice,
									args->dwIoControlCode,
									args->lpInBuffer,
									args->nInBufferSize,
									args->lpOutBuffer,
									args->nOutBufferSize,
									args->lpBytesReturned,
									args->lpOverlapped);
	fprintf(stderr, "Call returned: %p (ID: %d, in: %d, out: %d)\n", args->hDevice, args->id, args->nInBufferSize, args->nOutBufferSize);
	fflush(stderr);
	args->lastError = GetLastError();
	return args->retVal;
}
#endif


/*	DWORD GetQuiescenceWaitTime() {
		static size_t lastWaitStep = -1;
		if(InReplayMode())
			return INFINITE;
		else if(topIndex != lastWaitStep) {
			lastWaitStep = topIndex;
			return 1000;
		}
		else
			return 0;
	}

DWORD Chess::GetQuiescenceWaitTime() {
	if(firstTest)
		return 2000;
	else if(waitingForShutDown)
		return 2000;
	else
		return currExecution->GetQuiescenceWaitTime();
}
*/

DWORD GetQuiescenceWaitTime(){
	return 2000;
}

BOOL WINAPI __wrapper_DeviceIoControl(
									  HANDLE hDevice, 
									  DWORD dwIoControlCode, 
									  LPVOID lpInBuffer, 
									  DWORD nInBufferSize, 
									  LPVOID lpOutBuffer, 
									  DWORD nOutBufferSize, 
									  LPDWORD lpBytesReturned, 
									  LPOVERLAPPED lpOverlapped)
{

	BOOL isRead = TRUE; //(nOutBufferSize > 0);
	BOOL isWrite = TRUE; //(nInBufferSize > 0);

	//if(!isRead && !isWrite) {
	//	// Sometimes the driver calls DIO with inBufferSize = outBufferSize = 0.
	//	// Conservatively handle that case as a write
	//	isRead = FALSE;
	//	isWrite = TRUE;
	//}

	if(isWrite)
		Chess::SyncVarAccess(-2, SVOP::DIO_SEND);

	if(isWrite && !isRead) {
		return DeviceIoControl(hDevice, dwIoControlCode, lpInBuffer, nInBufferSize, lpOutBuffer, nOutBufferSize, lpBytesReturned, lpOverlapped);
	}

	// Snippet to call Real_DeviceIoControl in a new thread
	DeviceIoControlArgs *args = (DeviceIoControlArgs *) malloc(sizeof(DeviceIoControlArgs));
	args->dwIoControlCode = dwIoControlCode;
	args->hDevice = hDevice;
	args->lpBytesReturned = lpBytesReturned;
	args->lpInBuffer = lpInBuffer;
	args->lpOutBuffer = lpOutBuffer;
	args->lpOverlapped = lpOverlapped;
	args->nInBufferSize = nInBufferSize;
	args->nOutBufferSize = nOutBufferSize;
	args->id = Chess::GetCurrentTid();
	
//	ChessWaitForGo();

#ifdef ONE_THREAD_PER_HANDLE_MODEL
	args->hEvent = CreateEventW(NULL, false, false, NULL);
	
	HANDLE root = hDevice;
	if(rootHandle.count(hDevice) != 0)
		root = rootHandle[hDevice];

	DioQParam param(root, dwIoControlCode);

	if(handleToQueue.count(param) == 0) {
		DeviceIoQueue *DioQ = new DeviceIoQueue(param);
		//fprintf(stderr, "Creating a separate queue for %p\n", root);
		CreateThread(NULL, 0, RunThread, (LPVOID) DioQ, 0, 0);

		handleToQueue[param] = DioQ;
	}

	DeviceIoQueue *DioQ = handleToQueue[param];
	DioQ->push(args);

	HANDLE externalEvent = args->hEvent;
#endif
	
#ifdef ONE_THREAD_PER_REQUEST_MODEL
	DWORD childId;
	HANDLE externalEvent = CreateThread(NULL, 0, myAsyncDeviceIoControl, (LPVOID) args, 0, &childId);
#endif
	
	// Hack to handle the external event
	Task tid = Chess::GetCurrentTid();
	// Chess::extEventBlock(tid);
	
	while(true){
		DWORD waitTime = GetQuiescenceWaitTime();
		/*fprintf(stderr, "(%d) Waiting for %u\n", tid, waitTime);
		fflush(stderr);*/

		Chess::SyncVarAccess(-2, SVOP::DIO_RECEIVE);
		DWORD retVal = WaitForSingleObject(externalEvent, waitTime);

		/*fprintf(stderr, "Wait finished\n", waitTime);
		fflush(stderr);*/
		switch(retVal){
			case WAIT_OBJECT_0 : 
				//Chess::ThreadReadWriteSyncVar(GetWin32SyncManager()->GetSyncVarFromHandle(externalEvent));
				//Chess::OnNewExternalEvent();
				//Chess::SyncVarAccess(-2, SVOP::RWVAR_READWRITE);
				goto wait_over;
			
			case WAIT_TIMEOUT :
				Chess::LocalBacktrack();
				break;	
			
			case WAIT_ABANDONED :
				assert(!"WAIT_ABANDONED case not implemented in CHESS"); //really dont know what we should do here
				return retVal;

			default:
				// error condition
				fprintf(stderr, "WaitForSingleObject failed with code %d\n", GetLastError());
				break;
		}
	}

wait_over:
	// Chess::extEventUnblock(tid);
	BOOL retVal = args->retVal;
	if(! retVal)
		SetLastError(args->lastError);
	
	CloseHandle(externalEvent);
	free(args);
	return retVal;

	// Snippet to test async I/O
	//OVERLAPPED overlapped;
	//HANDLE newEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
	//overlapped.hEvent = (HANDLE) ((size_t)newEvent | 1);
	//
	//BOOL res = Real_DeviceIoControl(hDevice, dwIoControlCode, lpInBuffer, nInBufferSize, lpOutBuffer, nOutBufferSize, lpBytesReturned, &overlapped);
	//if(!res && GetLastError() == ERROR_IO_PENDING) {
	//	WaitForSingleObject(overlapped.hEvent, INFINITE);
	//	fprintf(stderr, "Overlapped IO is happening...\n");
	//}
	//else if(!res) {
	//	fprintf(stderr, "DeviceIoControl Error: %d.  Reverting to non-overlapped IO\n", GetLastError());
	//	res = Real_DeviceIoControl(hDevice, dwIoControlCode, lpInBuffer, nInBufferSize, lpOutBuffer, nOutBufferSize, lpBytesReturned, lpOverlapped);
	//}
	//CloseHandle(newEvent);
	//return res;
}



stdext::hash_map<Task, Task> GetSymmetricTasks() {
	stdext::hash_map<Task, Task> rename;
	map<DioQParam, DeviceIoQueue *>::iterator iter;

	for(iter = handleToQueue.begin(); iter != handleToQueue.end(); ++iter) {
		DeviceIoQueue *currentQueue = (*iter).second;
		
		vector<Task> callOrder, sorted;

		if(currentQueue->curr == NULL) {
			assert(currentQueue->size() == 0);
			continue;
		}

		callOrder.push_back(currentQueue->curr->id);
		sorted.push_back(currentQueue->curr->id);

		size_t numRequests = currentQueue->size();
		for(size_t i = 0; i < numRequests; ++i) {
			DeviceIoControlArgs *req;

			DWORD res = currentQueue->pop(&req);
			assert(res == 0);

			callOrder.push_back(req->id);
			sorted.push_back(req->id);

			currentQueue->push(req);
		}

		sort(sorted.begin(), sorted.end());

		for(size_t i = 0; i < callOrder.size(); ++i) {
			assert(rename.count(callOrder[i]) == 0); // Not renamed earlier

			if(callOrder[i] != sorted[i])
				rename[callOrder[i]] = sorted[i];
		}
	}

	//std::cerr << "Size of rename is " << rename.size() << std::endl;
	//for(stdext::hash_map<Task, Task>::iterator r_iter = rename.begin(); r_iter != rename.end(); ++r_iter)
	//	std::cerr << "Changing " << r_iter->first << " to " << r_iter->second << std::endl;
	//fflush(stderr);
	return rename;
}