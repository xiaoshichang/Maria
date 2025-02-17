
message(STATUS "Setup Boost Library...")
if (WIN32)
    add_compile_definitions(_WIN32_WINNT=0x0601)
    set(BOOST_ROOT C:\\Boost)
else()
endif()
find_package(Boost COMPONENTS log REQUIRED)
message("Boost_LIBRARIES: ${Boost_LIBRARIES}")