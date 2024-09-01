
#include <boost/smart_ptr/make_shared_object.hpp>
#include <boost/bind/bind.hpp>
#include "TimerMgr.h"
#include "../Logger/Logger.h"


namespace Maria::Server::Native
{
    TimerID TimerMgr::global_timer_id = 0;
    boost::asio::io_context* TimerMgr::io_context_;
    std::unordered_map<TimerID, Timer*> TimerMgr::timers_;

    TimerID TimerMgr::AllocateTimerID()
    {
        global_timer_id++;
        return global_timer_id;
    }

    void TimerMgr::Init(boost::asio::io_context* io_context)
    {
        io_context_ = io_context;
    }

    TimerID TimerMgr::AddTimer(unsigned int delay_ms, TimeoutCallback callback)
    {
        auto tid = AllocateTimerID();
        auto delay = boost::posix_time::millisec (delay_ms);
        auto timer = new Timer(io_context_, delay, callback);
        timers_[tid] = timer;

        auto timeout = boost::bind(&TimerMgr::OnTimeout, timer, tid, boost::asio::placeholders::error);
        timer->Schedule(timeout);
        return tid;
    }

    TimerID TimerMgr::AddRepeatTimer(unsigned int delay_ms, unsigned int interval_ms, TimeoutCallback callback)
    {
        auto tid = AllocateTimerID();
        auto delay = boost::posix_time::millisec (delay_ms);
        auto interval = boost::posix_time::millisec (interval_ms);
        auto timer = new Timer(io_context_, delay, interval, callback);
        timers_[tid] = timer;

        auto timeout = boost::bind(&TimerMgr::OnTimeout, timer, tid, boost::asio::placeholders::error);
        timer->Schedule(timeout);
        return tid;
    }

    TimerID
    TimerMgr::AddRepeatTimer(unsigned int delay_ms, unsigned int interval_ms, boost::function<void(void)> callback)
    {
        auto tid = AllocateTimerID();
        auto delay = boost::posix_time::millisec (delay_ms);
        auto interval = boost::posix_time::millisec (interval_ms);
        auto timer = new Timer(io_context_, delay, interval, std::move(callback));
        timers_[tid] = timer;

        auto timeout = boost::bind(&TimerMgr::OnTimeout, timer, tid, boost::asio::placeholders::error);
        timer->Schedule(timeout);
        return tid;
    }

    bool TimerMgr::CancelTimer(TimerID tid)
    {
        auto item = timers_.find(tid);
        if (item == timers_.end())
        {
            return false;
        }
        item->second->CancelTimer();
        timers_.erase(item);
        return true;
    }

    void TimerMgr::UnInit()
    {
        TimerMgr::io_context_ = nullptr;
    }

    void TimerMgr::OnTimeout(Timer* timer, TimerID tid, const boost::system::error_code& error)
    {
        if (error == boost::asio::error::operation_aborted)
        {
            timers_.erase(tid);
            return;
        }
        if (error)
        {
            Logger::Error("unknown error");
            timers_.erase(tid);
            return;
        }

        timer->Timeout();
        if (timer->IsCanceled())
        {
            return;
        }

        if (timer->IsRepeat())
        {
            timer->Repeat();
            auto timeout = boost::bind(&TimerMgr::OnTimeout, timer, tid, boost::asio::placeholders::error);
            timer->Schedule(timeout);
        }
        else
        {
            timers_.erase(tid);
        }
    }

    unsigned int TimerMgr::GetTimersCount()
    {
        return timers_.size();
    }




}

