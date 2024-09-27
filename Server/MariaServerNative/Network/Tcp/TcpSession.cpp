
#include "TcpSession.h"
#include "TcpNetworkInstance.h"
#include "../../Logger/Logger.h"

using namespace Maria::Server::Native;

TcpSession::TcpSession(TcpNetworkInstance* network, boost::asio::io_context* context)
    : network_(network)
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

void TcpSession::Send(const char *data, int length)
{
    auto& networkInfo = network_->GetNetworkInfo();
    if(networkInfo.SessionEncoderType == SessionMessageEncoderType::Header)
    {
        const int HEADER_SIZE = 4;
        NetworkMessageHeader header{};
        header.MessageLength = length;
        SendWithHeader((const char *) &header, HEADER_SIZE, data, length);
    }
    else if (networkInfo.SessionEncoderType == SessionMessageEncoderType::Delim)
    {
        SendWithDelim(data, length);
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

    network_->OnDisconnect(this);
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

    auto& networkInfo = network_->GetNetworkInfo();
    if (networkInfo.SessionEncoderType == SessionMessageEncoderType::Header)
    {
        TryParseHeaderAndBody();
    }
    else if (networkInfo.SessionEncoderType == SessionMessageEncoderType::Delim)
    {
        throw;
    }
    else
    {
        throw;
    }

    Receive();
}

boost::asio::streambuf &TcpSession::GetBufferToSend()
{
    if (use_send_buffer_1_)
    {
        return send_buffer_1_;
    }
    return send_buffer_2_;
}

void TcpSession::SwitchBufferToSend()
{
    use_send_buffer_1_  = !use_send_buffer_1_;
}


void TcpSession::SendWithHeader(const char *header, int headerSize, const char *body, int bodySize)
{
    auto& buffer = GetBufferToSend();
    std::ostream os(&buffer);
    os.write(header, headerSize);
    os.write(body, bodySize);

    if (sending_)
    {
        return;
    }
    DoSend();
}

void TcpSession::SendWithDelim(const char *body, int bodySize)
{
    auto& buffer = GetBufferToSend();
    std::ostream os(&buffer);
    os.write(body, bodySize);
    os.write(&DELIM, 1);

    if (sending_)
    {
        return;
    }
    DoSend();
}

void TcpSession::DoSend()
{
    auto& buffer = GetBufferToSend();
    if (buffer.size() <= 0)
    {
        return;
    }

    auto callback = [this](boost::system::error_code ec, std::size_t bytes_transferred)
    {
        OnSend(ec, bytes_transferred);
    };

    SwitchBufferToSend();
    sending_ = true;
    boost::asio::async_write(socket_, buffer,  WRITE_RULE, callback);
}

void TcpSession::OnSend(boost::system::error_code ec, std::size_t bytes_transferred)
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
    DoSend();
}

