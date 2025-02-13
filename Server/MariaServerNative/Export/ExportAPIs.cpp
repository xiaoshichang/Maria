
#include "ExportAPIs.h"
#include "../Logger/Logger.h"
#include "../IOContext/IOContext.h"
#include "../Timer/TimerMgr.h"


using namespace Maria::Server::Native;

void Logger_Init(const char* dir, const char* fileName)
{
    Logger::Init(dir, fileName);
}

void Logger_UnInit()
{
    Logger::Finalize();
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

void TimerMgr_Init()
{
    auto ioContext = IOContext::Get();
    if (ioContext == nullptr)
    {
        Logger::Error("IOContext must be initialized first.");
        return;
    }
    TimerMgr::Init(ioContext);
}

void TimerMgr_UnInit()
{
    TimerMgr::UnInit();
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

void IOContext_Stop()
{
    IOContext::Stop();
}

void IOContext_UnInit()
{
    IOContext::UnInit();
}

NetworkInstance* NetworkInstance_Init(NetworkInitInfo info,
                                      OnSessionAcceptCallbackPtr onAccept,
                                      OnSessionConnectedCallbackPtr onConnected,
                                      OnSessionDisconnectCallbackPtr onDisconnect)
{
    if (info.ConnectionType == NetworkConnectionType::Tcp)
    {
        return new TcpNetworkInstance(info, onAccept, onConnected, onDisconnect);
    }
    else
    {
        return nullptr;
    }
}

void NetworkInstance_UnInit(NetworkInstance* network)
{
    delete network;
}

void NetworkInstance_Start(NetworkInstance* network, const char* ip, int port)
{
    network->Start(ip, port);
}

void NetworkInstance_Stop(NetworkInstance* network)
{
    network->Stop();
}

void NetworkInstance_ConnectTo(NetworkInstance* network, const char* ip, int port)
{
    network->ConnectTo(ip, port);
}

unsigned int NetworkInstance_GetSessionCount(NetworkInstance* network)
{
    return network->GetSessionCount();
}

void NetworkSession_Bind(NetworkSession* session, OnSessionReceiveCallbackPtr onReceive, OnSessionSendCallbackPtr onSend)
{
    session->Bind(onReceive, onSend);
}

void NetworkSession_Send(NetworkSession* session, const char* data, int length)
{
    session->Send(data, length);
}

void NetworkSession_Stop(NetworkSession* session)
{
    session->Stop();
}

void NetworkSession_ConsumeReceiveBuffer(NetworkSession* session, int count)
{
    session->ConsumeReceiveBuffer(count);
}