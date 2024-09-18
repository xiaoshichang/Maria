#pragma once

namespace Maria::Server::Native
{
    class NetworkSession;
    typedef void (*OnSessionAcceptCallbackPtr)(NetworkSession* session);
    typedef void (*OnSessionConnectedCallbackPtr)(NetworkSession* session, const int ec);
    typedef void (*OnSessionDisconnectCallbackPtr)(NetworkSession* session);
    typedef void (*OnSessionReceiveCallbackPtr)(const char* data, size_t length);

    struct NetworkMessageHeader
    {
        int MessageLength;
    };

    class NetworkSession
    {
    public:
        virtual ~NetworkSession() = default;

    public:
        virtual void Start() = 0;
        virtual void Stop() = 0;
        virtual void Send(const char* data, int length) = 0;

    protected:
        virtual void Receive() = 0;
        virtual void OnConnectionRead(size_t byteCount) = 0;
        virtual void OnConnectionDisconnect() = 0;

    public:
        void Bind(OnSessionReceiveCallbackPtr onReceive);

    protected:
        OnSessionReceiveCallbackPtr on_receive_callback_ = nullptr;

    };
}

