#pragma once

#pragma once

#ifdef _WIN32
#  define NativeAPI __declspec( dllexport )
#else
#  define NativeAPI
#endif

#include "../Timer/TimerMgr.h"

using namespace Maria::Server::Native;

#ifdef __cplusplus
extern "C"
{
#endif

NativeAPI void Logger_Init(const char* dir, const char* fileName);
NativeAPI void Logger_Debug(const char* message);
NativeAPI void Logger_Info(const char* message);
NativeAPI void Logger_Warning(const char* message);
NativeAPI void Logger_Error(const char* message);

NativeAPI TimerID Timer_AddTimer(unsigned int delay, TimeoutCallback callback);
NativeAPI TimerID Timer_AddRepeatTimer(unsigned int delay, unsigned int interval, TimeoutCallback callback);
NativeAPI bool Timer_CancelTimer(TimerID);
NativeAPI unsigned int Timer_GetTimersCount();

NativeAPI void IOContext_Init();
NativeAPI void IOContext_Run();
NativeAPI void IOContext_UnInit();


#ifdef __cplusplus
}
#endif