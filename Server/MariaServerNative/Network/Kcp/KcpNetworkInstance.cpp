
#include "KcpNetworkInstance.h"
#include "../../Logger/Logger.h"
#include "../../IOContext/IOContext.h"

using namespace Maria::Server::Native;

KcpNetworkInstance::KcpNetworkInstance(NetworkInitInfo info, OnSessionAcceptCallbackPtr onAccept,
                                       OnSessionConnectedCallbackPtr onConnected,
                                       OnSessionDisconnectCallbackPtr onDisconnect)
        : NetworkInstance(info, onAccept, onConnected, onDisconnect)
{

}


KcpNetworkInstance::~KcpNetworkInstance()
{

}


void KcpNetworkInstance::StartListen(const char *ip, int port)
{
    auto address = boost::asio::ip::address_v4::from_string(ip);
    auto endpoint = udp::endpoint(address, port);
    socket_ = new udp::socket(*IOContext::Get(), endpoint);

}

void KcpNetworkInstance::StopListen()
{
    socket_->cancel();
    delete socket_;
    socket_ = nullptr;
}

void KcpNetworkInstance::ConnectTo(const char *ip, int port)
{
    Logger::Error("invalid operation.");
}

unsigned int KcpNetworkInstance::GetSessionCount()
{
    return 0;
}
