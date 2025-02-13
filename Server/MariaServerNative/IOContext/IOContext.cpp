
#include "IOContext.h"

using namespace Maria::Server::Native;

boost::asio::io_context* IOContext::Context_ = nullptr;

void IOContext::Init()
{
    Context_ = new boost::asio::io_context();
}

void IOContext::Run()
{
    Context_->run();
}

void IOContext::UnInit()
{
    delete Context_;
    Context_ = nullptr;
}

void IOContext::Stop()
{
    Context_->stop();
}

boost::asio::io_context *IOContext::Get()
{
    return Context_;
}
