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
boost::shared_ptr<sinks::synchronous_sink<sinks::basic_text_ostream_backend<char>>> Logger::console_sink_;
boost::shared_ptr<sinks::synchronous_sink< sinks::text_file_backend>> Logger::file_sink_;


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
    console_sink_ = logging::add_console_log();
    console_sink_->set_formatter(formatter);
    console_sink_->set_filter(logging::trivial::severity >= logging::trivial::debug);
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
    file_sink_ = logging::add_file_log(
        keywords::open_mode = std::ios_base::app,
        keywords::target = target,
        keywords::file_name = fileName
    );

    file_sink_->set_formatter(formatter);
    file_sink_->set_filter(logging::trivial::severity >= logging::trivial::debug);
    file_sink_->locked_backend()->auto_flush();
}

void Logger::Finalize()
{
    console_sink_->flush();
    console_sink_ = nullptr;

    file_sink_->flush();
    file_sink_ = nullptr;
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


