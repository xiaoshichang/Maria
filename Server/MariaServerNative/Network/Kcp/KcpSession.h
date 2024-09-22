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
        explicit KcpSession(KcpNetworkInstance* network, uint32_t session_id, udp::endpoint remote);
        ~KcpSession() override;

    public:
        void Send(const char* data, int length) override;

    public:
        void OnReceive(const char* data, std::size_t length);
        void Update();

    private:
        KcpNetworkInstance* network_;
        ikcpcb *kcp_;
        std::stringstream receive_buffer_;
    };
}

