
#include "KcpSession.h"
#include "KcpNetworkInstance.h"
#include <chrono>

using namespace Maria::Server::Native;

KcpSession::KcpSession(KcpNetworkInstance *network, uint32_t session_id, udp::endpoint remote)
    : network_(network)
{
    kcp_ = ikcp_create(session_id, this);

    auto output = [](const char *buf, int len, ikcpcb *kcp, void *user)->int
    {
        auto session = (KcpSession*)user;
        return session->network_->Send(buf, len, kcp, user);
    };
    kcp_->output = output;
}

KcpSession::~KcpSession()
{
    ikcp_release(kcp_);
    kcp_ = nullptr;
}

void KcpSession::Send(const char *data, int length)
{
    ikcp_send(kcp_, data, length);
}

void KcpSession::OnReceive(const char *data, std::size_t length)
{
    // input data to kcp
    ikcp_input(kcp_, data, length);
}

void KcpSession::Update()
{
    auto ts = std::chrono::high_resolution_clock::now().time_since_epoch().count();
    ikcp_update(kcp_, ts);
}
