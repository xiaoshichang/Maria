#pragma once

#include <boost/asio.hpp>

namespace Maria::Server::Native
{
    class IOContext
    {
    public:
        static void Init();
        static void Run();
        static void UnInit();
        static void Stop();

    private:
        static void InitTimerManager();
        static void UnInitTimerManager();

    private:
        static boost::asio::io_context* Context_;
    };
}

