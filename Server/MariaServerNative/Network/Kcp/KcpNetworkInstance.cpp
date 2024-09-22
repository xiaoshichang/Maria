
#include "KcpNetworkInstance.h"
#include "../../Logger/Logger.h"
#include "../../IOContext/IOContext.h"
#include "../../Timer/TimerMgr.h"

using namespace Maria::Server::Native;

KcpNetworkInstance::KcpNetworkInstance(NetworkInitInfo info, OnSessionAcceptCallbackPtr onAccept,
                                       OnSessionConnectedCallbackPtr onConnected,
                                       OnSessionDisconnectCallbackPtr onDisconnect)
        : NetworkInstance(info, onAccept, onConnected, onDisconnect)
{
    receive_buffer_ = new char[8192];
    auto callback = [this]()
    {
        Update();
    };
    TimerMgr::AddRepeatTimer(0, 20, callback);
}


KcpNetworkInstance::~KcpNetworkInstance()
{
    delete[] receive_buffer_;
    receive_buffer_ = nullptr;
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

unsigned int KcpNetworkInstance::GetSessionCount()
{
    return 0;
}

void KcpNetworkInstance::Update()
{
    for (auto& pair : sessions_)
    {
        pair.second->Update();
    }
}

void KcpNetworkInstance::Receive()
{
    auto callback = [this](boost::system::error_code ec, std::size_t bytes_transferred)
    {
        OnReceive(ec, bytes_transferred);
    };
    udp_socket_->async_receive_from(boost::asio::buffer(receive_buffer_, 8192), udp_remote_endpoint_, callback);
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
    if (session == sessions_.end())
    {
        Logger::Warning(std::format("session not found. {}:{}", udp_remote_endpoint_.address().to_string(), udp_remote_endpoint_.port()));
        Receive();
        return;
    }

    session->second->OnReceive(receive_buffer_, bytes_transferred);
}

int KcpNetworkInstance::Send(const char *buf, int len, ikcpcb *kcp, void *user)
{
    return 0;
}

