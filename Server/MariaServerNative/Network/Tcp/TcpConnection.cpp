#include <boost/bind/bind.hpp>
#include "TcpConnection.h"
#include "TcpNetworkInstance.h"
#include "../../Logger/Logger.h"

using namespace Maria::Server::Native;

TcpConnection::TcpConnection(boost::asio::io_context* context,
                             OnConnectionReadCallback onRead,
                             OnConnectionWriteCallback onWrite,
                             OnConnectionDisconnectCallback onDisconnect)
    : socket_(*context)
    , NetworkConnection(std::move(onRead), std::move(onWrite), std::move(onDisconnect))
{
}

TcpConnection::~TcpConnection()
{
    if (!closed_)
    {
        Logger::Warning("destroy connection while socket is open.");
    }
}

void TcpConnection::ReadAtLeast(boost::asio::streambuf& buffer, int byteCount)
{
    auto read_rule = boost::asio::detail::transfer_at_least_t(byteCount);
    auto callback = [this](boost::system::error_code ec, std::size_t bytes_transferred)
    {
        OnRead(ec, bytes_transferred);
    };
    boost::asio::async_read(socket_, buffer, read_rule, callback);
}

void TcpConnection::ReadUntilDelim(boost::asio::streambuf& buffer)
{
    auto callback = [this](boost::system::error_code ec, std::size_t bytes_transferred)
    {
        OnRead(ec, bytes_transferred);
    };
    boost::asio::async_read_until(socket_, buffer, '\n', callback);
}

void TcpConnection::Write(const char *data, int length, bool cache)
{
    to_write_buffers_.push_back(boost::asio::buffer(data, length));
    if (cache || writing_)
    {
        return;
    }
    Writing();
}

void TcpConnection::Writing()
{
    if (to_write_buffers_.empty())
    {
        return;
    }
    writing_buffers_ = std::move(to_write_buffers_);
    to_write_buffers_.clear();
    writing_ = true;

    auto callback = [this](boost::system::error_code ec, std::size_t bytes_transferred)
    {
        OnWrite(ec, bytes_transferred);
    };

    // https://www.gnu.org/software/libc/manual/html_node/Scatter_002dGather.html
    boost::asio::async_write(socket_, writing_buffers_,  WRITE_RULE,callback);
}

void TcpConnection::OnWrite(boost::system::error_code ec, std::size_t bytes_transferred)
{
    writing_ = false;
    if (ec)
    {
        Logger::Error(std::format("TcpConnection OnWrite error, what:{}, message:{}", ec.what(), ec.message()));
        OnDisconnect();
        return;
    }
    if (bytes_transferred == 0)
    {
        Logger::Error("TcpConnection OnWrite eof");
        OnDisconnect();
        return;
    }

    on_write_callback_(writing_buffers_.size());
    writing_buffers_.clear();
    Writing();
}

void TcpConnection::OnRead(boost::system::error_code ec, std::size_t bytes_transferred)
{
    if (ec)
    {
        Logger::Error(std::format("TcpConnection OnRead error, what:{}, message:{}", ec.what(), ec.message()));
        OnDisconnect();
        return;
    }
    if (bytes_transferred == 0)
    {
        Logger::Error("TcpConnection OnRead eof");
        OnDisconnect();
        return;
    }
    on_read_callback_(bytes_transferred);
}

void TcpConnection::Disconnect()
{
    OnDisconnect();
}

void TcpConnection::OnDisconnect()
{
    if (closed_)
    {
        return;
    }
    closed_ = true;
    socket_.close();
    on_disconnect_callback_();
}


