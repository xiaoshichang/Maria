add_executable(Test_ZeroMQ main.cpp)

target_include_directories(Test_ZeroMQ PUBLIC ${ZeroMQ_INCLUDE_DIR})
target_link_libraries(Test_ZeroMQ PUBLIC Iphlpapi ${ZeroMQ_STATIC_LIBRARY})

target_include_directories(Test_ZeroMQ PUBLIC "C:\\azmq\\include")
add_compile_definitions(ZMQ_STATIC)

target_include_directories(Test_ZeroMQ PRIVATE ${Boost_INCLUDE_DIRS})
target_link_libraries(Test_ZeroMQ PRIVATE ws2_32 wsock32 ${Boost_LIBRARIES})