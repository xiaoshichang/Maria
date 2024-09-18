
#include "NetworkSession.h"

using namespace Maria::Server::Native;

void
NetworkSession::Bind(OnSessionReceiveCallbackPtr onReceive)
{
    on_receive_callback_ = onReceive;
}
