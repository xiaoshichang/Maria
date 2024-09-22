
#include "NetworkInstance.h"
#include "../IOContext/IOContext.h"

using namespace Maria::Server::Native;

NetworkInstance::NetworkInstance(NetworkInitInfo info,
                                 OnSessionAcceptCallbackPtr onAccept,
                                 OnSessionConnectedCallbackPtr onConnected,
                                 OnSessionDisconnectCallbackPtr onDisconnect)
    : init_info_(info)
{
    on_accept_callback_ = onAccept;
    on_connected_callback_ = onConnected;
    on_disconnect_callback_ = onDisconnect;
}

NetworkInstance::~NetworkInstance()
{
}





