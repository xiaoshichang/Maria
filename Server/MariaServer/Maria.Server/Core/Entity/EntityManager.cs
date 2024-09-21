using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Maria.Server.Application;
using Maria.Server.Log;
using Maria.Shared.Foundation;

namespace Maria.Server.Core.Entity
{
	public class EntityManager
	{
		public void Init()
		{
			_CollectServerStubTypes();
		}

		public void UnInit()
		{
			
		}
		
		private void _CollectServerStubTypes()
		{
			foreach (var assembly in Program.GameplayAssemblies)
			{
				foreach (var type in assembly.GetTypes())
				{
					if (!type.IsSubclassOf(typeof(ServerEntity)))
					{
						continue;
					}
					var tid = StableHash.TypeToHash(type);
					_AllServerEntityTypes[tid] = type;
					if (!type.IsSubclassOf(typeof(ServerStubEntity)))
					{
						continue;
					}
					_AllServerStubTypes[tid] = type;
				}
			}
		}
		
		public void CreateServerEntity(Type type, params object?[]? objects)
		{
			var entity = Activator.CreateInstance(type, objects) as ServerEntity;
			if (entity == null)
			{
				Logger.Error($"CreateServerEntity. type error. {type.Name}");
				throw new Exception();
			}
			_AllServerEntities[entity.Guid] = entity;
			entity.OnCreate();
		}

		public void DestroyServerEntity(ServerEntity entity)
		{
			if (!_AllServerEntities.ContainsKey(entity.Guid))
			{
				Logger.Error("DestroyServerEntity. known error.");
				return;
			}
			entity.OnDestroy();
			_AllServerEntities.Remove(entity.Guid);
		}

		public Type? GetStubTypeByIndex(int index)
		{
			if (_AllServerStubTypes.TryGetValue(index, out var type))
			{
				return type;
			}
			Logger.Error($"stub type not found. {index}");
			return null;
		}

		public ImmutableDictionary<int, Type> GetAllStubTypes()
		{
			return _AllServerStubTypes.ToImmutableDictionary();
		}
		
		private readonly Dictionary<Guid, ServerEntity> _AllServerEntities = new();
		private readonly Dictionary<int, Type> _AllServerEntityTypes = new();
		private readonly Dictionary<int, Type> _AllServerStubTypes = new();
	}
}

