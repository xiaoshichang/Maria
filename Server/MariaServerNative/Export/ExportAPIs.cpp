
#include "ExportAPIs.h"
#include "../Logger/Logger.h"
#include "../IOContext/IOContext.h"

using namespace Maria::Server::Native;

void Logger_Init(const char* dir, const char* fileName)
{
    Logger::Init(dir, fileName);
}

void Logger_Debug(const char* message)
{
    Logger::Debug(message, LogTag::Managed);
}

void Logger_Info(const char* message)
{
    Logger::Info(message, LogTag::Managed);
}

void Logger_Warning(const char* message)
{
    Logger::Warning(message, LogTag::Managed);
}

void Logger_Error(const char* message)
{
    Logger::Error(message, LogTag::Managed);
}

TimerID Timer_AddTimer(unsigned int delay, TimeoutCallback callback)
{
    return TimerMgr::AddTimer(delay, callback);
}

TimerID Timer_AddRepeatTimer(unsigned int delay, unsigned int interval, TimeoutCallback callback)
{
    return TimerMgr::AddRepeatTimer(delay, interval, callback);
}

bool Timer_CancelTimer(TimerID tid)
{
    return TimerMgr::CancelTimer(tid);
}

unsigned int Timer_GetTimersCount()
{
    return TimerMgr::GetTimersCount();
}

void IOContext_Init()
{
    IOContext::Init();
}

void IOContext_Run()
{
    IOContext::Run();
}

void IOContext_UnInit()
{
    IOContext::UnInit();
}
