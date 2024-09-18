using System;
using System.Collections.Generic;
using System.Reflection;
using Maria.Shared.Foundation;

namespace Maria.Shared.Network
{
	public class NetworkSessionMessage
	{
		public static void RegisterNetworkSessionMessageType(Assembly assembly)
		{
			foreach (var type in assembly.GetTypes())
			{
				if (type.IsSubclassOf(typeof(NetworkSessionMessage)))
				{
					_RegisterNetworkSessionMessageType(type);
				}
			}
		}
		
		private static void _RegisterNetworkSessionMessageType(Type t)
		{
			var index = StableHash.TypeToHash(t);
			if (_Index2Type.TryAdd(index, t))
			{
				_Type2Index[t] = index;
				return;
			}
			
			throw new Exception("hash conflict.");
		}

		public static int GetTypeIDByType(Type t)
		{
			if (_Type2Index.TryGetValue(t, out var tid))
			{
				return tid;
			}

			throw new Exception($"type:{t.Name} is not registered.");
		}

		public static Type GetTypeByTypeID(int tid)
		{
			if (_Index2Type.TryGetValue(tid, out var type))
			{
				return type;
			}
			throw new Exception($"tid:{tid} is not registered.");
		}

		private static readonly Dictionary<int, Type> _Index2Type = new Dictionary<int, Type>();
		private static readonly Dictionary<Type, int> _Type2Index = new Dictionary<Type, int>();
	}
}