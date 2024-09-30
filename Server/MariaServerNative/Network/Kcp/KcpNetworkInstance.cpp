
#include "KcpNetworkInstance.h"
#include "../../Logger/Logger.h"
#include "../../IOContext/IOContext.h"
#include "../../Timer/TimerMgr.h"

using namespace Maria::Server::Native;

const char* KcpNetworkInstance::KCP_CONNECT_REQUEST = "MARIA FRAMEWORK KCP CONNECT REQUEST";
const char* KcpNetworkInstance::KCP_CONNECT_RESPOND = "MARIA FRAMEWORK KCP CONNECT RESPOND";


KcpNetworkInstance::KcpNetworkInstance(NetworkInitInfo info, OnSessionAcceptCallbackPtr onAccept,
                                       OnSessionConnectedCallbackPtr onConnected,
                                       OnSessionDisconnectCallbackPtr onDisconnect)
        : NetworkInstance(info, onAccept, onConnected, onDisconnect)
{
    udp_receive_buffer_ = new char[MAX_UDP_PACKET_SIZE];
    auto callback = [this]()
    {
        Update();
    };
    TimerMgr::AddRepeatTimer(0, 20, callback);
}


KcpNetworkInstance::~KcpNetworkInstance()
{
    delete[] udp_receive_buffer_;
    udp_receive_buffer_ = nullptr;
}


void KcpNetworkInstance::StartListen(const char *ip, int port)
{
    auto address = boost::asio::ip::address_v4::from_string(ip);
    auto endpoint = udp::endpoint(address, port);
    udp_socket_ = new udp::socket(*IOContext::Get(), endpoint);
    Receive();
}

void KcpNetworkInstance::StopListen()
{
    udp_socket_->cancel();
    delete udp_socket_;
    udp_socket_ = nullptr;
}

void KcpNetworkInstance::ConnectTo(const char *ip, int port)
{
    Logger::Error("invalid operation.");
}

void KcpNetworkInstance::OnDisconnect(KcpSession *session)
{
    on_disconnect_callback_(session);
    sessions_.erase(session->GetRemoteEndPoint());
    delete session;
}

unsigned int KcpNetworkInstance::GetSessionCount()
{
    return sessions_.size();
}

void KcpNetworkInstance::Update()
{
    auto ts = std::chrono::high_resolution_clock::now().time_since_epoch();
    auto tsInMs = std::chrono::duration_cast<std::chrono::milliseconds>(ts).count();
    for (auto& pair : sessions_)
    {
        pair.second->Update(tsInMs);
    }
}

void KcpNetworkInstance::Receive()
{
    auto callback = [this](boost::system::error_code ec, std::size_t bytes_transferred)
    {
        OnReceive(ec, bytes_transferred);
    };
    udp_socket_->async_receive_from(boost::asio::buffer(udp_receive_buffer_, MAX_UDP_PACKET_SIZE), udp_remote_endpoint_, callback);
}

void KcpNetworkInstance::OnReceive(const boost::system::error_code &ec, std::size_t bytes_transferred)
{
    if (ec)
    {
        Logger::Error(std::format("TcpSession OnReceive error, what:{}, message:{}", ec.what(), ec.message()));
        Receive();
        return;
    }

    auto session = sessions_.find(udp_remote_endpoint_);
    if (session != sessions_.end())
    {
        session->second->OnReceive(udp_receive_buffer_, bytes_transferred);
    }
    else
    {
        auto sid = AllocateSessionID();
        auto newSession = new KcpSession(this, sid, udp_remote_endpoint_);
        sessions_[udp_remote_endpoint_] = newSession;
        newSession->OnReceive(udp_receive_buffer_, bytes_transferred);
    }
    Receive();
}

size_t KcpNetworkInstance::Send(KcpSession *session, const char *buf, size_t len)
{
    return udp_socket_->send_to(boost::asio::buffer(buf, len), session->GetRemoteEndPoint());
}


uint32_t KcpNetworkInstance::AllocateSessionID()
{
    static uint32_t sessionID = 0;
    return sessionID++;
}




