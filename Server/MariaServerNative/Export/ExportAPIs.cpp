
#include "ExportAPIs.h"
#include "../Logger/Logger.h"

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