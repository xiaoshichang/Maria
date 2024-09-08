#pragma once

#include <boost/asio.hpp>
#include <boost/function.hpp>
#include <functional>

namespace Maria::Server::Native
{
    typedef std::function<void(size_t)> OnConnectionReadCallback;
    typedef std::function<void(size_t bufferCount)> OnConnectionWriteCallback;
    typedef std::function<void(void)> OnConnectionDisconnectCallback;

    class NetworkConnection
    {
    public:
        NetworkConnection(OnConnectionReadCallback onRead,
                          OnConnectionWriteCallback onWrite,
                          OnConnectionDisconnectCallback onDisconnect);

        virtual ~NetworkConnection() = default;
        virtual void ReadAtLeast(boost::asio::streambuf& buffer, int byteCount) = 0;
        virtual void ReadUntilDelim(boost::asio::streambuf& buffer) = 0;
        virtual void Write(const char* data, int length, bool cache) = 0;
        virtual void Disconnect() = 0;

    protected:
        virtual void OnWrite(boost::system::error_code ec, std::size_t bytes_transferred) = 0;
        virtual void OnRead(boost::system::error_code ec, std::size_t bytes_transferred) = 0;
        virtual void OnDisconnect() = 0;

    protected:
        OnConnectionDisconnectCallback on_disconnect_callback_;
        OnConnectionReadCallback on_read_callback_;
        OnConnectionWriteCallback on_write_callback_;
    };
}

