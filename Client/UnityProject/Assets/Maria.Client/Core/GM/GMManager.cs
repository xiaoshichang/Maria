using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Maria.Client.Application;
using Maria.Client.Foundation.Log;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Maria.Core.GM
{
	public class GMManagerInitParam
	{
		public ApplicationRoot ApplicationRoot; 
		public List<string> AssemblyList;
	}
	
	public static class GMManager
	{
		/// <summary>
		/// 初始化系统
		/// </summary>
		public static void Init(List<string> assemblies)
		{
			// create gm terminal and bind to gm system
			_Root = new GameObject("GMManager");
			_Root.AddComponent<GMTerminal>();
			
			// collect from framework
			CollectGMCommandsByReflection(Assembly.GetExecutingAssembly());

			// collect from gameplay
			foreach (var assemblyName in assemblies)
			{
				var assembly = Assembly.Load(assemblyName);
				CollectGMCommandsByReflection(assembly);
			}
			
			MLogger.Info($"Init GMManager OK. {_AllGMCommands.Count} commands collected.");
		}
		
		public static void UnInit()
		{
			Object.Destroy(_Root);
			_AllGMCommands.Clear();
		}
		
		
		private static void _RegisterGMCommand(GMAttribute attr, MethodInfo method)
		{
			if (_AllGMCommands.ContainsKey(attr.Command))
			{
				MLogger.Error($"duplicated gm name {method.Name}");
				return;
			}

			var cmd = new GMCommand()
			{
				Attribute = attr,
				Method = method
			};
			_AllGMCommands.Add(attr.Command, cmd);
		}
		
		/// <summary>
		/// 从Assembly中搜集GM指令
		/// </summary>
		public static void CollectGMCommandsByReflection(Assembly assembly)
		{
			if (assembly == null)
			{
				return;
			}
			
			foreach (var t in assembly.GetTypes())
			{
				foreach (var method in t.GetMethods())
				{
					var attrs = method.GetCustomAttributes();
					foreach (var attr in attrs)
					{
						if (attr is GMAttribute gma)
						{
							_RegisterGMCommand(gma, method);
						}
					}
				}
			}
		}

		private static object _ConvertParameter(string item, Type t)
		{
			try
			{
				if (t == typeof(int))
				{
					return int.Parse(item);
				}

				if (t == typeof(float))
				{
					return float.Parse(item);
				}

				if (t == typeof(string))
				{
					return item;
				}

				if (t == typeof(uint))
				{
					return uint.Parse(item);
				}

				if (t == typeof(double))
				{
					return double.Parse(item);
				}
			}
			catch (Exception e)
			{
				MLogger.Error($"{e.Message}! Content:{item}), Type:{t.Name}");
			}
			return null;
		}

		private static void _PrintCandidates(string prefix)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("\n>>>>>>>>>>>> relative commands >>>>>>>>>>>>\n");
			foreach (var kv in _AllGMCommands)
			{
				if (kv.Key.StartsWith(prefix))
				{
					var cmd = kv.Value;
					sb.Append($"{kv.Key} - {cmd.Attribute.Desc} \n");
				}
			}
			sb.Append("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<\n");
			MLogger.Warning(sb.ToString());
		}

		/// <summary>
		/// 执行GM命令
		/// </summary>
		public static bool Execute(string input)
		{
			var items = input.Split();
			if (items.Length <= 0)
			{
				MLogger.Error("gm input invalid!");
				return false;
			}
			var cmdName = items[0];
			if (!_AllGMCommands.TryGetValue(cmdName, out var cmd))
			{
				MLogger.Warning($"[{cmdName}] not found!");
				_PrintCandidates(cmdName);
				return false;
			}

			var infos = cmd.Method.GetParameters();
			var count = infos.Length;
			if (count != items.Length - 1)
			{
				MLogger.Error("gm parameter count not match!");
				return false;
			}

			var parameters = new object[count];
			for (var i = 0; i < count; i++)
			{
				var info = infos[i];
				var content = items[i + 1];
				var parameter = _ConvertParameter(content, info.ParameterType);
				if (parameter == null)
				{
					return false;
				}
				parameters[i] = parameter;
			}
			
			try
			{
				cmd.Method.Invoke(null, parameters);
				return true;
			}
			catch (Exception e)
			{
				MLogger.Error(e.Message);
				return false;
			}
		}

		public static int GetCommandsCount()
		{
			return _AllGMCommands.Count;
		}

		private static GameObject _Root;
		private static readonly Dictionary<string, GMCommand> _AllGMCommands = new();


	}
}