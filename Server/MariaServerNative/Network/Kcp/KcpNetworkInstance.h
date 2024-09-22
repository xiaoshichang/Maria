#pragma once

#include <boost/asio.hpp>
#include <boost/array.hpp>
#include "../NetworkInstance.h"
#include "KcpSession.h"

using boost::asio::ip::udp;

namespace Maria::Server::Native
{
    class KcpNetworkInstance : public NetworkInstance
    {
        public:
            KcpNetworkInstance(NetworkInitInfo info,
                    OnSessionAcceptCallbackPtr onAccept,
                    OnSessionConnectedCallbackPtr onConnected,
                    OnSessionDisconnectCallbackPtr onDisconnect);

            ~KcpNetworkInstance() override;

    public:
        void StartListen(const char* ip, int port) override;
        void StopListen() override;
        void ConnectTo(const char* ip, int port) override;
        unsigned int GetSessionCount() override;
        int Send(const char *buf, int len, ikcpcb *kcp, void *user);

    private:
        void Update();
        void Receive();
        void OnReceive(const boost::system::error_code& error, std::size_t bytes_transferred);


    private:

        udp::socket* udp_socket_ = nullptr;
        udp::endpoint udp_remote_endpoint_;
        char* receive_buffer_ = nullptr;
        std::unordered_map<udp::endpoint, KcpSession*> sessions_;
    };
}