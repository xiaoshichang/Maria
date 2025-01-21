using System;

namespace Maria.Shared.Entity
{
	public class Entity
	{
		protected Entity()
		{
			Guid = Guid.NewGuid();
		}

		protected Entity(Guid guid, object document)
		{
			Guid = guid;
		}

		public virtual void OnCreate()
		{
		}

		public virtual void OnDestroy()
		{
			
		}
		
		public Guid Guid { get; }
	}
}