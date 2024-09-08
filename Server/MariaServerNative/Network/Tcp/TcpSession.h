#pragma once
#include "../NetworkSession.h"
#include "TcpConnection.h"
#include <boost/asio.hpp>
#include <boost/asio/io_context.hpp>
#include <queue>

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
        void Stop() override;
        void Send(const char *data, int length) override;


    protected:
        void Receive() override;
        void OnConnectionWrite(size_t bufferCount) override;
        void OnConnectionRead(size_t byteCount) override;
        void OnConnectionDisconnect() override;

    public:
        tcp::socket& GetInternalSocket()
        {
            return connection_->GetInternalSocket();
        }

    private:
        TcpNetworkInstance* network_ = nullptr;
        TcpConnection* connection_ = nullptr;
        boost::asio::streambuf read_buffer_;
        std::queue<NetworkMessageHeader> headers_;
        const char DELIM = '\n';
    };
}

