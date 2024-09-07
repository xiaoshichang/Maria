#pragma once
#include "../NetworkConnection.h"
#include <boost/asio.hpp>

using boost::asio::ip::tcp;

namespace Maria::Server::Native
{
    class TcpNetworkInstance;

    class TcpConnection : public NetworkConnection
    {
    public:
        explicit TcpConnection(TcpNetworkInstance* network, boost::asio::io_context* context);
        ~TcpConnection() override;

        void StartReading() override;
        void Write(const char *data, int length) override;
        void Disconnect() override;

    protected:
        void OnWrite(boost::system::error_code ec, std::size_t bytes_transferred) override;
        void OnRead(boost::system::error_code ec, std::size_t bytes_transferred) override;
        void OnDisconnect() override;

    public:
        tcp::socket& GetInternalSocket()
        {
            return socket_;
        }

    private:
        TcpNetworkInstance* network_;
        tcp::socket socket_;
    };
}

