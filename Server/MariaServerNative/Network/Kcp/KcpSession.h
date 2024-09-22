#pragma once

#include "../NetworkSession.h"

namespace Maria::Server::Native
{
    class KcpNetworkInstance;

    class KcpSession : public NetworkSession
    {
    public:
        explicit KcpSession(KcpNetworkInstance* network);

    public:
        void Start() override;
        void Stop() override;
        void Send(const char* data, int length) override;

    protected:
        void Receive() override;
    };
}

