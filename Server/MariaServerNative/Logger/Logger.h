//
// Created by xiao on 2022/10/15.
//
// see https://scicomp.ethz.ch/public/manual/Boost/1.55.0/log_doc.pdf

#pragma once

#include <fstream>
#include <iostream>
#include <boost/smart_ptr/shared_ptr.hpp>

#include <boost/log/sources/severity_logger.hpp>
#include <boost/log/sources/record_ostream.hpp>
#include <boost/log/sources/severity_feature.hpp>

#include <boost/log/sinks/sync_frontend.hpp>
#include <boost/log/sinks/text_ostream_backend.hpp>

#include <boost/log/expressions.hpp>
#include <boost/log/expressions/formatters/stream.hpp>
#include <boost/log/expressions/formatters/date_time.hpp>

#include <boost/log/trivial.hpp>
#include <boost/log/support/date_time.hpp>

#include <boost/log/utility/setup/file.hpp>
#include <boost/log/utility/setup/console.hpp>
#include <boost/log/utility/setup/common_attributes.hpp>


namespace logging = boost::log;
namespace sinks = boost::log::sinks;
namespace src = boost::log::sources;
namespace expr = boost::log::expressions;
namespace attrs = boost::log::attributes;
namespace keywords = boost::log::keywords;


namespace Maria::Server::Native
{
    enum LogTag
    {
        Native,
        Managed
    };


    class Logger
    {
    public:
        static void Init(const char* target, const char* fileName);
        static void Finalize();

    private:
        static void InitLoggingCore();
        static void InitConsoleSink();
        static void InitFileSink(const char* target, const char* fileName);

    public:
        static void Error(const char* message, LogTag tag=LogTag::Native);
        static void Error(const std::string& message, LogTag tag=LogTag::Native);
        static void Warning(const char* message, LogTag tag=LogTag::Native);
        static void Warning(const std::string& message, LogTag tag=LogTag::Native);
        static void Info(const char* message, LogTag tag=LogTag::Native);
        static void Info(const std::string& message, LogTag tag=LogTag::Native);
        static void Debug(const char* message, LogTag tag=LogTag::Native);
        static void Debug(const std::string& message, LogTag tag=LogTag::Native);

        static void Log(logging::trivial::severity_level sensitive, const char* message, LogTag tag);

    private:
        static src::severity_logger_mt<logging::trivial::severity_level> core_logger_;
        static src::severity_logger_mt<logging::trivial::severity_level> managed_logger_;
        static boost::shared_ptr<sinks::synchronous_sink<sinks::basic_text_ostream_backend<char>>> console_sink_;
        static boost::shared_ptr<sinks::synchronous_sink< sinks::text_file_backend>> file_sink_;
    };
}
