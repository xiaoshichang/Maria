
message(STATUS "Setup ZeroMQ Library...")
if (WIN32)
    set(ZeroMQ_ROOT C:\\ZeroMQ)
else()
endif()
find_package(ZeroMQ REQUIRED)

message("ZeroMQ_FOUND: ${ZeroMQ_FOUND}")
message("ZeroMQ_INCLUDE_DIR: ${ZeroMQ_INCLUDE_DIR}")
message("ZeroMQ_LIBRARY: ${ZeroMQ_LIBRARY}")
message("ZeroMQ_STATIC_LIBRARY: ${ZeroMQ_STATIC_LIBRARY}")

