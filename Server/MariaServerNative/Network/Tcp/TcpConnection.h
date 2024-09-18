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
                               OnConnectionDisconnectCallback onDisconnect);

        ~TcpConnection() override;

        void ReadAtLeast(boost::asio::streambuf& buffer, int byteCount) override;
        void ReadUntilDelim(boost::asio::streambuf& buffer) override;
        void WriteWithHeader(const char* header, int headerSize, const char* body, int bodySize) override;
        void WriteWithDelim(const char* body, int bodySize, char delim) override;
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
        void DoWrite();
        boost::asio::streambuf& GetBufferToWrite();
        void SwitchBufferToWrite();

    private:
        tcp::socket socket_;
        bool closed_ = false;
        bool writing_ = false;

        bool write_to_1_ = true;
        boost::asio::streambuf write_buffer_1_;
        boost::asio::streambuf write_buffer_2_;

        const boost::asio::detail::transfer_all_t WRITE_RULE = boost::asio::transfer_all();
    };
}

