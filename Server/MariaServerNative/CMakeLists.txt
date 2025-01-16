cmake_minimum_required(VERSION 3.26)

cmake_policy(SET CMP0144 NEW)

project(MariaServerNative)

set(CMAKE_CXX_STANDARD 20)

include(cmake/SetupBoostLibrary.cmake)

add_library(MariaServerNative MODULE
        Logger/Logger.cpp
        Export/ExportAPIs.cpp
        IOContext/IOContext.cpp
        Timer/Timer.cpp
        Timer/TimerMgr.cpp
        Network/NetworkInstance.cpp
        Network/NetworkSession.cpp
        Network/Tcp/TcpNetworkInstance.cpp
        Network/Tcp/TcpSession.cpp
        3rd/kcp/ikcp.h
        3rd/kcp/ikcp.c
)

target_include_directories(MariaServerNative PRIVATE ${Boost_INCLUDE_DIRS})
target_link_directories(MariaServerNative PRIVATE ${Boost_LIBRARY_DIRS})
if (WIN32)
    target_link_libraries(MariaServerNative PRIVATE ws2_32 wsock32 ${Boost_LIBRARIES})
else()
    target_link_libraries(MariaServerNative PRIVATE ${Boost_LIBRARIES})
endif()

set_target_properties(MariaServerNative PROPERTIES OUTPUT_NAME "MariaServerNative")
