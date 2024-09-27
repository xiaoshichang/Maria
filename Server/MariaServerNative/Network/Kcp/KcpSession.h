#pragma once
#include <sstream>
#include <boost/asio.hpp>
#include "../NetworkSession.h"
#include "../../3rd/kcp/ikcp.h"

using boost::asio::ip::udp;

namespace Maria::Server::Native
{
    class KcpNetworkInstance;

    class KcpSession : public NetworkSession
    {
    public:
        explicit KcpSession(KcpNetworkInstance* network, uint32_t session_id, udp::endpoint  remote);
        ~KcpSession() override;

    public:
        void Send(const char* data, int length) override;

    public:
        void OnDisconnect();
        void OnReceive(const char* data, std::size_t length);
        void Update();
        const udp::endpoint& GetRemoteEndPoint() { return remote_endpoint_; }

    private:
        void SendWithHeader(const char* header, int headerSize, const char* body, int bodySize);
        void SendConnectRespond();
        void HandleSessionData(const char *data, std::size_t length);

    private:
        KcpNetworkInstance* network_;
        uint32_t session_id_;
        ikcpcb *kcp_;
        udp::endpoint remote_endpoint_;
        std::stringstream receive_buffer_;
        char* receive_buffer_temp_;
    };
}

