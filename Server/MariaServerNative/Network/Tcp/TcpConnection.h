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
        explicit TcpConnection(boost::asio::io_context* context,
                               OnConnectionReadCallback onRead,
                               OnConnectionWriteCallback onWrite,
                               OnConnectionDisconnectCallback onDisconnect);

        ~TcpConnection() override;

        void ReadAtLeast(boost::asio::streambuf& buffer, int byteCount) override;
        void ReadUntilDelim(boost::asio::streambuf& buffer) override;
        void Write(const char *data, int length, bool cache) override;
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
        void Writing();

    private:
        tcp::socket socket_;
        bool writing_ = false;
        bool closed_ = false;
        std::vector<boost::asio::streambuf::const_buffers_type> to_write_buffers_;
        std::vector<boost::asio::streambuf::const_buffers_type> writing_buffers_;
        const boost::asio::detail::transfer_all_t WRITE_RULE = boost::asio::transfer_all();
    };
}

