
#include "TcpSession.h"
#include "TcpConnection.h"

using namespace Maria::Server::Native;

TcpSession::TcpSession(TcpNetworkInstance* network, boost::asio::io_context* context)
    : network_(network)
{
    connection_ = new TcpConnection(network, context);
}

TcpSession::~TcpSession()
{
    delete connection_;
    connection_ = nullptr;
}

void TcpSession::Start()
{
    connection_->StartReading();
}

void TcpSession::Send(const char *data, int length) {

}

void TcpSession::OnConnectionWrite() {

}

void TcpSession::OnConnectionRead() {

}

void TcpSession::OnConnectionDisconnect() {

}
