
#include "KcpSession.h"
#include "KcpNetworkInstance.h"
#include "../../Logger/Logger.h"
#include <chrono>

using namespace Maria::Server::Native;

KcpSession::KcpSession(KcpNetworkInstance *network, uint32_t session_id, udp::endpoint  remote)
    : NetworkSession(network)
    , session_id_(session_id)
    , remote_endpoint_(std::move(remote))
{
    kcp_ = ikcp_create(session_id, this);
    receive_buffer_temp_ = new char[kcp_->mtu];

    auto output = [](const char *buf, int len, ikcpcb *kcp, void *user)->int
    {
        auto session = (KcpSession*)user;
        auto sent = dynamic_cast<KcpNetworkInstance*>(session->network_)->Send(session, buf, len);
        return (int)sent;
    };
    kcp_->output = output;
}

KcpSession::~KcpSession()
{
    ikcp_release(kcp_);
    kcp_ = nullptr;
    delete[] receive_buffer_temp_;
    receive_buffer_temp_ = nullptr;
}

void KcpSession::SendConnectRespond()
{
    auto messageLength = strlen(KcpNetworkInstance::KCP_CONNECT_RESPOND);
    char* rsp = new char[messageLength + 4];
    memcpy(rsp, KcpNetworkInstance::KCP_CONNECT_RESPOND, messageLength);
    memcpy(rsp + messageLength, &session_id_, 4);

    dynamic_cast<KcpNetworkInstance*>(network_)->Send(this, rsp, messageLength + 4);
}

void KcpSession::HandleSessionData(const char *data, std::size_t length)
{
    auto cnt = ikcp_input(kcp_, data, length);
    if (cnt != length)
    {
        Logger::Error("KcpSession OnReceive error 1.");
        OnDisconnect();
        return;
    }
    auto received = ikcp_recv(kcp_, receive_buffer_temp_, kcp_->mtu);
    if (received > kcp_->mtu)
    {
        Logger::Error("KcpSession OnReceive error 2.");
        OnDisconnect();
        return;
    }
    receive_buffer_.write(receive_buffer_temp_, received);
    TryParseHeaderAndBody();
}


void KcpSession::OnReceive(const char *data, std::size_t length)
{
    if (memcmp(data, KcpNetworkInstance::KCP_CONNECT_REQUEST, strlen(KcpNetworkInstance::KCP_CONNECT_REQUEST)) == 0)
    {
        SendConnectRespond();
    }
    else
    {
        HandleSessionData(data, length);
    }
}

void KcpSession::Update(unsigned int ts)
{
    ikcp_update(kcp_, ts);
}

void KcpSession::OnDisconnect()
{
    dynamic_cast<KcpNetworkInstance*>(network_)->OnDisconnect(this);
}

void KcpSession::SendWithHeader(const char *header, int headerSize, const char *body, int bodySize)
{
    boost::asio::streambuf temp;
    std::ostream os(&temp);
    os.write(header, headerSize);
    os.write(body, bodySize);

    if (ikcp_send(kcp_, (const char *)temp.data().begin()->data(), temp.size()) < 0)
    {
        Logger::Error("KcpSession SendWithHeader error.");
        OnDisconnect();
        return;
    }
}

void KcpSession::SendWithDelim(const char *body, int bodySize)
{
    Logger::Error("KcpSession SendWithDelim unsupported.");
}



