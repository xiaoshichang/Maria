
#include "NetworkInstance.h"
#include "../IOContext/IOContext.h"

using namespace Maria::Server::Native;

NetworkInstance::NetworkInstance(NetworkInitInfo info,
                                 OnSessionConnectedCallbackPtr onConnected,
                                 OnSessionAcceptCallbackPtr onAccept,
                                 OnSessionDisconnectCallbackPtr onDisconnect)
    : init_info_(info)
{
    context_ = IOContext::Get();
    on_accept_callback_ = onAccept;
    on_connected_callback_ = onConnected;
    on_disconnect_callback_ = onDisconnect;
}

NetworkInstance::~NetworkInstance()
{
    context_ = nullptr;
}



