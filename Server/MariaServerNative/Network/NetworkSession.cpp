
#include "NetworkSession.h"
#include "NetworkInstance.h"

using namespace Maria::Server::Native;

NetworkSession::NetworkSession(NetworkInstance *network)
    : network_(network)
{
}

void
NetworkSession::Bind(OnSessionReceiveCallbackPtr onReceive)
{
    on_receive_callback_ = onReceive;
}

void NetworkSession::OnDisconnect()
{
    network_->OnDisconnect(this);
}


