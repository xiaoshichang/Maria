#pragma once

#include <boost/asio/io_context.hpp>
#include "NetworkSession.h"

namespace Maria::Server::Native
{
    enum NetworkConnectionType : int
    {
        Tcp = 1,
        Kcp = 2,
    };

    enum SessionMessageEncoderType : int
    {
        Header = 1,
        Delim = 2,
    };

    struct NetworkInitInfo
    {
        NetworkConnectionType ConnectionType;
        SessionMessageEncoderType SessionEncoderType;
    };



    class NetworkInstance
    {
    public:
        explicit NetworkInstance(NetworkInitInfo info,
                                 OnSessionConnectedCallbackPtr onConnected,
                                 OnSessionAcceptCallbackPtr onAccept);
        virtual ~NetworkInstance();

        virtual void StartListen(const char* ip, int port) = 0;
        virtual void StopListen() = 0;
        virtual void ConnectTo(const char* ip, int port) = 0;
        virtual unsigned int GetSessionCount() = 0;

    protected:
        NetworkInitInfo init_info_;
        boost::asio::io_context* context_ = nullptr;
        OnSessionAcceptCallbackPtr on_accept_callback_;
        OnSessionConnectedCallbackPtr on_connected_callback_;

    };
}

