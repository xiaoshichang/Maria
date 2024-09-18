#pragma once

#include <boost/asio.hpp>
#include <boost/function.hpp>
#include <functional>

namespace Maria::Server::Native
{
    typedef std::function<void(size_t)> OnConnectionReadCallback;
    typedef std::function<void(void)> OnConnectionDisconnectCallback;

    class NetworkConnection
    {
    public:
        NetworkConnection(OnConnectionReadCallback onRead,
                          OnConnectionDisconnectCallback onDisconnect);

        virtual ~NetworkConnection() = default;
        virtual void ReadAtLeast(boost::asio::streambuf& buffer, int byteCount) = 0;
        virtual void ReadUntilDelim(boost::asio::streambuf& buffer) = 0;
        virtual void WriteWithHeader(const char* header, int headerSize, const char* body, int bodySize) = 0;
        virtual void WriteWithDelim(const char* body, int bodySize, char delim) = 0;
        virtual void Disconnect() = 0;

    protected:
        virtual void OnWrite(boost::system::error_code ec, std::size_t bytes_transferred) = 0;
        virtual void OnRead(boost::system::error_code ec, std::size_t bytes_transferred) = 0;
        virtual void OnDisconnect() = 0;

    protected:
        OnConnectionDisconnectCallback on_disconnect_callback_;
        OnConnectionReadCallback on_read_callback_;
    };
}

