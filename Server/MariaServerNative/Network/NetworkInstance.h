#pragma once
#include <set>
#include <boost/asio/io_context.hpp>
#include <boost/asio/completion_condition.hpp>
#include "NetworkSession.h"

namespace Maria::Server::Native
{
    enum NetworkConnectionType : int
    {
        Tcp = 1,
        Kcp = 2,
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
                                 OnSessionAcceptCallbackPtr onAccept,
                                 OnSessionConnectedCallbackPtr onConnected,
                                 OnSessionDisconnectCallbackPtr onDisconnect);
        virtual ~NetworkInstance();

        virtual void StartListen(const char* ip, int port) = 0;
        virtual void StopListen() = 0;
        virtual void ConnectTo(const char* ip, int port) = 0;
        virtual unsigned int GetSessionCount() = 0;

        NetworkInitInfo& GetNetworkInfo() { return init_info_; }

    protected:
        NetworkInitInfo init_info_;
        OnSessionAcceptCallbackPtr on_accept_callback_ = nullptr;
        OnSessionConnectedCallbackPtr on_connected_callback_ = nullptr;
        OnSessionDisconnectCallbackPtr on_disconnect_callback_ = nullptr;


    };
}

