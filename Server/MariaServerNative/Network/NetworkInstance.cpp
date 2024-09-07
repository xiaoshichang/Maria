
#include "NetworkInstance.h"
#include "../IOContext/IOContext.h"

using namespace Maria::Server::Native;

NetworkInstance::NetworkInstance(NetworkInitInfo info,
                                 OnSessionConnectedCallbackPtr onConnected,
                                 OnSessionAcceptCallbackPtr onAccept)
    : init_info_(info)
{
    context_ = IOContext::Get();
    on_accept_callback_ = onAccept;
    on_connected_callback_ = onConnected;
}

NetworkInstance::~NetworkInstance()
{
    context_ = nullptr;
}



