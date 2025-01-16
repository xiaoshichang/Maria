

#include <boost/asio/detail/socket_ops.hpp>
#include "NetworkSession.h"
#include "NetworkInstance.h"
#include "../Logger/Logger.h"

using namespace Maria::Server::Native;

NetworkSession::NetworkSession(NetworkInstance *network)
    : network_(network)
{
}

void NetworkSession::Bind(OnSessionReceiveCallbackPtr onReceive, OnSessionSendCallbackPtr onSend)
{
    on_receive_callback_ = onReceive;
    on_send_callback_ = onSend;
}


