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
        static boost::asio::io_context* Get();


    private:
        static boost::asio::io_context* Context_;
    };
}

