
#include "TcpSession.h"
#include "TcpNetworkInstance.h"
#include "../../Logger/Logger.h"

using namespace Maria::Server::Native;

TcpSession::TcpSession(TcpNetworkInstance* network, boost::asio::io_context* context)
    : NetworkSession(network)
    , socket_(*context)
{
}

TcpSession::~TcpSession()
{
}

void TcpSession::Start()
{
    Receive();
}

void TcpSession::Stop()
{
}

void TcpSession::Receive()
{
    auto& networkInfo = network_->GetNetworkInfo();
    if (networkInfo.SessionEncoderType == SessionMessageEncoderType::Header)
    {
        ReadAtLeast(1);
    }
    else if (networkInfo.SessionEncoderType == SessionMessageEncoderType::Delim)
    {
        ReadUntilDelim();
    }
    else
    {
        Logger::Error("unsupported session encoder type.");
        throw;
    }
}


void TcpSession::OnDisconnect()
{
    if (closed_)
    {
        return;
    }
    closed_ = true;
    socket_.close();

    dynamic_cast<TcpNetworkInstance*>(network_)->OnDisconnect(this);
}



void TcpSession::ReadAtLeast(int byteCount)
{
    auto read_rule = boost::asio::detail::transfer_at_least_t(byteCount);
    auto callback = [this](boost::system::error_code ec, std::size_t bytes_transferred)
    {
        OnReceive(ec, bytes_transferred);
    };
    boost::asio::async_read(socket_, receive_buffer_, read_rule, callback);
}

void TcpSession::ReadUntilDelim()
{
    auto callback = [this](boost::system::error_code ec, std::size_t bytes_transferred)
    {
        OnReceive(ec, bytes_transferred);
    };
    boost::asio::async_read_until(socket_, receive_buffer_, "\n", callback);
}

void TcpSession::OnReceive(boost::system::error_code ec, std::size_t bytes_transferred)
{
    if (ec)
    {
        Logger::Error(std::format("TcpSession OnReceive error, what:{}, message:{}", ec.what(), ec.message()));
        OnDisconnect();
        return;
    }
    if (bytes_transferred == 0)
    {
        Logger::Error("TcpSession OnReceive eof");
        OnDisconnect();
        return;
    }

    const char* data = boost::asio::buffer_cast<const char*>(receive_buffer_.data());
    on_receive_callback_(data, receive_buffer_.size());
    Receive();
}

void TcpSession::ConsumeReceiveBuffer(int count)
{
    receive_buffer_.consume(count);
}

void TcpSession::Send(const char *data, int length)
{
    send_queue_.emplace_back(data, length);
    auto buffer = send_queue_[0];
    if (!sending_)
    {
        DoSend();
    }
}

void TcpSession::DoSend()
{
    if (send_queue_.empty())
    {
        return;
    }

    auto count = (int)send_queue_.size();
    auto callback = [=, this](boost::system::error_code ec, std::size_t bytes_transferred)
    {
        OnSend(ec, bytes_transferred, count);
    };

    sending_ = true;

    boost::asio::async_write(socket_, send_queue_,  WRITE_RULE, callback);
    send_queue_.clear();
}

void TcpSession::OnSend(boost::system::error_code ec, std::size_t bytes_transferred, int buffer_count)
{
    if (ec)
    {
        Logger::Error(std::format("TcpSession OnSend error, what:{}, message:{}", ec.what(), ec.message()));
        OnDisconnect();
        return;
    }
    if (bytes_transferred == 0)
    {
        Logger::Error("TcpSession OnSend eof");
        OnDisconnect();
        return;
    }
    sending_ = false;

    // let session owner known that there are {buffer_count} buffers sent.
    on_send_callback_(buffer_count);

    DoSend();
}


