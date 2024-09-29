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
        void OnDisconnect();
        void OnReceive(const char* data, std::size_t length);
        void Update(unsigned int ts);
        const udp::endpoint& GetRemoteEndPoint() { return remote_endpoint_; }

    private:
        void SendWithHeader(const char* header, int headerSize, const char* body, int bodySize) override;
        void SendWithDelim(const char* body, int bodySize) override;
        void SendConnectRespond();
        void HandleSessionData(const char *data, std::size_t length);

    private:
        uint32_t session_id_;
        ikcpcb *kcp_;
        udp::endpoint remote_endpoint_;
        std::stringstream receive_buffer_;
        char* receive_buffer_temp_;
    };
}

