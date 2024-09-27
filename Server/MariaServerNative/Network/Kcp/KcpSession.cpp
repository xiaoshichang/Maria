
#include "KcpSession.h"
#include "KcpNetworkInstance.h"
#include "../../Logger/Logger.h"
#include <chrono>

using namespace Maria::Server::Native;

KcpSession::KcpSession(KcpNetworkInstance *network, uint32_t session_id, udp::endpoint  remote)
    : network_(network)
    , session_id_(session_id)
    , remote_endpoint_(std::move(remote))
{
    kcp_ = ikcp_create(session_id, this);
    receive_buffer_temp_ = new char[kcp_->mtu];

    auto output = [](const char *buf, int len, ikcpcb *kcp, void *user)->int
    {
        auto session = (KcpSession*)user;
        return session->network_->Send(session, buf, len);
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

void KcpSession::Send(const char *data, int length)
{
    auto& networkInfo = network_->GetNetworkInfo();
    if(networkInfo.SessionEncoderType == SessionMessageEncoderType::Header)
    {
        const int HEADER_SIZE = 4;
        NetworkMessageHeader header{};
        header.MessageLength = length;
        SendWithHeader((const char *) &header, HEADER_SIZE, data, length);
    }
    else
    {
        Logger::Error("unsupported session encoder type.");
        throw;
    }
}

void KcpSession::SendConnectRespond()
{
    auto messageLength = strlen(KcpNetworkInstance::KCP_CONNECT_RESPOND);
    char* rsp = new char[messageLength + 4];
    memcpy(rsp, KcpNetworkInstance::KCP_CONNECT_RESPOND, messageLength);
    memcpy(rsp + messageLength, &session_id_, 4);
    network_->Send(this, rsp, messageLength + 4);
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

void KcpSession::Update()
{
    auto ts = std::chrono::high_resolution_clock::now().time_since_epoch().count();
    ikcp_update(kcp_, ts);
}

void KcpSession::OnDisconnect()
{
    network_->OnDisconnect(this);
}

void KcpSession::SendWithHeader(const char *header, int headerSize, const char *body, int bodySize)
{
    boost::asio::streambuf temp;
    std::ostream os(&temp);
    os.write(header, headerSize);
    os.write(body, bodySize);

    if (ikcp_send(kcp_, (const char *)temp.data().begin()->data(), temp.size()) < 0)
    {
        Logger::Error("KcpSession SendWithDelim error.");
        OnDisconnect();
        return;
    }
}



