
#include "IOContext.h"
#include "../Timer/TimerMgr.h"

using namespace Maria::Server::Native;

boost::asio::io_context* IOContext::Context_ = nullptr;

void IOContext::Init()
{
    Context_ = new boost::asio::io_context();
    InitTimerManager();
}

void IOContext::Run()
{
    Context_->run();
}

void IOContext::UnInit()
{
    UnInitTimerManager();
    delete Context_;
    Context_ = nullptr;
}

void IOContext::Stop()
{
    Context_->stop();
}

void IOContext::InitTimerManager()
{
    TimerMgr::Init(Context_);
}

void IOContext::UnInitTimerManager()
{
    TimerMgr::UnInit();
}

boost::asio::io_context *IOContext::Get()
{
    return Context_;
}
