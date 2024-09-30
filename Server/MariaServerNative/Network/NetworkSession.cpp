

#include <boost/asio/detail/socket_ops.hpp>
#include "NetworkSession.h"
#include "NetworkInstance.h"
#include "../Logger/Logger.h"

using namespace Maria::Server::Native;

NetworkSession::NetworkSession(NetworkInstance *network)
    : network_(network)
{
}

void NetworkSession::Bind(OnSessionReceiveCallbackPtr onReceive)
{
    on_receive_callback_ = onReceive;
}

void NetworkSession::TryParseHeaderAndBody()
{
    while(true)
    {
        auto bufferSize = receive_buffer_.size();
        if (bufferSize < sizeof(NetworkMessageHeader))
        {
            return;
        }
        auto ptr = boost::asio::buffer_cast<const NetworkMessageHeader*>(receive_buffer_.data());

        NetworkMessageHeader header{};
        header.MessageLength = boost::asio::detail::socket_ops::host_to_network_long(ptr->MessageLength);

        if (bufferSize < sizeof(NetworkMessageHeader) + header.MessageLength)
        {
            return;
        }

        receive_buffer_.consume(sizeof(NetworkMessageHeader));
        const char* data = boost::asio::buffer_cast<const char*>(receive_buffer_.data());
        on_receive_callback_(data, header.MessageLength);
        receive_buffer_.consume(header.MessageLength);
    }
}

void NetworkSession::Send(const char *data, int length)
{
    auto info = network_->GetNetworkInfo();
    if(info.SessionEncoderType == SessionMessageEncoderType::Header)
    {
        const int HEADER_SIZE = 4;
        NetworkMessageHeader header{};
        header.MessageLength = boost::asio::detail::socket_ops::network_to_host_long(length);
        SendWithHeader((const char *) &header, HEADER_SIZE, data, length);
    }
    else if (info.SessionEncoderType == SessionMessageEncoderType::Delim)
    {
        SendWithDelim(data, length);
    }
    else
    {
        Logger::Error("unsupported session encoder type.");
        throw;
    }
}



