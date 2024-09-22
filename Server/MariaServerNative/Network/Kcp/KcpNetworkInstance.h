#pragma once

#include <boost/asio.hpp>
#include "../NetworkInstance.h"

using boost::asio::ip::udp;

namespace Maria::Server::Native
{
    class KcpNetworkInstance : public NetworkInstance
    {
        public:
            KcpNetworkInstance(NetworkInitInfo info,
                    OnSessionAcceptCallbackPtr onAccept,
                    OnSessionConnectedCallbackPtr onConnected,
                    OnSessionDisconnectCallbackPtr onDisconnect);

            ~KcpNetworkInstance() override;

    public:
        void StartListen(const char* ip, int port) override;
        void StopListen() override;
        void ConnectTo(const char* ip, int port) override;
        unsigned int GetSessionCount() override;

    private:
        udp::socket* socket_ = nullptr;
    };
}