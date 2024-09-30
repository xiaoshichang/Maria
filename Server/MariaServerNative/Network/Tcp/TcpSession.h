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
        void Start();
        void Stop();
        void OnDisconnect();

    private:
        void Receive();

    private:
        void ReadAtLeast(int byteCount);
        void ReadUntilDelim();
        void OnReceive(boost::system::error_code ec, std::size_t bytes_transferred);

        boost::asio::streambuf& GetBufferToSend();
        void SwitchBufferToSend();
        void SendWithHeader(const char* header, int headerSize, const char* body, int bodySize) override;
        void SendWithDelim(const char* body, int bodySize) override;
        void DoSend();
        void OnSend(boost::system::error_code ec, std::size_t bytes_transferred);

    public:
        tcp::socket& GetInternalSocket()
        {
            return socket_;
        }

    private:
        tcp::socket socket_;

        bool closed_ = false;
        bool sending_ = false;
        bool use_send_buffer_1_ = true;
        boost::asio::streambuf send_buffer_1_;
        boost::asio::streambuf send_buffer_2_;

        const boost::asio::detail::transfer_all_t WRITE_RULE = boost::asio::transfer_all();
    };
}

