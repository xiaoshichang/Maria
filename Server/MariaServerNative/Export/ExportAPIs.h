#pragma once

#pragma once

#ifdef _WIN32
#  define NativeAPI __declspec( dllexport )
#else
#  define NativeAPI
#endif

#include "../Timer/TimerMgr.h"
#include "../Network/Tcp/TcpNetworkInstance.h"
#include "../Network/NetworkSession.h"

using namespace Maria::Server::Native;

#ifdef __cplusplus
extern "C"
{
#endif

NativeAPI void Logger_Init(const char* dir, const char* fileName);
NativeAPI void Logger_UnInit();
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

NativeAPI NetworkInstance* NetworkInstance_Init(NetworkInitInfo info,
                                                OnSessionAcceptCallbackPtr onAccept,
                                                OnSessionConnectedCallbackPtr onConnected,
                                                OnSessionDisconnectCallbackPtr onDisconnect);
NativeAPI void NetworkInstance_UnInit(NetworkInstance* network);
NativeAPI void NetworkInstance_StartListen(NetworkInstance* network, const char* ip, int port);
NativeAPI void NetworkInstance_StopListen(NetworkInstance* network);
NativeAPI void NetworkInstance_ConnectTo(NetworkInstance* network, const char* ip, int port);
NativeAPI unsigned int NetworkInstance_GetSessionCount(NetworkInstance* network);

NativeAPI void NetworkSession_Bind(NetworkSession* session, OnSessionReceiveCallbackPtr onReceive, OnSessionSendCallbackPtr onSend);
NativeAPI void NetworkSession_Send(NetworkSession* session, const char* data, int length);
NativeAPI void NetworkSession_Stop(NetworkSession* session);
NativeAPI void NetworkSession_ConsumeReceiveBuffer(NetworkSession* session, int count);

#ifdef __cplusplus
}
#endif