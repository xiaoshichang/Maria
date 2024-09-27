#pragma once

#include <boost/asio.hpp>
#include "../NetworkInstance.h"
#include "KcpSession.h"
#include <queue>

using boost::asio::ip::udp;

namespace Maria::Server::Native
{
    struct KcpSendRequest
    {
        KcpSession *session;

    };


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
        void OnDisconnect(KcpSession* session);
        unsigned int GetSessionCount() override;
        size_t Send(KcpSession *session, const char *buf, size_t len);

    private:
        static uint32_t AllocateSessionID();
        void Update();
        void Receive();
        void OnReceive(const boost::system::error_code& error, std::size_t bytes_transferred);


    private:

        // todo: use kcp mtu size
        const int MAX_UDP_PACKET_SIZE = 65535 - 20 - 8;

        udp::socket* udp_socket_ = nullptr;
        udp::endpoint udp_remote_endpoint_;
        char* udp_receive_buffer_ = nullptr;

        std::unordered_map<udp::endpoint, KcpSession*> sessions_;

    public:
        static const char* KCP_CONNECT_REQUEST;
        static const char* KCP_CONNECT_RESPOND;
    };
}