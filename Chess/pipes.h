/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include <cassert>
#include <iostream>
#include <windows.h>

#define BUF_SIZE 1024

// The Pipe Functions in the win32 API are more elaborate and have far more
// elaborate options than we really require.  This class hides the options
// that are not used in addition to giving an OO abstraction to named pipes.
class Pipe {
private:
	HANDLE pipe;
	BOOL isInitialized;

public:
	Pipe() {		
		pipe = INVALID_HANDLE_VALUE;
		isInitialized = false;
	}

	HANDLE getHandle() {
		return pipe;
	}

	// NamedPipe server functions
	BOOL myCreateNamedPipe(LPCTSTR name, DWORD mode) {
		assert(!isInitialized && "Re-initialization of pipe");

		pipe = CreateNamedPipe(
			name,
			mode,
			PIPE_WAIT,
			1,
			BUF_SIZE,
			BUF_SIZE,
			INFINITE,
			NULL
			);
		if(pipe == NULL) {
			*GetChessErrorStream() << "CreateNamedPipe of " << name << "failed: " << GetLastError() << std::endl;
			return false;
		}
		isInitialized = true;
		return true;
	}

	BOOL myConnectNamedPipe() {
		assert(isInitialized && "Attempting to wait on uninitalized pipe");
		
		BOOL connected = ConnectNamedPipe(pipe, NULL);
		if(!connected && GetLastError() != ERROR_PIPE_CONNECTED) {
			*GetChessErrorStream() << "ConnectNamedPipe failed: " << GetLastError() << std::endl;
			return false;
		}
		return true;
	}

	// NamedPipe client functions
	BOOL myCreateFile(LPCTSTR name, DWORD desiredAccess, DWORD flags) {
		assert(!isInitialized && "Re-initialization of pipe");

		pipe = CreateFile(
			name,
			desiredAccess,
			0,
			NULL,
			OPEN_EXISTING,
			flags,
			NULL
			);
		if(pipe == INVALID_HANDLE_VALUE) {
			*GetChessErrorStream() << "CreateFile of " << name << " failed: " << GetLastError() << std::endl;
			return false;
		}
		isInitialized = true;
		return true;
	}

	BOOL myReadFile(char *msg, DWORD len, OVERLAPPED *OL = NULL) {
		DWORD readBytes;
		BOOL res;

		res = ReadFile(pipe, msg, len, &readBytes, OL);
		if(OL != NULL) {
			// Leave error reporting to caller during Async I/O
			return res;
		}
		else if(res == false || readBytes != len) {
			*GetChessErrorStream() << "ERROR: Read failed " << GetLastError() << std::endl;
			return false;
		}

		//*GetChessErrorStream() << "Read " << msg << endl;
		return true;
	}

	BOOL myWriteFile(char *msg, DWORD len, OVERLAPPED *OL = NULL) {
		DWORD writtenBytes;
		BOOL res;

		res = WriteFile(pipe, msg, len, &writtenBytes, OL);
		if(OL != NULL) {
			// Leave error reporting to caller during Async I/O
			return res;
		}
		else if(res == false || writtenBytes != len) {
			*GetChessErrorStream() << "ERROR: Write failed " << GetLastError() << std::endl;
			return false;
		}

		//*GetChessErrorStream() << "Wrote " << msg << endl;
		return true;
	}
};