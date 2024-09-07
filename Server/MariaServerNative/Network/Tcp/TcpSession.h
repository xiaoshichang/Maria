#pragma once
#include "../NetworkSession.h"
#include "TcpConnection.h"
#include <boost/asio.hpp>
#include <boost/asio/io_context.hpp>

using boost::asio::ip::tcp;

namespace Maria::Server::Native
{
    class TcpNetworkInstance;

    class TcpSession : public NetworkSession
    {
    public:
        explicit TcpSession(TcpNetworkInstance* network, boost::asio::io_context* context);
        ~TcpSession() override;

    public:
        void Start() override;
        void Send(const char *data, int length) override;

    protected:
        void OnConnectionWrite() override;
        void OnConnectionRead() override;
        void OnConnectionDisconnect() override;

    public:
        tcp::socket& GetInternalSocket()
        {
            return connection_->GetInternalSocket();
        }

    private:
        const TcpNetworkInstance* network_ = nullptr;
        TcpConnection* connection_ = nullptr;
    };
}

