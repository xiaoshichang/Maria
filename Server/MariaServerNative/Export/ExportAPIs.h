#pragma once

#pragma once

#ifdef _WIN32
#  define NativeAPI __declspec( dllexport )
#else
#  define NativeAPI
#endif

#ifdef __cplusplus
extern "C"
{
#endif

NativeAPI void Logger_Init(const char* dir, const char* fileName);
NativeAPI void Logger_Debug(const char* message);
NativeAPI void Logger_Info(const char* message);
NativeAPI void Logger_Warning(const char* message);
NativeAPI void Logger_Error(const char* message);

#ifdef __cplusplus
}
#endif