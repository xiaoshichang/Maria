using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

#pragma warning disable CS8618

namespace Maria.Server.Application
{
	public class DatabaseConfig
	{
		/// <summary>
		/// Connection String
		/// https://www.mongodb.com/docs/drivers/csharp/current/quick-start/#std-label-csharp-quickstart
		/// </summary>
		public string Connection { get; set; }
		
		/// <summary>
		/// database name
		/// </summary>
		public string DatabaseName { get; set; }
	}

	public class CommonConfig
	{
		/// <summary>
		/// ServerGroup 唯一ID
		/// </summary>
		public string GroupID { get; set; } = string.Empty;
		
		/// <summary>
		/// 游戏业务 Assemblies 列表
		/// </summary>
		public List<string> GameplayAssemblies { get; set; } = new List<string>();
		
		/// <summary>
		/// 日志路径
		/// </summary>
		public string LogPath { get; set; } = string.Empty;
		
		/// <summary>
		/// 数据库相关配置
		/// </summary>
		public DatabaseConfig Database { get; set; }
	}

	public class ServerConfigBase
	{
		/// <summary>
		/// 进程的名称
		/// </summary>
		public string Name { get; set; } = string.Empty;
		
		/// <summary>
		/// 内部网络IP
		/// </summary>
		public string InnerIp { get; set; } = string.Empty;
		
		/// <summary>
		/// 内部网络Port
		/// </summary>
		public int InnerPort { get; set; } = 0;
		
		/// <summary>
		/// Telnet网络Port
		/// </summary>
		public int TelnetPort { get; set; } = 0;
	}

	public class GMServerConfig : ServerConfigBase
	{
	}

	public class GameServerConfig : ServerConfigBase
	{
	}

	public class GateServerConfig : ServerConfigBase
	{
		
		/// <summary>
		/// 外部网络 IP
		/// </summary>
		public string OuterIp { get; set; } = string.Empty;
		
		/// <summary>
		/// 外部网络 Port
		/// </summary>
		public int OuterPort { get; set; }

	}

	public class ServerGroupConfig
	{
		public CommonConfig Common { get; set; }
		public GMServerConfig GMServer { get; set; }
		public List<GameServerConfig> GameServers { get; set; }
		public List<GateServerConfig> GateServers { get; set; }


		public ServerConfigBase? GetServerConfigByName(string servername)
		{
			if (GMServer.Name == servername)
			{
				return GMServer;
			}
			foreach (var config in GameServers)
			{
				if (config.Name == servername)
				{
					return config;
				}
			}
			foreach (var config in GateServers)
			{
				if (config.Name == servername)
				{
					return config;
				}
			}
			return null;
		}

		public static ServerGroupConfig LoadConfig(string configPath)
		{
			string content = File.ReadAllText(configPath);
			try
			{
				var groupConfig = JsonSerializer.Deserialize<ServerGroupConfig>(content);
				if (groupConfig == null)
				{
					throw new ApplicationException("Invalid config file");
				}
				groupConfig._InitIDMapping();
				return groupConfig;
			}
			catch (Exception e)
			{
				throw new Exception($"Failed to parse config file: {configPath}", e);
			}
		}

		public GMServerConfig GetGMConfig()
		{
			return GMServer;
		}

		public int GetTotalGameAndGateCount()
		{
			return GameServers.Count + GateServers.Count;
		}
		
		public int GetServerGroupNodesCount()
		{
			return 1 + GetTotalGameAndGateCount();
		}

		private void _InitIDMapping()
		{
			_ID2ConfigMapping.Clear();
			_Config2IDMapping.Clear();

			_ID2ConfigMapping[GM_INDEX] = GMServer;
			_Config2IDMapping[GMServer] = GM_INDEX;

			if (GameServers.Count > MAX_GAME_COUNT)
			{
				throw new IndexOutOfRangeException("game server config out of index");
			}
			
			for (int i = 0; i < GameServers.Count; i++)
			{
				var game = GameServers[i];
				var idx = GAME_START_INDEX + i;
				_ID2ConfigMapping[idx] = game;
				_Config2IDMapping[game] = idx;
			}

			if (GateServers.Count > MAX_GATE_COUNT)
			{
				throw new IndexOutOfRangeException("gate server config out of index");
			}
			
			for (int i = 0; i < GateServers.Count; i++)
			{
				var gate = GateServers[i];
				var idx = GATE_START_INDEX + i;
				_ID2ConfigMapping[idx] = gate;
				_Config2IDMapping[gate] = idx;
			}
		}

		public ServerConfigBase GetConfigByIndex(int idx)
		{
			if (_ID2ConfigMapping.TryGetValue(idx, out var config))
			{
				return config;
			}
			throw new IndexOutOfRangeException("index not found");
		}

		public int GetIDByConfig(ServerConfigBase config)
		{
			if (_Config2IDMapping.TryGetValue(config, out var idx))
			{
				return idx;
			}
			throw new IndexOutOfRangeException("config not found");
		}

		/// <summary>
		/// GM进程的ID
		/// </summary>
		private const int GM_INDEX = 0;
		
		/// <summary>
		/// Game进程最大数量
		/// </summary>
		private const int MAX_GAME_COUNT = 10000000;
		
		/// <summary>
		/// Game进程的起始 ID
		/// </summary>
		private const int GAME_START_INDEX = 10000000;
		
		/// <summary>
		/// Gate进程的最大数量
		/// </summary>
		private const int MAX_GATE_COUNT = 10000000;
		
		/// <summary>
		/// Gate进程的起始 ID
		/// </summary>
		private const int GATE_START_INDEX = 20000000;
		
		private readonly Dictionary<int, ServerConfigBase> _ID2ConfigMapping = new();
		private readonly Dictionary<ServerConfigBase, int> _Config2IDMapping = new ();
	}
}



