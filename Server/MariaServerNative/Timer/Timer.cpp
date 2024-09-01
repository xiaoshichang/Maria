
#include "Timer.h"

#include <utility>

using namespace Maria::Server::Native;

Timer::Timer(boost::asio::io_context* io_context,
              boost::posix_time::millisec delay,
              TimeoutCallback callback):
      repeat_(false),
      canceled_(false),
      interval_(boost::posix_time::millisec(0)),
      callback_(callback),
      timer_(*io_context, delay)
{
}

Timer::Timer(boost::asio::io_context* io_context,
              boost::posix_time::millisec delay,
              boost::posix_time::millisec interval,
              TimeoutCallback callback):
        repeat_(true),
        canceled_(false),
        interval_(interval),
        callback_(callback),
        timer_(*io_context, delay)
{
}

Timer::Timer(boost::asio::io_context* io_context,
             boost::posix_time::millisec delay,
             boost::posix_time::millisec interval,
             boost::function<void(void)> callback):
        repeat_(true),
        canceled_(false),
        interval_(interval),
        callback_(std::move(callback)),
        timer_(*io_context, delay)
{
}

void Timer::Repeat()
{
    auto expires = timer_.expires_at();
    auto next = expires + interval_;
    timer_.expires_at(next);
}

void Timer::Schedule(boost::function<void(boost::system::error_code)> timeout)
{
    timer_.async_wait(timeout);
}

void Timer::CancelTimer()
{
    canceled_ = true;
    timer_.cancel();
}

void Timer::Timeout()
{
    callback_();
}







