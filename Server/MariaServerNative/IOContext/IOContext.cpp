
#include "IOContext.h"

using namespace Maria::Server::Native;

boost::asio::io_context* IOContext::Context_ = nullptr;
std::unique_ptr<boost::asio::executor_work_guard<boost::asio::io_context::executor_type>> IOContext::WorkGuard_ = nullptr;

void IOContext::Init()
{
    Context_ = new boost::asio::io_context();
    auto workGuard = boost::asio::make_work_guard(Context_->get_executor());
    WorkGuard_ = std::make_unique<boost::asio::executor_work_guard<boost::asio::io_context::executor_type>>(workGuard);
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
    WorkGuard_->reset();
    WorkGuard_ = nullptr;
}

boost::asio::io_context *IOContext::Get()
{
    return Context_;
}
