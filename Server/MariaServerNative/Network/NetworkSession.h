#pragma once

namespace Maria::Server::Native
{
    class NetworkSession;
    typedef void (*OnSessionAcceptCallbackPtr)(NetworkSession* session);
    typedef void (*OnSessionConnectedCallbackPtr)(NetworkSession* session, const int ec);
    typedef void (*OnSessionDisconnectCallbackPtr)(NetworkSession* session);
    typedef void (*OnSessionReceiveCallbackPtr)(const char* data, size_t length);
    typedef void (*OnSessionSendCallbackPtr)(size_t buffer_count);

    class NetworkSession
    {
    public:
        virtual ~NetworkSession() = default;

    public:
        virtual void Start() = 0;
        virtual void Send(const char* data, int length) = 0;

    protected:
        virtual void OnConnectionWrite() = 0;
        virtual void OnConnectionRead() = 0;
        virtual void OnConnectionDisconnect() = 0;

    };
}

