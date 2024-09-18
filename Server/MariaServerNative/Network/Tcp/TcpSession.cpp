
#include "TcpSession.h"
#include "TcpConnection.h"
#include "TcpNetworkInstance.h"
#include "../../Logger/Logger.h"

using namespace Maria::Server::Native;

TcpSession::TcpSession(TcpNetworkInstance* network, boost::asio::io_context* context)
    : network_(network)
{
    auto onRead = std::bind(&TcpSession::OnConnectionRead, this, std::placeholders::_1);
    auto onDisconnect = std::bind(&TcpSession::OnConnectionDisconnect, this);
    connection_ = new TcpConnection(context, onRead, onDisconnect);
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
        connection_->WriteWithHeader((const char*)&header, HEADER_SIZE, data, length);
    }
    else if (networkInfo.SessionEncoderType == SessionMessageEncoderType::Delim)
    {
        connection_->WriteWithDelim(data, length, DELIM);
    }
    else
    {
        Logger::Error("unsupported session encoder type.");
        throw;
    }
}

void TcpSession::OnConnectionDisconnect()
{
    network_->OnSessionDisconnect(this);
}

void TcpSession::OnConnectionRead(size_t byteCount)
{
    auto& networkInfo = network_->GetNetworkInfo();
    if (networkInfo.SessionEncoderType == SessionMessageEncoderType::Header)
    {
        TryParseHeaderAndBody();
        Receive();
    }
    else if (networkInfo.SessionEncoderType == SessionMessageEncoderType::Delim)
    {
        throw;
    }
    else
    {
        throw;
    }
}

void TcpSession::TryParseHeaderAndBody()
{
    while(true)
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

}
