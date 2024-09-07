#pragma once
#include <set>
#include <boost/asio/ip/tcp.hpp>
#include "../NetworkInstance.h"
#include "TcpSession.h"

namespace Maria::Server::Native
{
    class TcpNetworkInstance : public NetworkInstance
    {
    public:
        TcpNetworkInstance(NetworkInitInfo info,
                           OnSessionConnectedCallbackPtr onConnected,
                           OnSessionAcceptCallbackPtr onAccept);
        ~TcpNetworkInstance() override;

    public:
        void StartListen(const char* ip, int port) override;
        void StopListen() override;
        void ConnectTo(const char* ip, int port) override;
        unsigned int GetSessionCount() override;

    private:
        void Accept();

    private:
        boost::asio::ip::tcp::acceptor* acceptor_ = nullptr;
        std::set<TcpSession*> sessions_;
    };
}

