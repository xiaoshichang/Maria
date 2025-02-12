#pragma once

#include <boost/asio.hpp>
#include <boost/asio/streambuf.hpp>


namespace Maria::Server::Native
{
    class NetworkSession;
    class NetworkInstance;

    typedef void (*OnSessionAcceptCallbackPtr)(NetworkSession* session);
    typedef void (*OnSessionConnectedCallbackPtr)(NetworkSession* session, const int ec);
    typedef void (*OnSessionDisconnectCallbackPtr)(NetworkSession* session);
    typedef void (*OnSessionReceiveCallbackPtr)(const char* data, size_t length);
    typedef void (*OnSessionSendCallbackPtr)(size_t bufferCount);

    class NetworkSession
    {
    public:
        explicit NetworkSession(NetworkInstance* network);
        virtual ~NetworkSession() = default;

    public:
        virtual void Start() = 0;
        virtual void Stop() = 0;
        virtual void Send(const char* data, int length) = 0;
        virtual void ConsumeReceiveBuffer(int count) = 0;
    protected:
        virtual void Receive() = 0;
        virtual void OnReceive(boost::system::error_code ec, std::size_t bytes_transferred) = 0;
        virtual void OnSend(boost::system::error_code ec, std::size_t bytes_transferred, int buffer_count) = 0;

    public:
        void Bind(OnSessionReceiveCallbackPtr onReceive, OnSessionSendCallbackPtr onSend);

    protected:
        NetworkInstance* network_;
        OnSessionReceiveCallbackPtr on_receive_callback_ = nullptr;
        OnSessionSendCallbackPtr on_send_callback_ = nullptr;
    };
}

