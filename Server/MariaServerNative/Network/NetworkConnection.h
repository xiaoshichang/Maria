#pragma once

#include <boost/asio.hpp>
#include <boost/function.hpp>

namespace Maria::Server::Native
{
    class NetworkConnection
    {
    public:
        virtual ~NetworkConnection() = default;
        virtual void StartReading() = 0;
        virtual void Write(const char* data, int length) = 0;
        virtual void Disconnect() = 0;

    protected:
        virtual void OnWrite(boost::system::error_code ec, std::size_t bytes_transferred) = 0;
        virtual void OnRead(boost::system::error_code ec, std::size_t bytes_transferred) = 0;
        virtual void OnDisconnect() = 0;


    };
}

