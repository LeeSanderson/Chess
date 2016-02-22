/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>

#define BUFFER_SIZE 1
ULONG QueueSize;
ULONG QueueStartOffset;

CONDITION_VARIABLE BufferNotEmpty;
CONDITION_VARIABLE BufferNotFull;
CRITICAL_SECTION   BufferLock;

BOOL StopRequested;

DWORD WINAPI ProducerThreadProc (PVOID p)
{
  UNREFERENCED_PARAMETER(p);

    while (true)
    {
        // Produce a new item.

        EnterCriticalSection (&BufferLock);

        while (QueueSize == BUFFER_SIZE && StopRequested == FALSE)
        {
            // Buffer is full - sleep so consumers can get items.
            SleepConditionVariableCS (
                &BufferNotFull, &BufferLock, INFINITE);
        }

        if (StopRequested == TRUE)
        {
            LeaveCriticalSection (&BufferLock);
            break;
        }

        // Insert the item at the end of the queue and increment size.

        QueueSize++;

        LeaveCriticalSection (&BufferLock);

        // If a consumer is waiting, wake it.

        WakeConditionVariable (&BufferNotEmpty);
    }

    return 0;
}

DWORD WINAPI ConsumerThreadProc (PVOID p)
{
  UNREFERENCED_PARAMETER(p);

    while (true)
    {
        EnterCriticalSection (&BufferLock);

        while (QueueSize == 0 && StopRequested == FALSE)
        {
            // Buffer is empty - sleep so producers can create items.
            SleepConditionVariableCS (
                &BufferNotEmpty, &BufferLock, INFINITE);
        }

        if (StopRequested == TRUE && QueueSize == 0)
        {
            LeaveCriticalSection (&BufferLock);
            break;
        }

        // Consume the first available item.

        QueueSize--;
        QueueStartOffset++;

        if (QueueStartOffset == BUFFER_SIZE)
        {
            QueueStartOffset = 0;
        }

        LeaveCriticalSection (&BufferLock);

        // If a producer is waiting, wake it.

        WakeConditionVariable (&BufferNotFull);
    }

    return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun()
{

    InitializeConditionVariable (&BufferNotEmpty);
    InitializeConditionVariable (&BufferNotFull);
    InitializeCriticalSection (&BufferLock);
    StopRequested = FALSE;
    QueueSize = 0;
    QueueStartOffset = 0;

    DWORD id;
    HANDLE hProducer1 = CreateThread (
        NULL, 0, ProducerThreadProc, (PVOID)1, 0, &id);
    HANDLE hConsumer1 = CreateThread (
        NULL, 0, ConsumerThreadProc, (PVOID)1, 0, &id);
    HANDLE hConsumer2 = CreateThread (
        NULL, 0, ConsumerThreadProc, (PVOID)2, 0, &id);

    EnterCriticalSection (&BufferLock);
    StopRequested = TRUE;
    LeaveCriticalSection (&BufferLock);

    WakeAllConditionVariable (&BufferNotFull);
    WakeAllConditionVariable (&BufferNotEmpty);

    WaitForSingleObject (hProducer1, INFINITE);
    WaitForSingleObject (hConsumer1, INFINITE);
    WaitForSingleObject (hConsumer2, INFINITE);

    DeleteCriticalSection(&BufferLock);
    return 0;
}
