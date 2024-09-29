#pragma once


#include <boost/asio/streambuf.hpp>


namespace Maria::Server::Native
{
    class NetworkSession;
    class NetworkInstance;

    enum SessionMessageEncoderType : int
    {
        Header = 1,
        Delim = 2,
    };

    typedef void (*OnSessionAcceptCallbackPtr)(NetworkSession* session);
    typedef void (*OnSessionConnectedCallbackPtr)(NetworkSession* session, const int ec);
    typedef void (*OnSessionDisconnectCallbackPtr)(NetworkSession* session);
    typedef void (*OnSessionReceiveCallbackPtr)(const char* data, size_t length);

    struct NetworkMessageHeader
    {
        unsigned int MessageLength;
    };

    class NetworkSession
    {
    public:
        explicit NetworkSession(NetworkInstance* network);
        virtual ~NetworkSession() = default;

    public:
        void Send(const char* data, int length);

    public:
        void Bind(OnSessionReceiveCallbackPtr onReceive);
    protected:
        virtual void SendWithHeader(const char* header, int headerSize, const char* body, int bodySize) = 0;
        virtual void SendWithDelim(const char* body, int bodySize) = 0;
        void TryParseHeaderAndBody();


    protected:
        NetworkInstance* network_;
        OnSessionReceiveCallbackPtr on_receive_callback_ = nullptr;
        boost::asio::streambuf receive_buffer_;
        const char DELIM = '\n';

    };
}

