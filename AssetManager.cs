using System;
using System.Collections.Generic;

namespace PairedGame
{
	public abstract class AssetManager<T>
	{
		protected static Dictionary<string, T> resourceMap = new Dictionary<string, T>();
		
		public static void AddAsset(string key, T asset)
		{
			if(!IsAssetLoaded(key))
				resourceMap.Add(key, asset);
		}
		
		public static void RemoveAsset(string key)
		{
			resourceMap.Remove(key);
		}
		
		public static bool IsAssetLoaded(string key)
		{
			return resourceMap.ContainsKey(key);	
		}
		
		public static T Get(string key)
		{
			return resourceMap[key];
		}
	}
}

