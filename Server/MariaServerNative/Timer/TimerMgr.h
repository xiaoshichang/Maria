#pragma once
#include <boost/asio.hpp>
#include <unordered_map>
#include <boost/asio.hpp>
#include <boost/date_time/posix_time/posix_time.hpp>
#include <boost/shared_ptr.hpp>
#include <boost/function.hpp>
#include "Timer.h"


namespace Maria::Server::Native
{

    class TimerMgr
    {

    public:
        static void Init(boost::asio::io_context* io_context);
        static void UnInit();
        static TimerID AllocateTimerID();
        static TimerID AddTimer(unsigned int delay_ms, TimeoutCallback callback);
        static TimerID AddRepeatTimer(unsigned int delay_ms, unsigned int interval_ms, TimeoutCallback callback);
        static TimerID AddRepeatTimer(unsigned int delay_ms, unsigned int interval_ms, boost::function<void(void)> callback);

        static bool CancelTimer(TimerID tid);
        static void OnTimeout(Timer* timer, TimerID tid, const boost::system::error_code& error);
        static unsigned int GetTimersCount();

    private:
        static TimerID global_timer_id;
        static boost::asio::io_context* io_context_;
        static std::unordered_map<TimerID, Timer*> timers_;

    };
}


