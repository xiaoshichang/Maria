//
// Created by xiao on 2022/10/15.
//

#include "Logger.h"
#include "boost/date_time/posix_time/posix_time_types.hpp"
#include <boost/date_time/posix_time/posix_time.hpp>
#include <boost/log/attributes/constant.hpp>

using namespace Maria::Server::Native;

src::severity_logger_mt<logging::trivial::severity_level> Logger::core_logger_;
src::severity_logger_mt<logging::trivial::severity_level> Logger::managed_logger_;

void Logger::Init(const char* dir, const char* fileName)
{
    InitLoggingCore();
    InitConsoleSink();
    if (dir != nullptr && fileName != nullptr)
    {
        InitFileSink(dir, fileName);
    }
}

void Logger::InitLoggingCore()
{
    logging::add_common_attributes();
    core_logger_.add_attribute("Tag", attrs::constant<std::string>("Native"));
    managed_logger_.add_attribute("Tag", attrs::constant<std::string>("Managed"));
}

void Logger::InitConsoleSink()
{
    logging::formatter formatter = expr::format("[%1%][%2%][%3%] - %4%")
                                   % expr::format_date_time< boost::posix_time::ptime >("TimeStamp", "%Y-%m-%d %H:%M:%S")
                                   % logging::trivial::severity
                                   % expr::attr<std::string>("Tag")
                                   % expr::message;

    // console
    auto consoleSink = logging::add_console_log();
    consoleSink->set_formatter(formatter);
    consoleSink->set_filter(logging::trivial::severity >= logging::trivial::debug);
}

void Logger::InitFileSink(const char* target, const char* fileName)
{
    // formatter
    logging::formatter formatter = expr::format("[%1%][%2%][%3%] - %4%")
        % expr::format_date_time< boost::posix_time::ptime >("TimeStamp", "%Y-%m-%d %H:%M:%S")
        % logging::trivial::severity
        % expr::attr<std::string>("Tag")
        % expr::message;

    // file
    auto fileSink = logging::add_file_log(
        keywords::open_mode = std::ios_base::app,
        keywords::target = target,
        keywords::file_name = fileName
    );

    fileSink->set_formatter(formatter);
    fileSink->set_filter(logging::trivial::severity >= logging::trivial::debug);
    fileSink->locked_backend()->auto_flush();
}

void Logger::Finalize()
{

}

void Logger::Warning(const char* const message, LogTag tag)
{
    Log(logging::trivial::severity_level::warning, message, tag);
}

void Logger::Warning(const std::string &message, LogTag tag)
{
    Logger::Warning(message.c_str(), tag);
}

void Logger::Error(const char* const message, LogTag tag)
{
    Log(logging::trivial::severity_level::error, message, tag);
}

void Logger::Error(const std::string &message, LogTag tag)
{
    Logger::Error(message.c_str(), tag);
}

void Logger::Info(const char* const message, LogTag tag)
{
    Log(logging::trivial::severity_level::info, message, tag);
}

void Logger::Info(const std::string &message, LogTag tag)
{
    Logger::Info(message.c_str(), tag);
}

void Logger::Debug(const char* const message, LogTag tag)
{
    Log(logging::trivial::severity_level::debug, message, tag);
}

void Logger::Debug(const std::string &message, LogTag tag)
{
    Logger::Debug(message.c_str(), tag);
}

void Logger::Log(logging::trivial::severity_level severity, const char* message, LogTag tag)
{
    if (tag == LogTag::Native)
    {
        BOOST_LOG_SEV(core_logger_, severity) << message;
    }
    else
    {
        BOOST_LOG_SEV(managed_logger_, severity) << message;
    }
}


