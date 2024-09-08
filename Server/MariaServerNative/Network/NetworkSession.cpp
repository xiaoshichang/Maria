
#include "NetworkSession.h"

using namespace Maria::Server::Native;

void
NetworkSession::Bind(OnSessionReceiveCallbackPtr onReceive, OnSessionSendCallbackPtr onSend)
{
    on_receive_callback_ = onReceive;
    on_send_callback_ = onSend;
}
