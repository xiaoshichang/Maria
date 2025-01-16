using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Maria.Client.Foundation.Log
{
	public enum LogLevel : byte
	{
		Debug,
		Info,
		Warning,
		Error
	}
	
	public static class MLogger
	{
		private static readonly string _RecordFormat = "[{0}][{1}] - {2}";
		
#if UNITY_EDITOR
		private static readonly Dictionary<LogLevel, string> _LevelToColor = new Dictionary<LogLevel, string>()
		{
			{LogLevel.Debug, "<color=#00FF00>"},
			{LogLevel.Info, "<color=#FFFFFF>"},
			{LogLevel.Warning, "<color=#FFFF00>"},
			{LogLevel.Error, "<color=#FF3333>"}
		};
#endif

		private static void _PrintBuildType()
		{
			var buildType = "NotDefined";
#if MARIA_BUILDTYPE_DEBUG
			buildType = "Debug";
#endif
#if MARIA_BUILDTYPE_RELEASE
			buildType = "Release";
#endif
			Info($"Build Type: {buildType}");
		}

		private static void _PrintRuntimePlatform()
		{
			var platform = UnityEngine.Application.platform;
			if (platform == RuntimePlatform.WindowsPlayer)
			{
				Info("RuntimePlatform: WindowsPlayer");
			}
			else if (platform == RuntimePlatform.WindowsEditor)
			{
				Info("RuntimePlatform: WindowsEditor");
			}
			else if (platform == RuntimePlatform.Android)
			{
				Info("RuntimePlatform: Android");
			}
			else if (platform == RuntimePlatform.IPhonePlayer)
			{
				Info("RuntimePlatform: IPhonePlayer");
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public static void Init()
		{
			_PrintRuntimePlatform();
			_PrintBuildType();
		}

		private static string _BuildRecord(LogLevel level, string message)
		{
			var ts =  DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
			var record = string.Format(_RecordFormat, ts, level.ToString(), message);
#if UNITY_EDITOR
			record = _LevelToColor[level] + record + "</color>";
#endif
			return record;
		}
		
		/// <summary>
		/// Debug 用于开发环境下输出调试信息
		/// </summary>
		[Conditional("MARIA_BUILDTYPE_DEBUG")]
		public static void Debug(string message)
		{
			var record = _BuildRecord(LogLevel.Debug, message);
			UnityEngine.Debug.Log(record);
		}
		
		/// <summary>
		/// Debug 用于开发环境下输出调试信息
		/// </summary>
		[Conditional("MARIA_BUILDTYPE_DEBUG")]
		public static void Debug(string format, params object[] args)
		{
			var message = string.Format(format, args);
			var record = _BuildRecord(LogLevel.Debug, message);
			UnityEngine.Debug.Log(record);
		}

		/// <summary>
		/// Info 用于输出普通日志
		/// </summary>
		public static void Info(string message)
		{
			var record = _BuildRecord(LogLevel.Info, message);
			UnityEngine.Debug.Log(record);
		}
		
		/// <summary>
		/// Info 用于输出普通日志
		/// </summary>
		public static void Info(string format, params object[] args)
		{
			var message = string.Format(format, args);
			var record = _BuildRecord(LogLevel.Info, message);
			UnityEngine.Debug.Log(record);
		}

		/// <summary>
		/// Warning  代表发生了错误，但是不影响主流程和底层模块，只影响部分业务模块
		/// </summary>
		public static void Warning(string message)
		{
			var record = _BuildRecord(LogLevel.Warning, message);
			UnityEngine.Debug.LogWarning(record);
		}
		
		/// <summary>
		/// Warning  代表发生了错误，但是不影响主流程和底层模块，只影响部分业务模块
		/// </summary>
		public static void Warning(string format, params object[] args)
		{
			var message = string.Format(format, args);
			var record = _BuildRecord(LogLevel.Warning, message);
			UnityEngine.Debug.LogWarning(record);
		}

		/// <summary>
		/// Error 代表发生严重错误，正常的流程不允许存在
		/// </summary>
		public static void Error(string message)
		{
			var record = _BuildRecord(LogLevel.Error, message);
			UnityEngine.Debug.LogError(record);
		}

		
		/// <summary>
		/// Error 代表发生严重错误，正常的流程不允许存在
		/// </summary>
		public static void Error(string format, params object[] args)
		{
			var message = string.Format(format, args);
			var record = _BuildRecord(LogLevel.Error, message);
			UnityEngine.Debug.LogError(record);
		}

		/// <summary>
		/// Asset 代表程序员的预期，不符合预期直接抛出异常，避免错误继续
		/// </summary>
		[Conditional("MARIA_BUILDTYPE_DEBUG")]
		public static void Assert(bool condition, string message)
		{
			UnityEngine.Debug.Assert(condition, message);
		}

		[Conditional("MARIA_BUILDTYPE_DEBUG")]
		public static void Assert(bool condition)
		{
			UnityEngine.Debug.Assert(condition);
		}
		
	}
}