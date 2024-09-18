
#include "NetworkConnection.h"

#include <utility>

using namespace Maria::Server::Native;

NetworkConnection::NetworkConnection(OnConnectionReadCallback onRead,
                                     OnConnectionDisconnectCallback onDisconnect)
    : on_read_callback_(std::move(onRead))
    , on_disconnect_callback_(std::move(onDisconnect))
{
}
