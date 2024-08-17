using Maria.Shared.Serialize;
using UnityEngine;

public static class NetworkManager
{
	public static void Init()
	{
		int i = 1;
		var s = new Serializer();
		s.Serialize(i);
	}

	public static void UnInit()
	{
		
	}
}
