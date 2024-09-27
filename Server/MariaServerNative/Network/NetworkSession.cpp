
#include "NetworkSession.h"
#include "NetworkInstance.h"

using namespace Maria::Server::Native;

void
NetworkSession::Bind(OnSessionReceiveCallbackPtr onReceive)
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
        auto header = boost::asio::buffer_cast<const NetworkMessageHeader*>(receive_buffer_.data());
        if (bufferSize < sizeof(NetworkMessageHeader) + header->MessageLength)
        {
            return;
        }

        receive_buffer_.consume(sizeof(NetworkMessageHeader));
        const char* data = boost::asio::buffer_cast<const char*>(receive_buffer_.data());
        on_receive_callback_(data, header->MessageLength);
        receive_buffer_.consume(header->MessageLength);
    }
}


