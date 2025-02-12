#pragma once
#include "../NetworkSession.h"
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

        void OnDisconnect();

    public:
        void Start() override;
        void Stop() override;
        void Send(const char* data, int length) override;
        void ConsumeReceiveBuffer(int count) override;

    protected:
        void DoSend();
        void OnSend(boost::system::error_code ec, std::size_t bytes_transferred, int buffer_count) override;
        void Receive() override;
        void OnReceive(boost::system::error_code ec, std::size_t bytes_transferred) override;

    public:
        tcp::socket& GetInternalSocket()
        {
            return socket_;
        }

    private:
        tcp::socket socket_;

        bool closed_ = false;
        bool sending_ = false;
        boost::asio::streambuf receive_buffer_;
        std::vector<boost::asio::const_buffer> send_queue_;
        const boost::asio::detail::transfer_all_t WRITE_RULE = boost::asio::transfer_all();
    };
}

