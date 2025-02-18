
#include <boost/asio/placeholders.hpp>
#include <boost/bind/bind.hpp>
#include "TcpNetworkInstance.h"
#include "../../Logger/Logger.h"
#include "../../IOContext/IOContext.h"

using namespace Maria::Server::Native;
using boost::asio::ip::tcp;

TcpNetworkInstance::TcpNetworkInstance(NetworkInitInfo info,
                                       OnSessionAcceptCallbackPtr onAccept,
                                       OnSessionConnectedCallbackPtr onConnected,
                                       OnSessionDisconnectCallbackPtr onDisconnect)
   : NetworkInstance(info, onAccept,onConnected, onDisconnect)
{
}

TcpNetworkInstance::~TcpNetworkInstance()
{

}

void TcpNetworkInstance::Start(const char *ip, int port)
{
    Logger::Info(std::format("StartListen at:{}:{}", ip, port));
    auto address = boost::asio::ip::address_v4::from_string(ip);
    auto endpoint = tcp::endpoint(address, port);
    try
    {
        acceptor_ = new tcp::acceptor(*IOContext::Get(), endpoint);
        Accept();
    }
    catch(std::exception& e)
    {
        Logger::Error(e.what());
        throw e;
    }
}

void TcpNetworkInstance::Stop()
{
    if (acceptor_ == nullptr)
    {
        return;
    }

    // stop accept new session
    acceptor_->cancel();
    delete acceptor_;
    acceptor_ = nullptr;

    // and disconnect all exist sessions
    std::set<TcpSession*> all = sessions_;
    for (auto session : all)
    {
        session->Stop();
    }
    Logger::Info(std::format("Closing network instance, {} sessions closed.", all.size()));
}

void TcpNetworkInstance::Accept()
{
    auto session = new TcpSession(this, IOContext::Get());
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
        // todo: supports disconnect operation in on_accept_callback_
        session->Start();
        Accept();
    };

    acceptor_->async_accept(session->GetInternalSocket(), callback);
}

void TcpNetworkInstance::ConnectTo(const char *ip, int port)
{
    auto address = boost::asio::ip::address_v4::from_string(ip);
    auto endpoint = tcp::endpoint(address, port);
    auto session = new TcpSession(this, IOContext::Get());
    auto& socket = session->GetInternalSocket();

    auto callback = [this, session](const boost::system::error_code &ec)
    {
        if (ec)
        {
            delete session;
            Logger::Error(std::format("On OnConnect Error. error:{}", ec.value()));
            return;
        }
        sessions_.insert(session);
        on_connected_callback_(session, ec.value());
        session->Start();
    };

    socket.async_connect(endpoint, std::move(callback));
    Logger::Info(std::format("ConnectTo {}:{}", ip, port));
}


unsigned int TcpNetworkInstance::GetSessionCount()
{
    return sessions_.size();
}

void TcpNetworkInstance::OnDisconnect(TcpSession *session)
{
    on_disconnect_callback_(session);
    sessions_.erase(session);
    delete session;
}


