#include <boost/bind/bind.hpp>
#include "TcpConnection.h"
#include "TcpNetworkInstance.h"
#include "../../Logger/Logger.h"

using namespace Maria::Server::Native;

TcpConnection::TcpConnection(boost::asio::io_context* context,
                             OnConnectionReadCallback onRead,
                             OnConnectionDisconnectCallback onDisconnect)
    : socket_(*context)
    , NetworkConnection(std::move(onRead), std::move(onDisconnect))
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

void TcpConnection::WriteWithHeader(const char *header, int headerSize, const char *body, int bodySize)
{
    auto& buffer = GetBufferToWrite();
    std::ostream os(&buffer);
    os.write(header, headerSize);
    os.write(body, bodySize);

    if (writing_)
    {
        return;
    }
    DoWrite();
}

void TcpConnection::WriteWithDelim(const char *body, int bodySize, char delim)
{
    auto& buffer = GetBufferToWrite();
    std::ostream os(&buffer);
    os.write(body, bodySize);
    os.write(&delim, 1);

    if (writing_)
    {
        return;
    }
    DoWrite();
}

void TcpConnection::DoWrite()
{
    auto& buffer = GetBufferToWrite();
    if (buffer.size() <= 0)
    {
        return;
    }

    auto callback = [this](boost::system::error_code ec, std::size_t bytes_transferred)
    {
        OnWrite(ec, bytes_transferred);
    };

    SwitchBufferToWrite();
    writing_ = true;
    boost::asio::async_write(socket_, buffer,  WRITE_RULE, callback);
}

void TcpConnection::OnWrite(boost::system::error_code ec, std::size_t bytes_transferred)
{
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
    writing_ = false;
    DoWrite();
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

boost::asio::streambuf &TcpConnection::GetBufferToWrite()
{
    if (write_to_1_)
    {
        return write_buffer_1_;
    }
    return write_buffer_2_;
}

void TcpConnection::SwitchBufferToWrite()
{
    write_to_1_  = !write_to_1_;
    auto& buffer = GetBufferToWrite();
    if (buffer.size() != 0)
    {
        Logger::Error("SwitchBufferToWrite");
    }
}






