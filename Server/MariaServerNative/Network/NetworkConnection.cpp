
#include "NetworkConnection.h"

#include <utility>

using namespace Maria::Server::Native;

NetworkConnection::NetworkConnection(OnConnectionReadCallback onRead,
                                     OnConnectionWriteCallback onWrite,
                                     OnConnectionDisconnectCallback onDisconnect)
    : on_read_callback_(std::move(onRead))
    , on_write_callback_(std::move(onWrite))
    , on_disconnect_callback_(std::move(onDisconnect))
{
}
