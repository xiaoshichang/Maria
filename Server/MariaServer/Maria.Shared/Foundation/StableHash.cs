using System;

namespace Maria.Shared.Foundation
{
	public static class StableHash
	{
		public static int StringToHash(string str)
		{
			var h = 0;
			for (var i = 0; i < str.Length; i++)
			{
				h = 31 * h + str[i];
			}
			return h;
		}
		
		public static int TypeToHash(Type t)
		{
			return StringToHash(t.Name);
		}
	}
}

