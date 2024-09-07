
#include "TcpConnection.h"
#include "TcpNetworkInstance.h"

using namespace Maria::Server::Native;

TcpConnection::TcpConnection(TcpNetworkInstance* network, boost::asio::io_context* context)
    : network_(network)
    , socket_(*context)
{

}

TcpConnection::~TcpConnection()
{
}

void TcpConnection::StartReading()
{

}

void TcpConnection::Write(const char *data, int length) {

}

void TcpConnection::Disconnect() {

}

void TcpConnection::OnWrite(boost::system::error_code ec, std::size_t bytes_transferred) {

}

void TcpConnection::OnRead(boost::system::error_code ec, std::size_t bytes_transferred) {

}

void TcpConnection::OnDisconnect() {

}
