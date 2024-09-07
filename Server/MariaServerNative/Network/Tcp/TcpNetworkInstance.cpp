
#include <boost/asio/placeholders.hpp>
#include <boost/bind/bind.hpp>
#include "TcpNetworkInstance.h"
#include "../../Logger/Logger.h"

using namespace Maria::Server::Native;
using boost::asio::ip::tcp;

TcpNetworkInstance::TcpNetworkInstance(NetworkInitInfo info,
                                       OnSessionConnectedCallbackPtr onConnected,
                                       OnSessionAcceptCallbackPtr onAccept)
                                       :
                                       NetworkInstance(info, onConnected, onAccept)
{
}

TcpNetworkInstance::~TcpNetworkInstance()
{

}

void TcpNetworkInstance::StartListen(const char *ip, int port)
{
    Logger::Info(std::format("StartListen at:{}:{}", ip, port));
    auto address = boost::asio::ip::address_v4::from_string(ip);
    auto endpoint = tcp::endpoint(address, port);
    acceptor_ = new tcp::acceptor(*context_, endpoint);
    Accept();
}

void TcpNetworkInstance::StopListen()
{
    acceptor_->cancel();
    delete acceptor_;
    acceptor_ = nullptr;
}

void TcpNetworkInstance::Accept()
{
    auto session = new TcpSession(this, context_);
    auto callback = [this, session](boost::system::error_code ec)
    {
        if (ec == boost::asio::error::operation_aborted)
        {
            delete session;
            return;
        }
        if (ec)
        {
            Logger::Error(std::format("On Accept Error. error:{}", ec.value()));
            delete session;
            Accept();
            return;
        }
        sessions_.insert(session);
        on_accept_callback_(session);
        session->Start();
        Accept();
    };

    acceptor_->async_accept(session->GetInternalSocket(), callback);
}

void TcpNetworkInstance::ConnectTo(const char *ip, int port)
{

}

unsigned int TcpNetworkInstance::GetSessionCount()
{
    return sessions_.size();
}

