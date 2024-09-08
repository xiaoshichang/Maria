
#include "TcpSession.h"
#include "TcpConnection.h"
#include "TcpNetworkInstance.h"
#include "../../Logger/Logger.h"

using namespace Maria::Server::Native;

TcpSession::TcpSession(TcpNetworkInstance* network, boost::asio::io_context* context)
    : network_(network)
{
    auto onRead = std::bind(&TcpSession::OnConnectionRead, this, std::placeholders::_1);
    auto onWrite = std::bind(&TcpSession::OnConnectionWrite, this, std::placeholders::_1);
    auto onDisconnect = std::bind(&TcpSession::OnConnectionDisconnect, this);
    connection_ = new TcpConnection(context, onRead, onWrite, onDisconnect);
}

TcpSession::~TcpSession()
{
    delete connection_;
    connection_ = nullptr;
}

void TcpSession::Start()
{
    Receive();
}

void TcpSession::Stop()
{
    connection_->Disconnect();
}

void TcpSession::Receive()
{
    auto& networkInfo = network_->GetNetworkInfo();
    if (networkInfo.SessionEncoderType == SessionMessageEncoderType::Header)
    {
        connection_->ReadAtLeast(read_buffer_, 1);
    }
    else if (networkInfo.SessionEncoderType == SessionMessageEncoderType::Delim)
    {
        connection_->ReadUntilDelim(read_buffer_);
    }
    else
    {
        throw;
    }
}

void TcpSession::Send(const char *data, int length)
{
    auto& networkInfo = network_->GetNetworkInfo();
    if(networkInfo.SessionEncoderType == SessionMessageEncoderType::Header)
    {
        headers_.emplace(length);
        auto& header = headers_.back();
        connection_->Write((const char*)&header, sizeof(header), true);
    }
    else if (networkInfo.SessionEncoderType == SessionMessageEncoderType::Delim)
    {
        connection_->Write(&DELIM, 1, true);
    }
    else
    {
        throw;
    }
    connection_->Write(data, length, false);
}

void TcpSession::OnConnectionDisconnect()
{
    network_->OnSessionDisconnect(this);
}

void TcpSession::OnConnectionWrite(size_t bufferCount)
{
    for (int i = 0; i < bufferCount; ++i)
    {
        headers_.pop();
    }
    on_send_callback_(bufferCount);
}

void TcpSession::OnConnectionRead(size_t byteCount)
{
    auto& networkInfo = network_->GetNetworkInfo();
    if (networkInfo.SessionEncoderType == SessionMessageEncoderType::Header)
    {
        auto bufferSize = read_buffer_.size();
        if (bufferSize < sizeof(NetworkMessageHeader))
        {
            return;
        }
        auto header = boost::asio::buffer_cast<const NetworkMessageHeader*>(read_buffer_.data());
        if (bufferSize < sizeof(NetworkMessageHeader) + header->MessageLength)
        {
            return;
        }

        read_buffer_.consume(sizeof(NetworkMessageHeader));
        const char* data = boost::asio::buffer_cast<const char*>(read_buffer_.data());
        on_receive_callback_(data, header->MessageLength);
        read_buffer_.consume(header->MessageLength);
    }
    else if (networkInfo.SessionEncoderType == SessionMessageEncoderType::Delim)
    {
        auto length = read_buffer_.size();
        const char* data = boost::asio::buffer_cast<const char*>(read_buffer_.data());
        on_receive_callback_(data, length);
        read_buffer_.consume(length);
    }
    else
    {
        throw;
    }
}
