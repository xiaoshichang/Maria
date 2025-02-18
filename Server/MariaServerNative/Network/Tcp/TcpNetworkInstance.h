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
                           OnSessionAcceptCallbackPtr onAccept,
                           OnSessionConnectedCallbackPtr onConnected,
                           OnSessionDisconnectCallbackPtr onDisconnect);
        ~TcpNetworkInstance() override;

    public:
        void Start(const char* ip, int port) override;
        void Stop() override;
        void ConnectTo(const char* ip, int port) override;
        unsigned int GetSessionCount() override;
        void OnDisconnect(TcpSession* session);

    private:
        void Accept();


    private:
        boost::asio::ip::tcp::acceptor* acceptor_ = nullptr;
        std::set<TcpSession*> sessions_;

    };
}

